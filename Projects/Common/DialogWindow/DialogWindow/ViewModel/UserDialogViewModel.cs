using System.Windows;
using Infrastructure;
using Infrastructure.Common;

namespace DialogBox.ViewModel
{
    public class UserDialogViewModel : DialogContent
    {
        //public UserDialogViewModel(string message) :
        //    this(message, MessageBoxButton.OK, MessageBoxImage.None)
        //{ }

        //public UserDialogViewModel(string message, MessageBoxButton button) :
        //    this(message, button, MessageBoxImage.None)
        //{ }

        public UserDialogViewModel(string message, MessageBoxButton button, MessageBoxImage image)
        {
            Message = message;
            SetButtonsVisibility(button);
            SetImageVisibility(image);

            Initialize();
        }

        void Initialize()
        {
            Title = "Firesec";

            Result = MessageBoxResult.None;

            OkCommand = new RelayCommand(OnOk);
            CancelCommand = new RelayCommand(OnCancel);
            YesCommand = new RelayCommand(OnYes);
            NoCommand = new RelayCommand(OnNo);
        }

        void SetButtonsVisibility(MessageBoxButton button)
        {
            switch (button)
            {
                case MessageBoxButton.OK:
                    IsOkButtonVisible = true;
                    break;

                case MessageBoxButton.OKCancel:
                    IsOkButtonVisible = true;
                    IsCancelButtonVisible = true;
                    break;

                case MessageBoxButton.YesNo:
                    IsYesButtonVisible = true;
                    IsNoButtonVisible = true;
                    break;

                case MessageBoxButton.YesNoCancel:
                    IsYesButtonVisible = true;
                    IsNoButtonVisible = true;
                    IsCancelButtonVisible = true;
                    break;
            }
        }

        void SetImageVisibility(MessageBoxImage image)
        {
            switch (image)
            {
                case MessageBoxImage.Information:
                    IsInformationImageVisible = true;
                    break;

                case MessageBoxImage.None:
                    IsNoneImageVisible = true;
                    break;

                case MessageBoxImage.Question:
                    IsQuestionImageVisible = true;
                    break;

                case MessageBoxImage.Stop:
                    IsStopImageVisible = true;
                    break;

                case MessageBoxImage.Warning:
                    IsWarningImageVisible = true;
                    break;
            }
        }

        public bool IsOkButtonVisible { get; private set; }
        public bool IsCancelButtonVisible { get; private set; }
        public bool IsYesButtonVisible { get; private set; }
        public bool IsNoButtonVisible { get; private set; }

        public bool IsInformationImageVisible { get; private set; }
        public bool IsNoneImageVisible { get; private set; }
        public bool IsQuestionImageVisible { get; private set; }
        public bool IsStopImageVisible { get; private set; }
        public bool IsWarningImageVisible { get; private set; }

        public string Message { get; private set; }
        public MessageBoxResult Result { get; set; }

        public RelayCommand OkCommand { get; private set; }
        void OnOk()
        {
            Result = MessageBoxResult.OK;
            Close(true);
        }

        public RelayCommand CancelCommand { get; private set; }
        void OnCancel()
        {
            Result = MessageBoxResult.Cancel;
            Close(true);
        }

        public RelayCommand YesCommand { get; private set; }
        void OnYes()
        {
            Result = MessageBoxResult.Yes;
            Close(true);
        }

        public RelayCommand NoCommand { get; private set; }
        void OnNo()
        {
            Result = MessageBoxResult.No;
            Close(true);
        }
    }
}