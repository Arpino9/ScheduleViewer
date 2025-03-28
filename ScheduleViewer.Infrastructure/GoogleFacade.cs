﻿using Google.Apis.PhotosLibrary.v1.Data;
using ScheduleViewer.Infrastructure.Fitbit;
using ScheduleViewer.Infrastructure.Google_Books;

namespace ScheduleViewer.Infrastructure;

/// <summary>
/// Google Facade
/// </summary>
/// <remarks>
/// Googleサービスの窓口クラス。必要に応じてガード節を入れる。
/// </remarks>
public static class GoogleFacade
{

    #region Books

    /// <summary>
    /// Googleブックス
    /// </summary>
    public static class Books
    {
        /// <summary> 読込 </summary>
        private static BooksReader _reader = new BooksReader();

        /// <summary>
        /// タイトル検索
        /// </summary>
        /// <param name="title">タイトル</param>
        public static void FindByTitle(string title)
            => _reader.FindByTitle(title);
    }

    #endregion

    #region Calendar

    /// <summary>
    /// Googleカレンダー
    /// </summary>
    public static class Calendar
    {
        /// <summary> 読込 </summary>
        private static CalendarReader _reader = new CalendarReader();

        /// <summary>
        /// 初期化
        /// </summary>
        /// <returns></returns>
        public static async Task InitializeAsync()
            => _reader.InitializeAsync();

        /// <summary>
        /// カレンダーが読み込み中か
        /// </summary>
        /// <returns>
        /// True  : 読込中
        /// False : 読込済
        /// </returns>
        public static bool IsLoading()
            => _reader.Loading is null ||
               _reader.Loading.Value;

        /// <summary>
        /// イベントを取得する
        /// </summary>
        /// <param name="title">タイトル</param>
        /// <param name="startDate">開始日付</param>
        /// <returns>イベント</returns>
        /// <remarks>
        /// 指定されたタイトル、開始日と一致するイベントを取得する。
        /// </remarks>
        public static IReadOnlyList<CalendarEventsEntity> FindByTitle(string title, DateOnly startDate)
            => _reader.FindByTitle(title, startDate);

        /// <summary>
        /// イベントを取得する
        /// </summary>
        /// <param name="address">住所</param>
        /// <returns>イベント</returns>
        /// <remarks>
        /// 指定された住所と一致するイベントを抽出する。
        /// </remarks>
        public static IReadOnlyList<CalendarEventsEntity> FindByAddress(string address)
            => _reader.FindByAddress(address);

        /// <summary>
        /// イベントを取得する
        /// </summary>
        /// <param name="date">開始日付</param>
        /// <returns>イベント</returns>
        /// <remarks>
        /// 指定された日付と一致するイベントを取得する。
        /// </remarks>
        public static IReadOnlyList<CalendarEventsEntity> FindByDate(DateOnly date)
            => _reader.FindByDate(date);

        /// <summary>
        /// イベントを取得する
        /// </summary>
        /// <param name="startDate">開始日付</param>
        /// <param name="endDate">終了日付</param>
        /// <returns>イベント</returns>
        /// <remarks>
        /// 指定された開始日、終了日と一致するイベントを取得する。
        /// </remarks>
        public static IReadOnlyList<CalendarEventsEntity> FindByDate(DateOnly startDate, DateOnly endDate)
            => _reader.FindByDate(startDate, endDate);

        /// <summary>
        /// イベントを取得する
        /// </summary>
        /// <param name="startDate">開始日付</param>
        /// <param name="endDate">終了日付</param>
        /// <returns>イベント</returns>
        /// <remarks>
        /// 指定された開始日、終了日と一致するイベントを取得する。
        /// </remarks>
        public static IReadOnlyList<CalendarEventsEntity> FindByDate(DateOnly startDate, DateOnly endDate, TimeSpan startTime)
            => _reader.FindByDate(startDate, endDate, startTime);

        /// <summary>
        /// イベントを取得する
        /// </summary>
        /// <param name="address">住所</param>
        /// <param name="startDate">開始日付</param>
        /// <returns>イベント</returns>
        /// <remarks>
        /// 指定された住所、開始日と一致するイベントを取得する。
        /// </remarks>
        public static IReadOnlyList<CalendarEventsEntity> FindByAddress(string address, DateOnly startDate)
            => _reader.FindByAddress(address, startDate);

        /// <summary>
        /// イベントを取得する
        /// </summary>
        /// <param name="address">住所</param>
        /// <param name="startDate">開始日付</param>
        /// <param name="endDate">終了日付</param>
        /// <returns>イベント</returns>
        /// <remarks>
        /// 指定された住所、開始日、終了日と一致するイベントを取得する。
        /// </remarks>
        public static IReadOnlyList<CalendarEventsEntity> FindByAddress(string address, DateOnly startDate, DateOnly endDate)
            => _reader.FindByAddress(address, startDate, endDate);

        /// <summary>
        /// イベントを取得する
        /// </summary>
        /// <param name="title">タイトル</param>
        /// <param name="startDate">開始日付</param>
        /// <param name="endDate">終了日付</param>
        /// <returns>イベント</returns>
        /// <remarks>
        /// 指定されたタイトル、開始日、終了日と一致するイベントを取得する。
        /// </remarks>
        public static IReadOnlyList<CalendarEventsEntity> FindByTitle(string title, DateOnly startDate, DateOnly endDate)
            => _reader.FindByTitle(title, startDate, endDate);

        /// <summary>
        /// イベントを取得する
        /// </summary>
        /// <param name="address">住所</param>
        /// <param name="startTime">開始時刻</param>
        /// <param name="endTime">終了時刻</param>
        /// <returns>イベント</returns>
        /// <remarks>
        /// 指定された住所、開始時刻、終了時刻と一致するイベントを取得する。
        /// </remarks>
        public static IReadOnlyList<CalendarEventsEntity> FindByAddress(string address, TimeSpan startTime, TimeSpan endTime)
            => _reader.FindByAddress(address, startTime, endTime);

        /// <summary>
        /// イベントを取得する
        /// </summary>
        /// <param name="address">住所</param>
        /// <param name="startDate">開始日付</param>
        /// <param name="endDate">終了日付</param>
        /// <param name="startTime">開始時刻</param>
        /// <returns>イベント</returns>
        /// <remarks>
        /// 指定された住所、開始日時、終了日と一致するイベントを取得する。
        /// </remarks>
        public static IReadOnlyList<CalendarEventsEntity> FindByAddress(string address, DateOnly startDate, DateOnly endDate, TimeSpan startTime)
            => _reader.FindByAddress(address, startDate, endDate, startTime);

        /// <summary>
        /// イベントを取得する
        /// </summary>
        /// <param name="address">住所</param>
        /// <param name="startDate">開始日付</param>
        /// <param name="endDate">終了日付</param>
        /// <param name="startTime">開始時刻</param>
        /// <param name="endTime">終了時刻</param>
        /// <returns>イベント</returns>
        /// <remarks>
        /// 指定された住所、開始日時、終了日時と一致するイベントを取得する。
        /// </remarks>
        public static IReadOnlyList<CalendarEventsEntity> FindByAddress(string address, DateOnly startDate, DateOnly endDate, TimeSpan startTime, TimeSpan endTime)
            => _reader.FindByAddress(address, startDate, endDate, startTime, endTime);
    }

    #endregion

    #region Drive

    /// <summary>
    /// Googleドライブ
    /// </summary>
    public static class Drive
    {
        /// <summary> 読込 </summary>
        private static DriveReader _reader = new DriveReader();

        /// <summary>
        /// 初期化
        /// </summary>
        /// <returns></returns>
        public static async Task InitializeAsync()
            => _reader.InitializeAsync();

        /// <summary>
        /// 指定された日の支出を取得
        /// </summary>
        /// <param name="date">日付</param>
        /// <returns>支出</returns>
        public static List<ExpenditureEntity> GetExpenditure(DateOnly date)
            => _reader.GetExpenditure(date);
    }

    #endregion

    #region Fitbit

    /// <summary>
    /// Fitbit
    /// </summary>
    public static class Fitbit
    {
        /// <summary> 読込 </summary>
        private static readonly FitbitReader _reader = new FitbitReader();

        /// <summary> 読込 </summary>
        private static Fitbit_ProfileEntity _profile;

        /// <summary>
        /// 初期化
        /// </summary>
        /// <returns>Taskオブジェクト</returns>
        public static async Task InitializeAsync()
        {
            await _reader.Initialize();

            _profile = await _reader.GetProfileAsync();

            var sleep1 = await _reader.GetSleepAsync(DateOnly.FromDateTime(DateTime.Today));
        }

        /// <summary>
        /// プロフィールを取得
        /// </summary>
        /// <returns>プロフィール</returns>
        public static async Task ReadProfileAsync()
            => await _reader.GetProfileAsync();

        /// <summary>
        /// 睡眠時間を取得
        /// </summary>
        /// <param name="date">日付</param>
        /// <returns>睡眠時間</returns>
        public static async Task<Fitbit_SleepEntity> ReadSleepAsync(DateOnly date)
            => await _reader.GetSleepAsync(date);

        /// <summary>
        /// 活動時間を取得
        /// </summary>
        /// <param name="date">日付</param>
        /// <returns>活動時間</returns>
        public static async Task<Fitbit_ActivityEntity> ReadActivityAsync(DateOnly date)
            => await _reader.GetActivityAsync(date);

        /// <summary>
        /// 心拍数を取得
        /// </summary>
        /// <param name="date">日付</param>
        /// <returns>心拍数</returns>
        public static async Task<Fitbit_HeartEntity> ReadHeartAsync(DateOnly date)
            => await _reader.GetHeartAsync(date);

        /// <summary>
        /// 体重を取得
        /// </summary>
        /// <param name="date">日付</param>
        /// <returns>体重</returns>
        public static async Task<Fitbit_WeightEntity> ReadWeightAsync(DateOnly date)
            => await _reader.GetWeightAsync(date);
    }

    #endregion

    #region Fitness

    /// <summary>
    /// Google Fit
    /// </summary>
    public static class Fitness
    {
        /// <summary> 読込 - 活動記録 </summary>
        private static readonly FitnessReader_Activity _readerActivity = new FitnessReader_Activity();
        
        /// <summary>
        /// 指定された期間の活動ポイントを取得する
        /// </summary>
        /// <param name="startTime">開始日</param>
        /// <param name="endTime">終了日</param>
        public static async Task ReadActivity(DateTimeOffset startTime, DateTimeOffset endTime)
            => _readerActivity.ReadActivity(startTime, endTime);

        /// <summary>
        /// 日付で検索
        /// </summary>
        /// <param name="date">日付</param>
        /// <returns>活動記録</returns>
        public static List<ActivityEntity> FindActivitiesByDate(DateOnly date)
            => _readerActivity.FindActivitiesByDate(date);

        /// <summary>
        /// 日付で検索
        /// </summary>
        /// <param name="date">日付</param>
        /// <returns>歩数</returns>
        public static List<int> FindStepsByDate(DateOnly date)
            => _readerActivity.FindStepsByDate(date);

        /// <summary>
        /// 指定された期間の歩数を取得する
        /// </summary>
        /// <param name="startTime">開始日</param>
        /// <param name="endTime">終了日</param>
        public static async Task ReadSteps(DateTimeOffset startTime, DateTimeOffset endTime)
            => _readerActivity.ReadSteps(startTime, endTime);

        /// <summary>
        /// 指定された期間のFitness記録を取得する
        /// </summary>
        /// <param name="startTime">開始日</param>
        /// <param name="endTime">終了日</param>
        /// <remarks>
        /// セグメントごとにリスト化されて返ってくる模様。
        /// </remarks>
        public static async void ReadFitnessDataAsync(DateTime startTime, DateTime endTime)
            => _readerActivity.ReadFitnessDataAsync(startTime, endTime);

        /// <summary> 読込 - 睡眠記録 </summary>
        private static readonly FitnessReader_Sleep _readerSleep = new FitnessReader_Sleep();

        /// <summary>
        /// 指定された期間の睡眠時間を取得する
        /// </summary>
        /// <param name="startTime">開始日</param>
        /// <param name="endTime">終了日</param>
        public static async Task ReadSleepTime(DateTimeOffset startTime, DateTimeOffset endTime)
            => _readerSleep.ReadSleepTime(startTime, endTime);

        /// <summary>
        /// 日付で検索
        /// </summary>
        /// <param name="date">日付</param>
        /// <returns>睡眠時間</returns>
        public static List<int> FindSleepTimeByDate(DateOnly date)
            => _readerSleep.FindSleepTimeByDate(date);

        /// <summary> 読込 </summary>
        private static readonly FitnessWriter _writer = new FitnessWriter();

        /// <summary>
        /// 指定された期間に活動ポイントを書き込む
        /// </summary>
        /// <param name="startTime">開始日</param>
        /// <param name="endTime">終了日</param>
        public static void WriteActivity(DateTimeOffset startTime, DateTimeOffset endTime)
             => _writer.WriteDataSource(startTime, endTime);
    }

    #endregion

    #region Place

    /// <summary>
    /// Google Place
    /// </summary>
    public static class Place
    {
        /// <summary> 読込 </summary>
        private static readonly PlaceReader _reader = new PlaceReader();

        /// <summary>
        /// 住所から地点情報(緯度、経度)を取得する
        /// </summary>
        /// <param name="address">住所</param>
        /// <returns>地点情報(緯度、経度)</returns>
        public static (double? Latitude, double? Longitude) ReadLocation(string address)
            => _reader.ReadPlaceLocation(address);
    }

    #endregion

    #region Photo

    /// <summary>
    /// Googleフォト
    /// </summary>
    public static class Photo
    {
        /// <summary> 読込 </summary>
        private static readonly PhotoReader _reader = new PhotoReader();

        /// <summary>
        /// 初期化
        /// </summary>
        /// <returns></returns>
        public static async Task InitializeAsync()
            => _reader.InitializeAsync();

        /// <summary>
        /// 日付で検索
        /// </summary>
        /// <param name="date">日付</param>
        /// <returns>写真データ</returns>
        /// <remarks>
        /// 写真が登録されていれば、日付と一致する写真を取り出す。
        /// </remarks>
        public static List<PhotoEntity> FindByDate(DateOnly date)
            => _reader.FindPhotosByDate(date);
    }

    #endregion

    #region SpreadSheet

    /// <summary>
    /// スプレッドシート
    /// </summary>
    public static class SpreadSheet
    {
        /// <summary> 読込 </summary>
        private static readonly SheetReader _reader = new SheetReader();

        /// <summary>
        /// タスクシートを読み出す
        /// </summary>
        /// <returns>Taskオブジェクト</returns>
        public static IList<IList<object>> Initialize_Tasks()
            => _reader.ReadOAuth("1tc5uFTh09PBVVnV2OYmGZ3svY6C-6SwCAF6KIUO8l9c", "タスク一覧!A:B");
    }

    #endregion

    #region Tasks

    /// <summary>
    /// Googleタスク
    /// </summary>
    public static class Tasks
    {
        /// <summary> 読込 </summary>
        private static readonly TaskReader _reader = new TaskReader();

        /// <summary>
        /// 初期化
        /// </summary>
        /// <returns>Taskオブジェクト</returns>
        public static async Task InitializeAsync()
            => _reader.InitializeAsync();

        /// <summary>
        /// 対象日から検索
        /// </summary>
        /// <param name="date">対象日</param>
        /// <returns>タスク</returns>
        public static IReadOnlyList<TaskEntity> FindByDate(DateOnly date)
            => _reader.FindTasksByDate(date);
    }

    #endregion

}
