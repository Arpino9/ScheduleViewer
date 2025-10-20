# プロジェクト名
ScheduleViewer (C#)

## 概要
Google カレンダーをベースに、Fitbitなどの各種活動データを日別表示するアプリケーションです(作成中)。
同期する外部APIは以下の通りです。

[Google Service]
Google Cloud ConsoleのAPIから各種Googleサービスの情報を取得します。
取得する情報は以下の通りです。
※Google Photoは2025年4月の仕様変更により無課金枠では実質使用不可なので除外。

・Google Books
・Google Calendar
・Google Drive
・Google Fitness
・Google SpreadSheet
・Google Tasks

[Annict]
GraphQL APIを用いて視聴済みのアニメ情報を取得します。
取得するデータには、Googleカレンダーに別途登録が必要です。

[Fitbit]
FitbitのAPIから日次の睡眠・活動データを取得します。  OAuth2認証を利用しています。

## 使用技術
- 言語: C#
- フレームワーク: .NET Framework 4.8
- 環境: Visual Studio 2022, Windows 10

## 機能
- Googleカレンダーの予定、タスク表示
- 読了した本の日別表示
- 睡眠データの取得
- 視聴済みアニメの確認

## セットアップ(簡易的な設定ができるように修正中)
1. Fitbit Developerでアプリを登録してください。
2. クライアントIDとシークレットをScheduleViewer.configに設定してください。
