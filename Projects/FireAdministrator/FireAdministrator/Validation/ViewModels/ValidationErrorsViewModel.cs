using System.Collections.Generic;
using System.Linq;
using FiresecClient;
using FiresecClient.Validation;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

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
			var errors = new List<ValidationErrorViewModel>();

			var devicesValidator = new DevicesValidator(FiresecManager.FiresecConfiguration);
			devicesValidator.Validate();
			errors.AddRange(devicesValidator.DeviceErrors.Select(x => new ValidationErrorViewModel(x)));
			errors.AddRange(devicesValidator.ZoneErrors.Select(x => new ValidationErrorViewModel(x)));
			errors.AddRange(devicesValidator.DirectionErrors.Select(x => new ValidationErrorViewModel(x)));

			InstructionValidator.Validate();
			errors.AddRange(InstructionValidator.InstructionErrors.Select(x => new ValidationErrorViewModel(x)));

			Errors = new List<ValidationErrorViewModel>(errors);
		}

		List<ValidationErrorViewModel> _errors;
		public List<ValidationErrorViewModel> Errors
		{
			get { return _errors; }
			set
			{
				_errors = value;
				OnPropertyChanged("Errors");
			}
		}

		ValidationErrorViewModel _selectedError;
		public ValidationErrorViewModel SelectedError
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
			get { return Errors.Any(x => x.ErrorLevel == ErrorLevel.CannotSave); }
		}

		public bool CannotWrite
		{
			get { return Errors.Any(x => x.ErrorLevel == ErrorLevel.CannotWrite); }
		}

		public RelayCommand ClickCommand { get; private set; }
		void OnClick()
		{
			if (SelectedError != null) SelectedError.Select();
		}
	}
}