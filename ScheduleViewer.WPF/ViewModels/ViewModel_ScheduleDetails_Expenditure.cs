﻿namespace ScheduleViewer.WPF.ViewModels;

/// <summary>
/// ViewModel - スケジュール詳細 (支出一覧)
/// </summary>
public sealed class ViewModel_ScheduleDetails_Expenditure : ViewModelBase<Model_ScheduleDetails_Expenditure>
{
    public override event PropertyChangedEventHandler PropertyChanged;

    public ViewModel_ScheduleDetails_Expenditure()
    {
        this.Model.ViewModel = this;
        this.Model.Initialize();

        this.BindEvents();
    }

    /// <summary> Model - スケジュール詳細 (支出一覧) </summary>
    protected override Model_ScheduleDetails_Expenditure Model 
        => Model_ScheduleDetails_Expenditure.GetInstance();

    protected override void BindEvents()
    {
        this.Expenditures_SelectionChanged.Subscribe(this.Model.ListView_SelectionChanged);
    }

    /// <summary> 一覧 - ItemSource </summary>
    public ReactiveCollection<ExpenditureEntity> Expenditures_ItemSource { get; set; } = new ReactiveCollection<ExpenditureEntity>();

    /// <summary> 一覧 - SelectedIndex </summary>
    public ReactiveProperty<int> Expenditures_SelectedIndex { get; set; } = new ReactiveProperty<int>();

    /// <summary> 一覧 - SelectionChanged </summary>
    public ReactiveCommand Expenditures_SelectionChanged { get; private set; } = new ReactiveCommand();

    /// <summary> 計算対象 </summary>
    public ReactiveProperty<string> CanCalc { get; set; } = new ReactiveProperty<string>();

    /// <summary> 内容 </summary>
    public ReactiveProperty<string> ItemName { get; set; } = new ReactiveProperty<string>();

    /// <summary> 金額 </summary>
    public ReactiveProperty<int> Price { get; set; } = new ReactiveProperty<int>();

    /// <summary> 保有金融機関 </summary>
    public ReactiveProperty<string> FinancialInstitutions { get; set; } = new ReactiveProperty<string>();

    /// <summary> 大項目 </summary>
    public ReactiveProperty<string> Category_Large { get; set; } = new ReactiveProperty<string>();

    /// <summary> 中項目 </summary>
    public ReactiveProperty<string> Category_Middle { get; set; } = new ReactiveProperty<string>();

    /// <summary> メモ </summary>
    public ReactiveProperty<string> Memo { get; set; } = new ReactiveProperty<string>();

    /// <summary> 振替 </summary>
    public ReactiveProperty<string> Change { get; set; } = new ReactiveProperty<string>();

    /// <summary> ID </summary>
    public ReactiveProperty<string> ID { get; set; } = new ReactiveProperty<string>();

}
