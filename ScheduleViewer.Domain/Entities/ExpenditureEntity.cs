namespace ScheduleViewer.Domain.Entities;

/// <summary>
/// Entity - 家計簿
/// </summary>
public sealed class ExpenditureEntity
{
    public ExpenditureEntity(string[] record)
    {
        var i = default(int);

        this.CanCalc               = (record[0] == "1")? "はい": "いいえ";

        this.Date                  = DateTime.Parse(AdjustArray(false));
        this.ItemName              = AdjustArray(false);
        this.Price                 = long.Parse(AdjustArray(false));
        this.FinancialInstitutions = AdjustArray(false);
        this.Category_Large        = AdjustArray(false);
        this.Category_Middle       = AdjustArray(false);
        this.Memo                  = AdjustArray(true);
        this.Change                = (AdjustArray(false) == "1") ? "はい" : "いいえ";
        this.ID                    = AdjustArray(false);

        string AdjustArray(bool isMemo)
        {
            i++;

            if (isMemo) 
            {
                return record[i];
            }

            start:

            if (string.IsNullOrEmpty(record[i]) ||
                record[i] == "\"")
            {
                i++;
                goto start;
            }

            return record[i];
        }
    }

    /// <summary> ID </summary>
    public string ID { get; set; }

    /// <summary> 計算対象 </summary>
    public string CanCalc { get; set; }
    
    /// <summary> 日付 </summary>
    public DateTime Date { get; set; }

    /// <summary> 内容 </summary>
    public string ItemName { get; set; }

    /// <summary> 金額(円) </summary>
    public long Price { get; set; }

    /// <summary> 保有金融機関 </summary>
    public string FinancialInstitutions { get; set; }

    /// <summary> 大項目 </summary>
    public string Category_Large { get; set; }

    /// <summary> 中項目 </summary>
    public string Category_Middle { get; set;}

    /// <summary> メモ </summary>
    public string Memo { get; set; }

    /// <summary> 振替 </summary>
    public string Change { get; set; }
}
