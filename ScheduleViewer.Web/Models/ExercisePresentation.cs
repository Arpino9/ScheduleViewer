namespace ScheduleViewer.Web.Models;

public static class ExercisePresentation
{
    public static string[] NoteLines(string? notes)
        => System.Text.RegularExpressions.Regex.Replace(notes ?? "", @"<br\s*/?>", "\n",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase)
            .Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
    public static string FormatExerciseDuration(double seconds)
        => $"{Math.Floor(seconds / 60):0}分 {seconds % 60:0.#########}秒";
    public static string TokyoTime(DateTimeOffset value)
        => value.ToOffset(TimeSpan.FromHours(9)).ToString("HH:mm");

    public static string ExerciseLabel(ExerciseRecord item)
        => System.Text.RegularExpressions.Regex.IsMatch(item.DisplayName ?? "", @"[\p{IsHiragana}\p{IsKatakana}\p{IsCJKUnifiedIdeographs}]")
            ? item.DisplayName ?? "その他の運動" : item.ExerciseType switch
            {
                "WALKING" => "ウォーキング", "RUNNING" => "ランニング",
                "STRENGTH_TRAINING" => "筋力トレーニング", "FUNCTIONAL_STRENGTH_TRAINING" => "ファンクショナル筋力トレーニング",
                "WEIGHT_MACHINES" => "ウエイトマシン", "WEIGHTLIFTING" => "ウエイトリフティング",
                "WEIGHTS" => "ウエイト", "FREE_WEIGHTS" => "フリーウエイト", "BODY_WEIGHT" => "自重トレーニング",
                "BIKING" or "OUTDOOR_BIKE" => "サイクリング", "STATIONARY_BIKE" => "エアロバイク",
                "SWIMMING" or "SWIMMING_POOL" => "水泳", "YOGA" => "ヨガ", "STRETCHING" => "ストレッチ",
                "PILATES" => "ピラティス", "HIKING" => "ハイキング", "TREADMILL" => "トレッドミル",
                "OTHER" => "その他", _ => "その他の運動"
            };

    public static string ExerciseIcon(string type) => type switch
    {
        "WALKING" or "NORDIC_WALKING" or "POWER_WALKING" => "🚶",
        "RUNNING" or "TRAIL_RUN" or "TREADMILL" => "🏃",
        "BIKING" or "OUTDOOR_BIKE" or "STATIONARY_BIKE" or "MOUNTAIN_BIKE" => "🚴",
        "SWIMMING" or "SWIMMING_POOL" or "SWIMMING_OPEN_WATER" => "🏊",
        "STRENGTH_TRAINING" or "FUNCTIONAL_STRENGTH_TRAINING" or "BODY_WEIGHT" => "💪",
        "WEIGHT_MACHINES" or "WEIGHTLIFTING" or "WEIGHTS" or "FREE_WEIGHTS" => "🏋️",
        "YOGA" or "PILATES" or "STRETCHING" => "🧘",
        "HIKING" => "🥾", _ => "🏅"
    };
}
