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
| CalendarController | `GET /api/calendar` | 日付でイベント取得 |
| | `GET /api/calendar/range` | 日付範囲で取得 |
| | `GET /api/calendar/search` | キーワード検索 (タイトル・場所・説明、最大10件) |
| | `GET /api/calendar/search/title` | タイトル検索 |
| | `GET /api/calendar/search/address` | 場所検索 |
| | `GET /api/calendar/search/description` | 説明検索 |
| | `GET /api/calendar/anime` | 日付でアニメイベント取得 |
| | `POST /api/calendar/reload` | キャッシュ再読み込み |
| AnimeRegisterController | `POST /api/anime/register` | アニメ視聴記録をカレンダーに登録 |
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
| | `GET /api/photos/local/date/{date}` | ローカルフォルダから日付で検索 |
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
- 最終更新: 2026-04-10

### 完了済み
- Spring Boot REST API の全コントローラー実装 (Calendar / Fitbit / Anime / Books / Photo / Tasks / Drive)
- `AuthController` 追加 (`GET /api/auth/status`, `POST /api/auth/google/{service}`, `POST /api/auth/google/all`)
- Web フロントエンド完成 (`static/index.html` + `css/style.css` + `js/app.js`)
  - サイドバーカレンダー + 5タブ詳細パネル (スケジュール/タスク/健康/本/アニメ)
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
  - **`SpreadsheetController` に概要・各話サムネイルエンドポイント追加**:
    - `GET /api/spreadsheet/caption?title=` → `取得(番組)` シートから概要取得
    - `GET /api/spreadsheet/episode-thumbnail?title=` → `サムネイル(アニメ各話)` シートから各話画像URL取得
  - **アニメタブ: 各話サムネイル表示機能追加** (`サムネイル(アニメ各話)` シート参照)
  - **アニメタブ: Annict 検索タイトルの正規化** (`normalizedTitle` / `searchWord` / `matchTitle`)
  - **`searchByTitle` で `_` とスペースを同一視**
  - **自動起動スクリプト作成**: `start-server.bat` / `start-server-silent.vbs`
- フロントエンド修正 (2026-03-31):
  - **写真タブ: description の `<br>` + `<a href>` 対応** (`extractPhotoUrls()`)
  - **スケジュールタブ: description の HTML 表示対応** (`descToSafeHtml()`)
  - **静的ファイルの配信**: `target/classes/static/` が優先されるため JS/CSS 変更後は `target` へのコピーが必要
- フロントエンド修正 (2026-04-09):
  - **ローカル写真サービス追加** (`LocalPhotoService`, `WebMvcConfig`):
    - `GET /api/photos/local/date/{date}` → `C:/Users/okaji/Desktop/Google Photo/{yyyy}年/{m}月/{yyyymmdd}/`
    - `/local-photos/**` を静的リソースとして公開
  - **写真タブ・収支タブをスケジュールタブに統合**
  - **終日イベントの時刻非表示** (`renderEventCard`)
  - **添付ファイル表示機能追加** (`CalendarEventsEntity.attachments`, MIMEタイプ別アイコン)
  - **現在のスケジュールタブの表示順**: 終日 → 時間指定 → 写真 → 添付ファイル → 収支
  - **現在のタブ構成**: スケジュール / タスク / 健康 / 本 / アニメ (5タブ)
- 機能追加・修正 (2026-04-10):
  - **カレンダー検索機能追加**:
    - `GET /api/calendar/search?q=` エンドポイント追加 (タイトル・場所・説明の部分一致、最大10件)
    - `CalendarService.search(q)` メソッド追加
    - フロントエンド: メインエリア上部に検索バー配置
    - ドロップダウン候補表示 (2文字以上で起動、300ms デバウンス)
    - 時間指定イベントは「タイトル / 日付 / 時刻 / 場所」、全日は「タイトルのみ」
    - キーボード操作: ↑↓で候補選択、Enter でイベント詳細モーダル表示、Esc で閉じる
    - 候補クリック or Enter でカレンダーの該当日付へ自動移動
    - イベント詳細モーダル: 日付・時間・場所・詳細を表示
  - **アニメ視聴登録機能追加** (`AnimeRegisterController`):
    - `POST /api/anime/register` エンドポイント新規作成
    - サイドバーに「▶ アニメ視聴登録」ボタン (紫色、「全て再読込」の上)
    - モーダルフォーム: 視聴日(デフォルト: 今日) / タイトル / 話数 / サブタイトル / 視聴先(デフォルト: dアニメストア) / 概要
    - Google Calendar に全日イベントとして登録 (カラー: フラミンゴ = colorId "4")
    - イベントタイトル形式: `{タイトル} 第{話数}話`
    - description 形式: `\n【サブタイトル】\n...\n\n【視聴先】\n...\n\n【概要】\n...`
    - 登録後、選択中の日付と一致する場合はスケジュールタブを自動再読み込み
    - `CalendarService` のスコープを `CALENDAR_READONLY` → `CALENDAR` に変更 (書き込み権限追加)
      - **スコープ変更後は `~/.scheduleviewer/token_Calendar/StoredCredential` を削除して再認証が必要**
  - **アニメタブ: 概要表示をカレンダーイベントから取得するよう変更**:
    - スプレッドシートの `/api/spreadsheet/caption` 呼び出しを廃止
    - `parseAnimeDesc()` を多行対応に改善 (従来は次の1行のみ取得していた)
    - `desc['概要']` (カレンダーイベントの `【概要】` セクション) を直接使用

### 未着手
- Fitbit PKCE 認証フローの完全実装・テスト
- OAuth2トークン管理のDB/セッション移行
- SQLite → JPA移行
