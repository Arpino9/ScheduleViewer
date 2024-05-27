namespace ScheduleViewer.WPF.Models;

/// <summary>
/// Model - 基底
/// </summary>
/// <typeparam name="T">ViewModel</typeparam>
public abstract class ModelBase<T> where T : class
{
    /// <summary> ViewModel </summary>
    internal abstract T ViewModel { get; set; }
}
