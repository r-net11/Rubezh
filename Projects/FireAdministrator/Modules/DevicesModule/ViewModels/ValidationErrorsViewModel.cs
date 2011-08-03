using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using FiresecClient;
using System.Windows;
using FiresecClient.Validation;

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
                List<ValidationErrorViewModel> errors = new List<ValidationErrorViewModel>();

                Validator.Validate();

                foreach (var deviceValidationError in Validator.DeviceErrors)
                {
                    ValidationErrorViewModel validationErrorViewModel = new ValidationErrorViewModel(deviceValidationError);
                    errors.Add(validationErrorViewModel);
                }

                foreach (var zoneValidationError in Validator.ZoneErrors)
                {
                    ValidationErrorViewModel validationErrorViewModel = new ValidationErrorViewModel(zoneValidationError);
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
