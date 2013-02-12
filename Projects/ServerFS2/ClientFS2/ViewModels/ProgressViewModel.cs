using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using ServerFS2;

namespace ClientFS2.ViewModels
{
    public class ProgressViewModel : DialogViewModel
    {
        public ProgressViewModel(string title)
        {
            Title = title;
            CancelText = "Отменить";
            CloseOnEscape = false;
            AllowClose = false;
            StopCommand = new RelayCommand(OnStop);
            ServerHelper.Progress -= Progress;
            ServerHelper.Progress += Progress;
        }

        public void CloseProgress()
        {
            ServerHelper.Progress -= Progress;
            Close(true);
        }

        public void Progress(int value, int maxValue = 100, string description = "")
        {
            ApplicationService.Invoke(() => OnProgress(value, maxValue,description));
        }

        void OnProgress(int value, int maxValue, string description)
        {
            Description = description;
            MaxValue = maxValue;
            Value = value;
        }

        int _value;
        public int Value
        {
            get { return _value; }
            set
            {
                _value = value;
                OnPropertyChanged("Value");
            }
        }

        int _maxValue = 100;
        public int MaxValue
        {
            get { return _maxValue; }
            set
            {
                _maxValue = value;
                OnPropertyChanged("MaxValue");
            }
        }

        string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged("Description");
            }
        }

        string _cancelText;
        public string CancelText
        {
            get { return _cancelText; }
            set
            {
                _cancelText = value;
                OnPropertyChanged("CancelText");
            }
        }

        public RelayCommand StopCommand { get; private set; }
        void OnStop()
        {
            //FiresecManager.FSAgent.CancelProgress();
        }
    }
}
