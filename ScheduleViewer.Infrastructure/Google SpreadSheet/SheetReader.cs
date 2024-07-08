namespace ScheduleViewer.Infrastructure.Google_SpreadSheet;

/// <summary>
/// Google SpreadSheet 読込
/// </summary>
internal class SheetReader : GoogleServiceBase<SheetsService>
{
    /// <summary> 
    /// 初期化子 
    /// </summary>
    protected override SheetsService Initializer
    {
        get => base.Initialize_OAuth(initializer => new SheetsService(initializer),
                                     new[] { SheetsService.Scope.Spreadsheets },
                                     "token_Sheets");
    } 

    /// <summary>
    /// 読込
    /// </summary>
    /// <param name="sheetId">シートID</param>
    /// <param name="sheetRange">シート範囲</param>
    internal IList<IList<object>> ReadOAuth(string sheetId, string sheetRange)
    {
        var request = Initializer.Spreadsheets.Values.Get(sheetId, sheetRange);

        return request.Execute().Values;
    }
}
