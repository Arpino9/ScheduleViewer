namespace ScheduleViewer.WPF.ViewModels;

/// <summary>
/// ViewModel - 基底
/// </summary>
/// <typeparam name="T">Model</typeparam>
/// <remarks>
/// メモリリーク防止のため、必ずPropertyChangedを入れる。
/// </remarks>
public abstract class ViewModelBase<T> : INotifyPropertyChanged where T : class
{
    /// <summary> Viewとの橋渡しプロパティ </summary>
    /// <remarks> アクセス修飾子をpublicにしないと、正しく通知されない </remarks>
    public abstract event PropertyChangedEventHandler PropertyChanged;
    
    /// <summary> Model </summary>
    protected abstract T Model { get; }

    /// <summary>
    /// イベント登録
    /// </summary>
    /// <remarks>
    /// Viewの指定したイベントと、発火させるメソッドを紐付ける。
    /// Subscribe()メソッドのオーバーロードが正しく呼ばれないので、
    /// 名前空間かglobal usingに「using System;」を必ず入れること。
    /// </remarks>
    protected abstract void BindEvents();
}