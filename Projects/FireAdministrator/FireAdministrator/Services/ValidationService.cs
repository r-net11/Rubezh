using FireAdministrator.ViewModels;
using Infrastructure;

namespace FireAdministrator
{
	public class ValidationService : IValidationService
	{
		public IValidationResult Validate()
		{
			ServiceFactory.Layout.ShowValidationArea(null);
			var validationErrorsViewModel = new ValidationErrorsViewModel();
			if (validationErrorsViewModel.HasErrors)
			{
				ServiceFactory.Layout.ShowValidationArea(validationErrorsViewModel);
			}
			return validationErrorsViewModel;
		}
	}
}