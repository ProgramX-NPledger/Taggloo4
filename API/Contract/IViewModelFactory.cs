namespace API.Contract;

/// <summary>
/// Creates or configures a View Model.
/// </summary>
/// <typeparam name="TViewModelType">The type of View Model to create or configure.</typeparam>
public interface IViewModelFactory<TViewModelType> : IPartialViewModelFactory<TViewModelType>
{
    /// <summary>
    /// Creates a View Model containing default values, including commly required data between calls, eg.
    /// lists for selection. Therefore, this tends to be used for GET requests.
    /// </summary>
    /// <returns>The created View Model.</returns>
    TViewModelType Create();
 
}