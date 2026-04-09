# ScheduleViewer — CLAUDE.md

## プロジェクト概要

ScheduleViewer は .NET 9.0 WPF アプリケーション。Google Calendar / Fitbit / Annict 等の外部APIから
データを取得し、スケジュールを表示する。

**移行方針: 段階的移行 (Staged Migration)**
- WPF クライアントはそのまま維持
- バックエンドを Spring Boot REST API (`schedule-viewer-api`) として並行構築
- 将来的に WPF が直接 API を呼ぶ形に移行

---

## リポジトリ構成

```
ScheduleViewer/
├── ScheduleViewer.sln                  # .NET ソリューション (既存 WPF)
├── ScheduleViewer.Domain/              # .NET ドメイン層
├── ScheduleViewer.Infrastructure/      # .NET インフラ層
│   ├── ScheduleViewer.db               # SQLite DB (実データ)
│   └── client_secret_732519...json     # Google OAuth クライアントシークレット
├── ScheduleViewer.WPF/                 # .NET WPF UI層
└── schedule-viewer-api/                # Spring Boot REST API (新規)
    ├── pom.xml                         # 親 POM (multi-module)
    ├── domain/                         # ドメイン層 (Java)
    ├── infrastructure/                 # インフラ層 (Java)
    └── api/                            # REST API 層 (Java)
```

---

## Spring Boot プロジェクト (`schedule-viewer-api`)

### 技術スタック

| 項目 | バージョン |
|------|-----------|
| Spring Boot | 3.3.5 |
| Java | 21 (Virtual Threads 使用) |
| Maven | 3.9.14 (インストール先: `Downloads/apache-maven-3.9.14-bin/`) |
| SQLite JDBC | xerial 3.45.3.0 |
| Google API Client | 2.6.0 |

### ビルド・起動コマンド

```bat
cd C:\Users\okaji\source\repos\ScheduleViewer\schedule-viewer-api
set MVN=C:\Users\okaji\Downloads\apache-maven-3.9.14-bin\apache-maven-3.9.14\bin\mvn.cmd

:: 全モジュールビルド & インストール
%MVN% install -DskipTests --no-transfer-progress

:: 起動
%MVN% spring-boot:run -pl api --no-transfer-progress
```

起動後、ブラウザで確認:
- **フロントエンド**: http://localhost:9080/
- **Swagger UI**: http://localhost:9080/swagger-ui/index.html
- **API ルート**: http://localhost:9080/api/

### モジュール構成

```
domain/        → エンティティ・バリューオブジェクト・リポジトリインターフェース
infrastructure/ → Google/Fitbit/Annict/SQLite の実装
api/           → REST コントローラー + Spring Boot 起動クラス
```

---

## 実装済みコントローラー一覧

| コントローラー | エンドポイント | 概要 |
|---------------|---------------|------|
| CalendarController | `GET /api/calendar` | 全イベント取得 |
| | `GET /api/calendar/range` | 日付範囲で取得 |
| | `GET /api/calendar/search/title` | タイトル検索 |
| | `POST /api/calendar/reload` | キャッシュ再読み込み |
| FitbitController | `POST /api/fitbit/auth` | PKCE 認証開始 |
| | `GET /api/fitbit/profile` | プロフィール |
| | `GET /api/fitbit/sleep` | 睡眠データ |
| | `GET /api/fitbit/activity` | 運動データ |
| | `GET /api/fitbit/heart` | 心拍データ |
| | `GET /api/fitbit/weight` | 体重データ |
| AnimeController | `GET /api/anime` | Annict でアニメ検索 |
| BooksController | `GET /api/books` | Google Books 検索 |
| PhotoController | `GET /api/photos` | 全写真一覧 (Deprecated) |
| | `GET /api/photos/date/{date}` | 日付で検索 |
| | `POST /api/photos/reload` | 再読み込み |
| TaskController | `GET /api/tasks` | 全タスク (期日降順) |
| | `GET /api/tasks/date/{date}` | 日付で検索 |
| | `POST /api/tasks/reload` | 再読み込み |
| DriveController | `GET /api/drive/expenditure` | 全家計簿データ |
| | `GET /api/drive/expenditure/date/{date}` | 日付で検索 |
| | `POST /api/drive/expenditure/reload` | 再読み込み |
| AuthController | `GET /api/auth/status` | 各サービスの認証状態確認 |
| | `POST /api/auth/google/{service}` | 指定サービスのOAuth認証開始 |
| | `POST /api/auth/google/all` | 全サービスを一括認証 |

---

## C# → Java 型マッピング

| C# | Java |
|----|------|
| `DateOnly` | `LocalDate` |
| `DateTime` | `LocalDateTime` |
| `TimeSpan` | `Duration` |
| `SolidColorBrush` / `Brush` | `String` (CSS カラー hex) |
| `BitmapImage` | 削除 (URL のみ) |
| `(TimeSpan Start, TimeSpan End)` タプル | `record TimeRange(Duration start, Duration end)` |
| `sealed record class` | `record` |
| `IReadOnlyList<T>` | `List<T>` |

---

## 設定ファイル (`application.yml`)

```yaml
spring:
  datasource:
    url: jdbc:sqlite:C:/Users/okaji/source/repos/ScheduleViewer/ScheduleViewer.Infrastructure/ScheduleViewer.db
  jpa:
    hibernate:
      ddl-auto: none   # スキーマ検証を無効化 (JdbcTemplate で直接アクセスするため)

scheduleviewer:
  google:
    client-secret-path: .../client_secret_732519...json
    calendar-id: okajima100@gmail.com
    drive-folder-id: 1sE_XnrJk3U9ZSq9hHUPNgOqPYpmbwXwS
  fitbit:
    client-id: 23PQG4
    redirect-uri: http://localhost:5000/
```

環境変数で上書き可能: `SQLITE_DB_PATH`, `GOOGLE_API_KEY`, `GOOGLE_CLIENT_SECRET_PATH`,
`GOOGLE_CALENDAR_ID`, `FITBIT_CLIENT_ID`, `FITBIT_CLIENT_SECRET`, `ANNICT_TOKEN`

---

## 起動時の動作

`@PostConstruct` により以下がバックグラウンド (Virtual Thread) で非同期起動:

| サービス | 動作 |
|---------|------|
| `CalendarService` | Google Calendar 全件取得 → インメモリキャッシュ |
| `TasksService` | Spreadsheet からリスト取得 → Google Tasks 全件取得 |
| `DriveService` | Google Drive フォルダ内 CSV 取得 → 家計簿データ |
| `PhotoService` | Google Photos 全件取得 (Deprecated) |

**Google 認証フロー**: 初回起動時にブラウザが開き OAuth2 認証を要求する。
認証済みトークンは `~/.scheduleviewer/<token_name>/` に保存される。

---

## 既知の注意事項

### Google Photos API
- 2025/04/01 の仕様変更により大幅制限。`PhotoService` は `@Deprecated` マーク済み。

### Google Books API パッケージ変更
- バージョン `v1-rev20240214-2.0.0` 以降、パッケージが `com.google.api.services.books.v1.*` に変更。
- `BooksService.java` のインポートを修正済み。

### Google API ライブラリバージョン (pom.xml で管理)
| ライブラリ | バージョン |
|-----------|-----------|
| google-api-services-calendar | v3-rev20250404-2.0.0 |
| google-api-services-books | v1-rev20240214-2.0.0 |
| google-api-services-drive | v3-rev20250511-2.0.0 |
| google-api-services-tasks | v1-rev20250518-2.0.0 |
| google-api-services-sheets | v4-rev20250603-2.0.0 |

### Tasks API `setMaxResults`
- 新バージョンで `long` → `Integer` に変更。`100` (int リテラル) を使用。

### Calendar API ソート
- `DateTime.compareTo()` の代わりに `Long.compare(sa.getValue(), sb.getValue())` を使用。

---

## Spreadsheet IDs (SpreadsheetService)

| 用途 | スプレッドシート ID |
|------|------------------|
| タスクリスト一覧 | `1tc5uFTh09PBVVnV2OYmGZ3svY6C-6SwCAF6KIUO8l9c` |
| サムネイル一覧・番組情報 | `191fTeVKET2K5yZ6trFewRV3_8GJ80s8qC92-NtgNvv0` |

### スプレッドシートのシート構成 (`191fTeVKET2K5yZ6trFewRV3_8GJ80s8qC92-NtgNvv0`)

| シート名 | 用途 | キー列 | 値列 |
|---------|------|--------|------|
| `サムネイル` | シリーズサムネイル画像 | B列: タイトル | C列: 画像URL |
| `取得(番組)` | 各話概要テキスト | タイトル列 (ヘッダー自動検出) | 概要列 (ヘッダー自動検出) |
| `サムネイル(アニメ各話)` | 各話サムネイル画像 | A列: 名称 (`のんのんびより 第2話` 形式) | B列: URL |

- **タイトル検索ロジック**: 完全一致 → 前方/後方一致 → 部分一致の順で検索
- **`_` とスペースは同一視**: 検索時に両者を正規化して比較 (`searchByTitle` 内で処理)
- **キャッシュ方式**: 遅延ロード (初回アクセス時に構築)、「全て再読込」ボタンで破棄

---

## 自動起動設定

PC起動時に自動起動するための設定ファイルを用意済み:

| ファイル | 用途 |
|---------|------|
| `start-server.bat` | サーバー起動 + `server.log` にログ出力 |
| `start-server-silent.vbs` | コンソール非表示で bat を起動 (タスクスケジューラから呼び出す) |

**タスクスケジューラの設定**: ログオン時トリガー、`wscript.exe` で VBS を実行

---

## 現在の作業状況
- 最終更新: 2026-03-31

### 完了済み
- Spring Boot REST API の全コントローラー実装 (Calendar / Fitbit / Anime / Books / Photo / Tasks / Drive)
- `AuthController` 追加 (`GET /api/auth/status`, `POST /api/auth/google/{service}`, `POST /api/auth/google/all`)
- Web フロントエンド完成 (`static/index.html` + `css/style.css` + `js/app.js`)
  - サイドバーカレンダー + 7タブ詳細パネル (スケジュール/タスク/健康/本/収支/写真/アニメ)
  - 認証管理パネル (各サービスの認証状態表示 + 認証ボタン)
  - 写真拡大モーダル
- Google OAuth トークンの起動時ガード実装
  - `hasToken()` でトークンファイル存在確認 (50バイト以上で有効判定)
  - トークン未設定のサービスは `@PostConstruct` をスキップ、認証管理パネルから手動認証可能
- 各種バグ修正 (〜2026-03-23):
  - `CalendarEventsEntity` の終日イベントコンストラクタで `isAllDay` が常に false になる問題
  - `Tasks.setMaxResults()` の型不一致 (`long` → `int`)
  - `Calendar` ソートの `DateTime.compareTo()` を `Long.compare()` に変更
  - `BooksService` のパッケージ名変更 (`books.v1.*`)
  - SQLite DB パスの修正
  - `ddl-auto: none` に変更 (validate でスキーマエラー)
  - ポート 9080 に変更 (8080/8090 が競合)
- 各種バグ修正・機能追加 (2026-03-25):
  - **支出タブ重複バグ修正**: `DriveService.listFilesInFolder` のクエリに `and trashed=false` を追加
    - ゴミ箱内の旧CSVファイルが取得されて同一データが重複表示されていた問題を解消
  - **`SpreadsheetController` に概要・各話サムネイルエンドポイント追加**:
    - `GET /api/spreadsheet/caption?title=` → `取得(番組)` シートから概要取得
    - `POST /api/spreadsheet/caption/reload`
    - `GET /api/spreadsheet/episode-thumbnail?title=` → `サムネイル(アニメ各話)` シートから各話画像URL取得
    - `POST /api/spreadsheet/episode-thumbnail/reload`
  - **アニメタブ: 概要をスプレッドシートから取得**:
    - `AnnictService` に `SpreadsheetService` を注入
    - Annict 登録有無にかかわらず `取得(番組)` シートの概要を優先表示
    - 概要検索キーをシリーズ名 (`searchWord`) → カレンダーのフルタイトル (`calTitle`) に修正
  - **アニメタブ: 各話サムネイル表示機能追加**:
    - シリーズサムネイルの下に各話サムネイルを表示 (`サムネイル(アニメ各話)` シート参照)
    - 該当エントリがない場合は非表示
    - CSS クラス `.anime-episode-thumb` 追加
  - **アニメタブ: Annict 検索タイトルの正規化**:
    - カレンダータイトルの `_` をスペースに変換した正規化タイトル (`normalizedTitle`) を導入
    - `searchWord` = 正規化タイトルの最初の単語 (例: `ジョジョの奇妙な冒険`)
    - `matchTitle` = 正規化タイトルから `getAnimeMatchTitle()` で導出 (例: `ジョジョの奇妙な冒険 ストーンオーシャン`)
    - サムネイル検索キーを `searchWord` → `matchTitle` に変更 (誤った別シリーズのサムネイルが返る問題を修正)
  - **`searchByTitle` で `_` とスペースを同一視**:
    - キーと検索語の両方で `_` → スペース変換後に比較
    - スプレッドシートが `_` 区切りのタイトルでも正しくマッチするように
  - **「全て再読込」にスプレッドシートキャッシュのリロードを追加**:
    - サムネイル・概要・各話サムネイルの3キャッシュも一括クリア
  - **自動起動スクリプト作成**: `start-server.bat` / `start-server-silent.vbs`
- フロントエンド修正 (2026-03-31):
  - **写真タブ: description の `<br>` + `<a href>` 対応**:
    - `extractPhotoUrls()` で `<br>` を `\n` に正規化後に行分割
    - `<a href="url">` 形式の URL 抽出に対応
    - `【写真】` 直後に複数 URL が続く場合も全件取得するよう修正
    - 返り値を `string[]` → `{url, title}[]` に変更し、写真リンクのラベルをイベントタイトルで表示
  - **スケジュールタブ: description の HTML 表示対応**:
    - `descToSafeHtml()` ヘルパーを追加
    - `<br>` を改行、`<a href>` を URL テキストに変換してからエスケープ表示
  - **静的ファイルの配信**: `target/classes/static/` が `src/main/resources/static/` より優先されるため、JS/CSS 変更後は `target` へのコピーが必要

### 次のタスク
- Google 各サービスの認証を完了させる
  1. `http://localhost:9080/` を開く
  2. 「🔑 認証管理」ボタン → 各サービスの「認証する」を押してブラウザでOAuth完了

### 未着手
- Fitbit PKCE 認証フローの完全実装・テスト
- OAuth2トークン管理のDB/セッション移行
- SQLite → JPA移行
