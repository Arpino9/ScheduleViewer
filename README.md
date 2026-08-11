# プロジェクト名
ScheduleViewer (C#)

## 概要
Google カレンダーをベースに、Fitbitなどの各種活動データを日別表示するアプリケーションです(作成中)。<br>
同期する外部APIは以下の通りです。<br>

[Google Service]<br>
Google Cloud ConsoleのAPIから各種Googleサービスの情報を取得します。<br>
取得する情報は以下の通りです。<br>
※Google Photoは2025年4月の仕様変更により無課金枠では実質使用不可なので除外。

・Google Books<br>
・Google Calendar<br>
・Google Drive<br>
・Google Fitness<br>
・Google SpreadSheet<br>
・Google Tasks

[Annict]<br>
GraphQL APIを用いて視聴済みのアニメ情報を取得します。<br>
取得するデータには、Googleカレンダーに別途登録が必要です。

[Fitbit]<br>
FitbitのAPIから日次の睡眠・活動データを取得します。  OAuth2認証を利用しています。

## 使用技術
- 言語: C#<br>
- フレームワーク: .NET 9.0<br>
- 環境: Visual Studio 2022, Windows 10

## 機能
- Googleカレンダーの予定、タスク表示<br>
- 読了した本の日別表示
- 睡眠データの取得
- 視聴済みアニメの確認

## セットアップ(簡易的な設定ができるように修正中)
1. Fitbit Developerでアプリを登録してください。
2. クライアントIDとシークレットをScheduleViewer.configに設定してください。

## Blazor版のローカル起動

前提として、Java 21以上と.NET 10 SDKをPATHから実行できるようにしてください。Maven本体はリポジトリ内のラッパーが必要に応じて取得します。

PowerShellでリポジトリのルートから次を実行します。

```powershell
.\start-web.ps1
```

スクリプトはAPIが未起動の場合にJava APIをビルドして起動し、準備完了後にBlazor版を `http://localhost:5124` で起動します。Mavenの依存関係はリポジトリ直下の `.m2-repository` に保存されます。スクリプトが起動したAPIは、終了時に自動停止します。

Blazor版のURLを変更する場合は次のように指定できます。

```powershell
.\start-web.ps1 -WebUrl http://localhost:6000
```

`-ApiUrl` は既に起動しているAPIの確認先を変えるための指定です。API自体のポートを変更する場合は、Java APIのサーバー設定と `ScheduleViewer.Web/wwwroot/appsettings.json` の `ApiBaseUrl` も同じURLへ変更してください。
