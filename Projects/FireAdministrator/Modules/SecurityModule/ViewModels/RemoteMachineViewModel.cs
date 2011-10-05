using Infrastructure.Common;

namespace SecurityModule.ViewModels
{
    public class RemoteMachineViewModel : DialogContent
    {
        public RemoteMachineViewModel()
        {
            Title = "Удаленный компьютер";
            IsDnsName = true;

            SaveCommand = new RelayCommand(OnSave);
            CancelCommand = new RelayCommand(OnCancel);
        }

        string _hostNameOrAddress;
        public string HostNameOrAddress
        {
            get { return _hostNameOrAddress; }
            set
            {
                _hostNameOrAddress = value;
                OnPropertyChanged("HostNameOrAddress");
            }
        }

        bool _isIpAddress;
        public bool IsIpAddress
        {
            get { return _isIpAddress; }
            set
            {
                _isIpAddress = value;
                if (_isIpAddress)
                {
                    HostNameOrAddress = string.Empty;
                    IsDnsName = false;
                }

                OnPropertyChanged("IsIpAddress");
            }
        }

        bool _isDnsName;
        public bool IsDnsName
        {
            get { return _isDnsName; }
            set
            {
                _isDnsName = value;
                if (_isDnsName)
                {
                    HostNameOrAddress = string.Empty;
                    IsIpAddress = false;
                }

                OnPropertyChanged("IsDnsName");
            }
        }

        public RelayCommand SaveCommand { get; private set; }
        void OnSave()
        {
            Close(true);
        }

        public RelayCommand CancelCommand { get; private set; }
        void OnCancel()
        {
            Close(false);
        }
    }
}