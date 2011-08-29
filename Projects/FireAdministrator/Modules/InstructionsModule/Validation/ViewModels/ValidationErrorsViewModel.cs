using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using FiresecClient.Validation;

namespace InstructionsModule.Validation.ViewModels
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
                InstructionValidator.Validate();
                foreach (var instructionError in InstructionValidator.InstructionErrors)
                {
                    errors.Add(new ValidationErrorViewModel(instructionError));
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
