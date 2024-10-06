namespace ScheduleViewer.WPF.Models;

/// <summary>
/// Model - 基底
/// </summary>
/// <typeparam name="TViewModel">ViewModel</typeparam>
public abstract class ModelBase<TViewModel> where TViewModel : class
{
    /// <summary> ViewModel </summary>
    internal abstract TViewModel ViewModel { get; set; }
}
