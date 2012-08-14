using System.Collections.Generic;
using System.Linq;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Validation;

namespace FireAdministrator.ViewModels
{
	public class ValidationErrorsViewModel : BaseViewModel, IValidationResult
	{
		public ValidationErrorsViewModel()
		{
			ClickCommand = new RelayCommand(OnClick);
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
				OnPropertyChanged("Errors");
			}
		}

		IValidationError _selectedError;
		public IValidationError SelectedError
		{
			get { return _selectedError; }
			set
			{
				_selectedError = value;
				OnPropertyChanged("SelectedError");
			}
		}

		public bool HasErrors
		{
			get { return Errors.Count > 0; }
		}
		public bool CannotSave
		{
			get { return Errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotSave); }
		}
		public bool CannotWrite
		{
			get { return Errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite); }
		}

		public RelayCommand ClickCommand { get; private set; }
		void OnClick()
		{
			if (SelectedError != null) 
				SelectedError.Navigate();
		}

		public void AddErrors(IEnumerable<IValidationError> validationErrors)
		{
			Errors.AddRange(validationErrors);
			OnPropertyChanged("Errors");
		}
	}
}