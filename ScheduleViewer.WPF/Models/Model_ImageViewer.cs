namespace ScheduleViewer.WPF.Models;

/// <summary>
/// Model - イメージビューワー
/// </summary>
public sealed class Model_ImageViewer : ModelBase<ViewModel_ImageViewer>
{
    #region Get Instance

    private static Model_ImageViewer model = null;

    public static Model_ImageViewer GetInstance()
    {
        if (model == null)
        {
            model = new Model_ImageViewer();
        }

        return model;
    }

    #endregion

    /// <summary> ViewModel - イメージビューワー  </summary>
    internal override ViewModel_ImageViewer ViewModel { get; set; }
}
