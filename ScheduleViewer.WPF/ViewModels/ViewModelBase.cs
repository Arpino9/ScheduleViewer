namespace ScheduleViewer.WPF.ViewModels;

/// <summary>
/// ViewModel - 基底
/// </summary>
/// <remarks>
/// メモリリーク防止のため、必ずPropertyChangedを入れる。
/// </remarks>
public abstract class ViewModelBase : INotifyPropertyChanged
{
    public abstract event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// イベント登録
    /// </summary>
    /// <remarks>
    /// Viewの指定したイベントと、発火させるメソッドを紐付ける。
    /// Subscribe()メソッドのオーバーロードが正しく呼ばれないので、
    /// 名前空間に「using System;」を必ず入れること。
    /// </remarks>
    protected abstract void BindEvents();
}