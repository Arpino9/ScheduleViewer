using ScheduleViewer.Infrastructure;

namespace ScheduleViewer.WPF.Models;

/// <summary>
/// Model - スケジュール詳細 (支出)
/// </summary>
public sealed class Model_ScheduleDetails_Expenditure : ModelBase<ViewModel_ScheduleDetails_Expenditure>, IViewer
{
    #region Get Instance

    private static Model_ScheduleDetails_Expenditure model = null;

    internal static Model_ScheduleDetails_Expenditure GetInstance()
    {
        if (model == null)
        {
            model = new Model_ScheduleDetails_Expenditure();
        }

        return model;
    }

    #endregion

    public void Initialize()
    {
        this.Reload(GoogleFacade.Drive.GetExpenditure(this.ViewModel_Header.Date.Value));

        this.ViewModel.Expenditures_SelectedIndex.Value = 0;

        this.ListView_SelectionChanged();
    }

    /// <summary>
    /// リロード
    /// </summary>
    /// <param name="expenditure">支出</param>
    private void Reload(IList<ExpenditureEntity> expenditures)
    {
        this.ViewModel.Expenditures_ItemSource.Clear();

        foreach (var expenditure in expenditures)
        {
            this.ViewModel.Expenditures_ItemSource.Add(expenditure);
        }
    }

    public void ListView_SelectionChanged()
    {
        if (this.ViewModel.Expenditures_ItemSource.IsEmpty())
        {
            // リストが空
            return;
        }

        if (this.ViewModel.Expenditures_SelectedIndex.Value.IsUnSelected())
        {
            // 未選択
            return;
        }

        var entity = this.ViewModel.Expenditures_ItemSource[this.ViewModel.Expenditures_SelectedIndex.Value];

        // 計算対象
        this.ViewModel.CanCalc.Value = entity.CanCalc;

        // 内容
        this.ViewModel.ItemName.Value = entity.ItemName;

        // 金額
        this.ViewModel.Price.Value = entity.Price;

        // 保有金融機関
        this.ViewModel.FinancialInstitutions.Value = entity.FinancialInstitutions;

        // 大項目
        this.ViewModel.Category_Large.Value = entity.Category_Large;
        
        // 中項目
        this.ViewModel.Category_Middle.Value = entity.Category_Middle;
        
        // メモ
        this.ViewModel.Memo.Value = entity.Memo;
        
        // 振替
        this.ViewModel.Change.Value = entity.Change;
        
        // ID
        this.ViewModel.ID.Value = entity.ID;
    }

    public void Clear_ViewForm()
    {
        // 計算対象
        this.ViewModel.CanCalc.Value = string.Empty;

        // 内容
        this.ViewModel.ItemName.Value = string.Empty;

        // 金額
        this.ViewModel.Price.Value = 0;
        
        // 保有金融機関
        this.ViewModel.FinancialInstitutions.Value = string.Empty;

        // 大項目
        this.ViewModel.Category_Large.Value = string.Empty;
        
        // 中項目
        this.ViewModel.Category_Middle.Value = string.Empty;
        
        // メモ
        this.ViewModel.Memo.Value = string.Empty;
        
        // 振替
        this.ViewModel.Change.Value = string.Empty;
        
        // ID
        this.ViewModel.ID.Value = string.Empty;
    }

    /// <summary>
    /// 大項目別にソート
    /// </summary>
    internal void SortByCategory_Large()
    {
        if (ListUtils.IsSortedAscending(this.ViewModel.Expenditures_ItemSource, x => x.Category_Large))
        {
            // 昇順 → 降順
            this.Reload(this.ViewModel.Expenditures_ItemSource.OrderByDescending(x => x.Category_Large).ToList());
        }
        else
        {
            // 降順 → 昇順
            this.Reload(this.ViewModel.Expenditures_ItemSource.OrderBy(x => x.Category_Large).ToList());
        }
    }

    /// <summary>
    /// 中項目別にソート
    /// </summary>
    internal void SortByCategory_Middle()
    {
        if (ListUtils.IsSortedAscending(this.ViewModel.Expenditures_ItemSource, x => x.Category_Middle))
        {
            // 昇順 → 降順
            this.Reload(this.ViewModel.Expenditures_ItemSource.OrderByDescending(x => x.Category_Middle).ToList());
        }
        else
        {
            // 降順 → 昇順
            this.Reload(this.ViewModel.Expenditures_ItemSource.OrderBy(x => x.Category_Middle).ToList());
        }
    }

    /// <summary>
    /// 内容別にソート
    /// </summary>
    internal void SortByItemName()
    {
        if (ListUtils.IsSortedAscending(this.ViewModel.Expenditures_ItemSource, x => x.Category_Middle))
        {
            // 昇順 → 降順
            this.Reload(this.ViewModel.Expenditures_ItemSource.OrderByDescending(x => x.ItemName).ToList());
        }
        else
        {
            // 降順 → 昇順
            this.Reload(this.ViewModel.Expenditures_ItemSource.OrderBy(x => x.ItemName).ToList());
        }
    }

    /// <summary>
    /// 金額別にソート
    /// </summary>
    internal void SortByPrice()
    {
        if (ListUtils.IsSortedAscending(this.ViewModel.Expenditures_ItemSource, x => x.Price))
        {
            // 昇順 → 降順
            this.Reload(this.ViewModel.Expenditures_ItemSource.OrderByDescending(x => x.Price).ToList());
        }
        else
        {
            // 降順 → 昇順
            this.Reload(this.ViewModel.Expenditures_ItemSource.OrderBy(x => x.Price).ToList());
        }
    }

    /// <summary> ViewModel - スケジュール詳細 (本一覧) </summary>
    public ViewModel_ScheduleDetails ViewModel_Header { get; set; }

    internal override ViewModel_ScheduleDetails_Expenditure ViewModel { get; set; }

}
