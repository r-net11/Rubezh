using System.Collections.Generic;
using System.Linq;
using StrazhAPI.Enums;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Validation;
using Infrastructure.Common.Windows.ViewModels;

namespace FireAdministrator.ViewModels
{
	public class ValidationErrorsViewModel : BaseViewModel, IValidationResult
	{
		public ValidationErrorsViewModel()
		{
			ClickCommand = new RelayCommand(OnClick);
			CloseValidationCommand = new RelayCommand(OnCloseValidation);
			Validate();
		}

		public void Validate()
		{
			Errors = new List<IValidationError>();
		}

		List<IValidationError> _errors;
		public List<IValidationError> Errors
		{
			get { return _errors; }
			set
			{
				_errors = value;
				OnPropertyChanged(() => Errors);
			}
		}

		IValidationError _selectedError;
		public IValidationError SelectedError
		{
			get { return _selectedError; }
			set
			{
				_selectedError = value;
				OnPropertyChanged(() => SelectedError);
			}
		}

		IEnumerable<IValidationError> SortedErrors(ModuleType? module)
		{
			if (module.HasValue)
				return Errors.Where(x => x.Module == module.Value);
			return Errors;
		}

		public bool HasErrors(ModuleType? module = null)
		{
			return SortedErrors(module).Count() > 0;
		}
		public bool CannotSave(ModuleType? module = null)
		{
			return SortedErrors(module).Any(x => x.ErrorLevel == ValidationErrorLevel.CannotSave);
		}
		public bool CannotWrite(ModuleType? module = null)
		{
			return SortedErrors(module).Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite);
		}

		public RelayCommand ClickCommand { get; private set; }
		void OnClick()
		{
			if (SelectedError != null)
				SelectedError.Navigate();
		}

		public RelayCommand CloseValidationCommand { get; private set; }
		void OnCloseValidation()
		{
			ServiceFactory.Layout.ShowFooter(null);
		}

		public void AddErrors(IEnumerable<IValidationError> validationErrors)
		{
			Errors.AddRange(validationErrors);
			OnPropertyChanged(() => Errors);
		}
	}
}