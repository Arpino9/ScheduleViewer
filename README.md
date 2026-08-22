# ScheduleViewer

Google Calendarを中心に、予定、タスク、健康、読書、アニメ、収支などの日別情報をまとめて確認する個人向けアプリケーションです。

デスクトップ版（WPF）に加えて、Blazor WebAssemblyとJava APIで構成したWeb版を開発しています。

## アプリケーション構成

| ディレクトリ | 役割 | 主な技術 |
| --- | --- | --- |
| `ScheduleViewer.Web` | Webフロントエンド | Blazor WebAssembly / .NET 10 |
| `schedule-viewer-api` | Web版のバックエンドAPI | Java 21 / Spring Boot |
| `ScheduleViewer.WPF` | デスクトップ版 | WPF / .NET 9 |
| `ScheduleViewer.Domain` | デスクトップ版のドメイン層 | C# / .NET 9 |
| `ScheduleViewer.Infrastructure` | デスクトップ版の外部サービス連携 | C# / .NET 9 |

## Blazor版の主な機能

### 日別ビュー

- 月間カレンダーから日付を選択
- 「スケジュール」「タスク」「健康」「本」「アニメ」の5タブ表示
- カレンダー予定のタイトル、場所、詳細をキーワード検索
- Google Tasksの期限・完了状態を表示
- Google Healthの歩数、距離、消費カロリー、睡眠、体重、心拍数などを表示
- 読書記録、アニメ視聴記録、Google Sheets由来の収支を日別表示
- 日付を素早く切り替えた場合、古い通信結果で新しい画面を上書きしないよう制御

### 地図表示

Google Calendarの予定に「場所」が設定されている場合、その文字列をJava API経由でNominatimへ問い合わせ、OpenStreetMap上に表示します。

- 予定カード内に地図サムネイルを表示
- サムネイルから拡大モーダルを表示
- 同じ地点をOpenStreetMapのサイトで開くリンクを提供
- 場所が未設定、または位置情報を取得できない予定では地図を表示しない

地図表示にはGoogle Maps APIキーを使用しません。Nominatimの接続先は `NOMINATIM_BASE_URL` で変更できます。

### Steam実績画像

Google Sheetsに登録したゲームタイトル、実績名、画像URLを予定と照合し、該当するSteam実績画像を予定カードに表示します。「全て再読込」では実績画像のキャッシュも更新します。

### Box／Google Photos添付

Google Calendarの予定に外部リンクを追加し、日別ビューから開けます。

- Box共有リンクは表示名とともにイベントの非公開拡張プロパティへ保存
- Google Photos共有URLはイベント説明の `【写真】` セクションへ追加
- BoxはHTTPSの `box.com` とそのサブドメインのみ許可
- Google PhotosはHTTPSの `photos.google.com` と `photos.app.goo.gl` のみ許可
- 不正な形式のURLや、許可していないホストのURLは登録しない

Google Photos APIから写真一覧を取得する旧機能は、2025年4月の仕様変更により非推奨です。現在のBlazor版では共有URLの登録・表示を利用します。

### アニメ視聴登録

視聴日、作品名、話数、サブタイトル、視聴先、概要を入力し、Google Calendarへ全日イベントとして登録します。登録後はカレンダーを再読み込みし、Annictの作品情報やGoogle Sheetsのサムネイルと合わせて表示します。

### Google Fit Takeoutインポート

Google FitのTakeoutで取得したJSONファイルから、対応する健康データをGoogle Healthへ取り込めます。

- 対応データは運動セッション、睡眠、体重
- 複数ファイルをまとめて選択可能
- 読み取り認証とは別に、Google Healthへの書き込み認証を使用
- 1ファイル25MB、選択ファイル合計50MB、1ファイル10,000データポイントまで
- ファイルごとに登録、重複、失敗件数を表示

### 認証管理

画面から次のサービスの認証状態を確認し、未認証のサービスは認証URLを取得できます。

- Google Calendar
- Google Tasks
- Google Drive
- Google Photos
- Google Sheets
- Google Health

認証フローではローカル環境のブラウザを使用するため、Java APIとブラウザは同じPC上で動かしてください。

## 連携サービス

| サービス | 用途 |
| --- | --- |
| Google Calendar | 予定、読書・アニメ記録、添付情報の保存 |
| Google Tasks | タスク表示 |
| Google Drive / Box | 関連ファイルへのリンク |
| Google Sheets | 収支、アニメ画像、Steam実績画像 |
| Google Books | 書籍情報 |
| Google Health | 活動、睡眠、身体データ |
| Annict | アニメ作品情報 |
| Nominatim / OpenStreetMap | 住所の位置情報取得と地図表示 |

## Blazor版のローカル起動

### 前提環境

- Windows / PowerShell
- Java 21以上
- .NET 10 SDK
- Javaと.NET SDKをPATHから実行できること

Maven本体はリポジトリ内のMaven Wrapperが必要に応じて取得します。

### 外部サービス設定

Java APIは環境変数から設定を上書きできます。使用する機能に応じて設定してください。

| 環境変数 | 用途 |
| --- | --- |
| `SQLITE_DB_PATH` | SQLiteデータベースのパス |
| `LOCAL_PHOTO_BASE_PATH` | ローカル写真フォルダのルート |
| `GOOGLE_API_KEY` | Google APIキー |
| `GOOGLE_CLIENT_SECRET_PATH` | Google OAuthクライアントシークレットJSONのパス |
| `GOOGLE_DRIVE_FOLDER_ID` | Google Driveの対象フォルダID |
| `GOOGLE_CALENDAR_ID` | 対象のGoogle Calendar ID |
| `ANNICT_TOKEN` | Annict APIトークン |
| `NOMINATIM_BASE_URL` | Nominatim APIの接続先（省略時は公開API） |

シークレットやトークンはリポジトリへコミットせず、環境変数またはローカルの設定ファイルで管理してください。

### 起動

リポジトリのルートで次を実行します。

```powershell
.\start-web.ps1
```

APIが未起動の場合、スクリプトはJava APIをビルドして `http://localhost:9080` で起動します。APIの準備完了後、Blazor版を `http://localhost:5124` で起動します。

Mavenの依存関係はリポジトリ直下の `.m2-repository` に保存されます。スクリプトが起動したAPIは、終了時に自動停止します。

Blazor版のURLを変更する場合は、次のように指定します。

```powershell
.\start-web.ps1 -WebUrl http://localhost:6000
```

`-ApiUrl` は、既に起動しているAPIの確認先を変更するためのオプションです。Java API自体のポートを変更する場合は、APIのサーバー設定と `ScheduleViewer.Web/wwwroot/appsettings.json` の `ApiBaseUrl` を同じURLへ変更してください。

## テスト

Blazor版のテストは次のコマンドで実行できます。

```powershell
dotnet test .\ScheduleViewer.Web.Tests\ScheduleViewer.Web.Tests.csproj
```

Java APIのテストは次のコマンドで実行できます。

```powershell
.\schedule-viewer-api\mvnw.cmd -f .\schedule-viewer-api\pom.xml test
```

## 開発状況

個人利用を前提として開発中です。外部APIの認証情報、カレンダーID、ローカルファイルパスなどは利用環境に合わせて設定する必要があります。
