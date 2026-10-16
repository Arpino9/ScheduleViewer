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
  読み取りには既存の `https://www.googleapis.com/auth/googlehealth.activity_and_fitness.readonly` を使用する。追加・編集のため、同じ認証に `https://www.googleapis.com/auth/googlehealth.activity_and_fitness.writeonly` を追加した。

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

閲覧のための再認証は不要。追加・編集を使う際は書き込みスコープへの同意が必要なため、既存アカウントも一度再認証する。401/403の場合も次を行う。

1. 設定 → 認証管理 → Google Health → 「認証する」または「再認証」。
2. 「認証ページを開く」からGoogleアカウントにログインし、活動とフィットネスの読み取りを許可する。
3. 認証管理で「状態を更新」し、健康タブの「運動記録」で「再取得」。

再認証は既存の `POST /api/auth/google/fitbit?force=true` を使用する。
追加・編集は閲覧と同じトークンを使用し、別アカウントへの誤登録を防ぐ。インポート用のトークンとは分離する。API本文、Notes、トークンをログやエラー応答に含めない。

## 実アカウントでの確認手順（未実施）

対象年の4/11にGoogle Healthで手動登録した筋力トレーニングを用意し、日時・運動時間と、
Notesのマシン名・重量・回数・改行が健康タブに表示されることを確認する。
同日の複数記録、記録のない日も確認し、追加や編集後は「再取得」を押す。
実アカウントに対する読み取りやテスト記録の登録は今回実行していない。

自動テストでは公式レスポンス形のデータを使い、Notes、HTMLのエスケープ、複数件、ページング、空データ、
401/403/取得失敗、日本時間の0時境界、異なる記録元オフセット、日またぎ、実活動時間と経過時間を検証する。

## 閲覧機能の初回実行済み検証

- Java 21 / Maven 3.9.9: `mvn test`（domain / infrastructure / api）、39件成功。
- `.NET 10`: `dotnet test ScheduleViewer.Web.Tests`（WebAssemblyビルド含む）、19件成功。
- `git diff --check` 成功。
- 最新masterに含まれていた `BeginLogout` の文字列としての改行を実改行へ修正し、Webの構文エラーを解消した。
- 実アカウントのAPI応答、ブラウザでのOAuth同意、4/11のテスト記録の実表示は未確認。

## 追加・編集

「再取得」の右側に「追加」「編集」を配置する。モーダルで運動種別、日本時間の開始・終了日時、
Notes（改行可、10000文字まで）を入力する。終了は開始より後でなければ保存できない。
運動種別は2026-10-17の公式ExerciseType一覧を使用し、主要な種別には日本語表示名を用意した。

- `GET /api/fitbit/exercises/types`: 運動種別の選択肢。
- `POST /api/fitbit/exercises`: Google Healthの [create](https://developers.google.com/health/reference/rest/v4/users.dataTypes.dataPoints/create) を呼ぶ。
  リソースIDはサーバー生成に任せ、取得一覧の `name` を編集対象のIDとして保持する。
- `PATCH /api/fitbit/exercises`: Google Healthの [patch](https://developers.google.com/health/reference/rest/v4/users.dataTypes.dataPoints/patch) を呼ぶ。
  既存データをGETして所有者を確認し、Notes・種別・日時を更新する。メトリクスなど入力対象外のフィールドを保持する。
  このAPIにupdateMaskは定義されていないため送信しない。

**編集できるのは、同じOAuthクライアントで作成した手動記録だけ**。
[所有者による更新制限](https://developers.google.com/health/data-management#timestamp_mutability_and_owner_updates) により、
Google Healthアプリで手動入力した記録や、他のアプリ・デバイスが作成した記録をScheduleViewerから更新することはできない。
リストの `dataSource.application.googleWebClientId`、`platform`、`recordingMethod` と現在のクライアントIDから
`canEdit` を判定する。サーバー側でも保存直前にGETし直して検査し、UIからのフラグは信用しない。
編集モーダルでこの制限を説明し、編集可能な記録だけを選択肢に表示する。

新規登録と日時変更時は `activeDuration` に終了－開始をセットする（休止区間のない手動入力）。
日時を変えないNotes・種別の更新では既存の実活動時間を維持する。
インポート済み記録に区間・イベントがある場合、その情報を失わないよう日時の変更を409で拒否する。
元のアプリでの編集を案内する。Notes・種別の更新では区間情報を保持する。

書き込みには読み取りと同じ認証の `activity_and_fitness.writeonly` を使用する。
権限エラー時はモーダルから「書き込み権限を再認証」→「Google認証ページを開く」で同意できる。
OAuth同意画面の設定でスコープを制限している場合は、Google Cloud側でもこのスコープを許可する。
認証完了後に保存を押す。権限を拒否しても、従来の読み取り機能は維持される。

create/patchは [Operation](https://developers.google.com/health/reference/rest/Shared.Types/Operation) を返す。
`done: true` かつ正常なresponseを確認して保存成功とし、Operation.errorはHTTP200でも失敗とする。
`done: false` は受付中として案内し、完了とは表示しない。未定義のポーリングAPIは使用せず、一覧から再取得できる。
タイムアウトや通信切断時は結果不明と案内し、モーダルでの再送を止める。
POSTを自動再送するとサーバー生成IDが増えて重複するため、まず一覧を再取得して反映を確認する。
401で更新トークンを使う場合だけ、既存方式に従って1回再試行する。

保存後は健康データと運動一覧を再取得する。開始日を別日に変えた記録は、移動先の日付で表示される。
Notes・トークン・APIの生のエラー本文はログや応答に含めない。JSONの入力エラーも本文を含まない応答にする。

閲覧機能はユーザーが実表示を確認済み。今回の追加・編集については実アカウントへの書き込みとOAuth同意は未実施。
実確認では再認証後、テスト記録を「追加」し、再取得で表示を確認してから「編集」でNotesや日時を変えて確認する。
今回もマージ・デプロイは行わない。

追加・編集の最終検証: Java 21 / `mvn test` 全モジュール47件成功、
`dotnet test ScheduleViewer.Web.Tests --no-restore` はWebAssemblyビルドを含め25件成功。
作成・更新JSON、所有権・ID検証、日またぎ、Notes、時間の維持・再計算、入力エラー、
Operationの成功・受付中・失敗、APIルートとモーダル表示を自動テストで検証した。
