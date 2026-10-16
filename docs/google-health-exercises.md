# Google Health 運動記録

健康タブの「運動記録」は `GET /api/fitbit/exercises?date=yyyy-MM-dd` から取得する。
既存の `/activity` は日次集計として維持する。

## 公式仕様（2026-10-17確認）

- [dataPoints.list](https://developers.google.com/health/reference/rest/v4/users.dataTypes.dataPoints/list):
  `GET https://health.googleapis.com/v4/users/me/dataTypes/exercise/dataPoints`。
  `pageSize=25`（exerciseの上限）と `nextPageToken` / `pageToken` を使用し、全ページ取得後に開始順に並べる。
  `dataSourceFamily=users/me/dataSourceFamilies/all-sources` により手動記録も取得対象とする。
- [Exercise / SessionTimeInterval](https://developers.google.com/health/reference/rest/v4/users.dataTypes.dataPoints#Exercise):
  `exerciseType`, `displayName`, `interval.startTime`, `interval.endTime`, `activeDuration`, `notes` を読む。
  `notes` は手動登録時の自由記述。Razorで文字列としてエスケープし、CSSの `white-space: pre-wrap` で改行を保持する。
  `activeDuration` は休止を除いた秒数（小数可、末尾s）。存在するときは0秒も含め優先し、ない場合だけ終了－開始を使う。画面で算出元を示す。
- [Filters / scopes](https://developers.google.com/health/filters):
  exerciseのセッション検索には `exercise.interval.civil_start_time` の `>=` / `<` を使用する。
  必要スコープ `https://www.googleapis.com/auth/googlehealth.activity_and_fitness.readonly` は既存の読み取り認証に含まれるため、追加スコープはない。

## 日本時間の日付と日またぎ

表示対象は **Asia/Tokyoでの開始日が選択日と一致するセッション**。
日をまたぐ運動も開始日に1件だけ表示し、終了日は翌日の日付、時間は運動全体を表示する（分割しない）。
記録元のcivil時刻は日本時間とは限らないため、検索範囲を選択日の2日前から2日後の0時未満まで広げ、
返却された必須の物理時刻startTimeをAsia/Tokyoへ変換して厳密に判定する。
終了時刻も同じ時間帯で表示する。サーバーやブラウザのローカル時間帯には依存しない。

## 認証・再取得

正常な空配列は記録なし、401は未認証・認証期限、403は権限不足、それ以外は取得失敗として表示する。
取得失敗や権限不足の際に部分取得した一覧を記録なしとして表示しない。
各状態から「再取得」できる。既存の歩数などの取得に失敗しても、運動記録は独立して表示できる。

既存の認証済みアカウントでは通常は再認証不要。401/403の場合は次を行う。

1. 設定 → 認証管理 → Google Health → 「認証する」または「再認証」。
2. 「認証ページを開く」からGoogleアカウントにログインし、活動とフィットネスの読み取りを許可する。
3. 認証管理で「状態を更新」し、健康タブの「運動記録」で「再取得」。

再認証は既存の `POST /api/auth/google/fitbit?force=true` を使用する。
インポート用の書き込み認証とは別のトークンを使用する。API本文、Notes、トークンをログやエラー応答に含めない。

## 実アカウントでの確認手順（未実施）

対象年の4/11にGoogle Healthで手動登録した筋力トレーニングを用意し、日時・運動時間と、
Notesのマシン名・重量・回数・改行が健康タブに表示されることを確認する。
同日の複数記録、記録のない日も確認し、追加や編集後は「再取得」を押す。
実アカウントに対する読み取りやテスト記録の登録は今回実行していない。

自動テストでは公式レスポンス形のデータを使い、Notes、HTMLのエスケープ、複数件、ページング、空データ、
401/403/取得失敗、日本時間の0時境界、異なる記録元オフセット、日またぎ、実活動時間と経過時間を検証する。

## 実行済み検証

- Java 21 / Maven 3.9.9: `mvn test`（domain / infrastructure / api）、39件成功。
- `.NET 10`: `dotnet test ScheduleViewer.Web.Tests`（WebAssemblyビルド含む）、19件成功。
- `git diff --check` 成功。
- 最新masterに含まれていた `BeginLogout` の文字列としての改行を実改行へ修正し、Webの構文エラーを解消した。
- 実アカウントのAPI応答、ブラウザでのOAuth同意、4/11のテスト記録の実表示は未確認。
