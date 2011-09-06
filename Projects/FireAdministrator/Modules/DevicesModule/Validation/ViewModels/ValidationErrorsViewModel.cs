using System.Collections.Generic;
using FiresecClient.Validation;
using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class ValidationErrorsViewModel : BaseViewModel
    {
        public ValidationErrorsViewModel()
        {
            ClickCommand = new RelayCommand(OnClick);
        }

        public List<ValidationErrorViewModel> Errors
        {
            get
            {
                var errors = new List<ValidationErrorViewModel>();

                DevicesValidator.Validate();

                foreach (var deviceValidationError in DevicesValidator.DeviceErrors)
                {
                    var validationErrorViewModel = new ValidationErrorViewModel(deviceValidationError);
                    errors.Add(validationErrorViewModel);
                }

                foreach (var zoneValidationError in DevicesValidator.ZoneErrors)
                {
                    var validationErrorViewModel = new ValidationErrorViewModel(zoneValidationError);
                    errors.Add(validationErrorViewModel);
                }

                InstructionValidator.Validate();

                foreach (var instructionError in InstructionValidator.InstructionErrors)
                {
                    var validationErrorViewModel = new ValidationErrorViewModel(instructionError);
                    errors.Add(validationErrorViewModel);
                }

                return errors;
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

        public RelayCommand ClickCommand { get; private set; }
        void OnClick()
        {
            if (SelectedError != null)
            {
                SelectedError.Select();
            }
        }
    }
}