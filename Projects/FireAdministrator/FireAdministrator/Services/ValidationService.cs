using FireAdministrator.ViewModels;
using Infrastructure;

namespace FireAdministrator
{
	public class ValidationService : IValidationService
	{
		public IValidationResult Validate()
		{
			ServiceFactory.Layout.ShowFooter(null);
			var validationErrorsViewModel = new ValidationErrorsViewModel();
			if (validationErrorsViewModel.HasErrors)
			{
				ServiceFactory.Layout.ShowFooter(validationErrorsViewModel);
			}
			return validationErrorsViewModel;
		}
	}
}