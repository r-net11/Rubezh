using System;
using System.ComponentModel;
using FiresecAPI.Models;

namespace FiresecService.Imitator
{
    public class DeviceStateViewModel : INotifyPropertyChanged
    {
        public DeviceStateViewModel(DriverState driverState, Action stateChanged)
        {
            StateChanged = stateChanged;
            DriverState = driverState;
        }

        Action StateChanged;
        public DriverState DriverState { get; private set; }

        public bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    OnPropertyChanged("IsActive");

                    if (StateChanged != null)
                        StateChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
