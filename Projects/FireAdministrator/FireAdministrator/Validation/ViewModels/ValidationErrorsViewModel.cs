using System.Collections.Generic;
using System.Linq;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Validation;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;

namespace FireAdministrator.ViewModels
{
	public class ValidationErrorsViewModel : BaseViewModel, IValidationResult
	{
		public ValidationErrorsViewModel()
		{
			ClickCommand = new RelayCommand(OnClick);
			EditValidationCommand = new RelayCommand(OnEditValidation);
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

		IEnumerable<IValidationError> SortedErrors(string module)
		{
			if (module != null)
				return Errors.Where(x => x.Module == module);
			return Errors;
		}

		public bool HasErrors(string module = null)
		{
			return SortedErrors(module).Count() > 0;
		}
		public bool CannotSave(string module = null)
		{
			return SortedErrors(module).Any(x => x.ErrorLevel == ValidationErrorLevel.CannotSave);
		}
		public bool CannotWrite(string module = null)
		{
			return SortedErrors(module).Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite);
		}

		public RelayCommand ClickCommand { get; private set; }
		void OnClick()
		{
			if (SelectedError != null)
				SelectedError.Navigate();
		}

		public RelayCommand EditValidationCommand { get; private set; }
		void OnEditValidation()
		{
			ServiceFactory.Events.GetEvent<EditValidationEvent>().Publish(null);
		}

		public void AddErrors(IEnumerable<IValidationError> validationErrors)
		{
			Errors.AddRange(validationErrors);
			OnPropertyChanged(() => Errors);
		}
	}
}