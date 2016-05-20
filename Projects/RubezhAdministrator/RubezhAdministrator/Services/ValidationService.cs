using FireAdministrator.ViewModels;
using RubezhClient;
using GKProcessor;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Validation;
using Infrastructure.Common.Windows;

namespace FireAdministrator
{
	public class ValidationService : IValidationService
	{
		public IValidationResult Validate()
		{
			using (new WaitWrapper())
			{
				ServiceFactory.Layout.ShowFooter(null);
				var validationErrorsViewModel = new ValidationErrorsViewModel();
				foreach (var module in ApplicationService.Modules)
				{
					var validationModule = module as IValidationModule;
					if (validationModule != null)
						validationErrorsViewModel.AddErrors(validationModule.Validate());
				}
				if (validationErrorsViewModel.HasErrors())
					ServiceFactory.Layout.ShowFooter(validationErrorsViewModel);
				return validationErrorsViewModel;
			}
		}
	}
}