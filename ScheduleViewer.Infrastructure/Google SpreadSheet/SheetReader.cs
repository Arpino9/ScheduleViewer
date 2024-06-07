namespace ScheduleViewer.Infrastructure.Google_SpreadSheet;

/// <summary>
/// Google Calendar 読込
/// </summary>
public static class SheetReader
{
    /// <summary> 初期化 </summary>
    public static SheetsService Initializer => GoogleService<SheetsService>.Initialize_OAuth(
                                               initializer => new SheetsService(initializer),
                                               SheetsService.Scope.Spreadsheets,
                                               "token_Sheets");

    /// <summary>
    /// 読込
    /// </summary>
    /// <param name="sheetId">シートID</param>
    /// <param name="sheetRange">シート範囲</param>
    public static IList<IList<object>> ReadOAuth(string sheetId, string sheetRange)
    {
        var request = Initializer.Spreadsheets.Values.Get(sheetId, sheetRange);

        return request.Execute().Values;
    }
}
