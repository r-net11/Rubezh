using System.Collections.Generic;
using System.Linq;
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
                errors.AddRange(DevicesValidator.DeviceErrors.Select(x => new ValidationErrorViewModel(x)));
                errors.AddRange(DevicesValidator.ZoneErrors.Select(x => new ValidationErrorViewModel(x)));

                InstructionValidator.Validate();
                errors.AddRange(InstructionValidator.InstructionErrors.Select(x => new ValidationErrorViewModel(x)));

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