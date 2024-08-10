namespace API.Contract;

/// <summary>
/// Configuration of View Models.
/// </summary>
/// <typeparam name="TViewModelType">Type of View Model.</typeparam>
/// <remarks>
/// It would not ordinarily be required for a Controller to invoke an implementation of this interface, instead, the
/// implementation of <seealso cref="IViewModelFactory{TViewModelType}"/> should call this to configure the concrete
/// View Model class.
/// </remarks>
public interface IPartialViewModelFactory<TViewModelType>
{
       
    /// <summary>
    /// The configuration of View Models allows for the retention of transient form values from the user and
    /// inclusion of commonly required data between calls, eg. lists for selection. Therefore, this tends to
    /// be used for POST and PUT requests.
    /// </summary>
    /// <param name="viewModel">The view model to configure, which will likely have been passed from the
    /// Controller containing transient form data..</param>
    /// <returns>The ocnfigured View Model.</returns>
    void Configure(ref TViewModelType viewModel);
}