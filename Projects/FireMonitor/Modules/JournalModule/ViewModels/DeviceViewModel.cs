using FiresecAPI.Models;
using Infrastructure.Common;

namespace JournalModule.ViewModels
{
    public class DeviceViewModel : BaseViewModel
    {
        public DeviceViewModel(Device device)
        {
            Device = device;
            IsChecked = true;
        }

        public Device Device { get; private set; }

        bool _isChecked;
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                OnPropertyChanged("IsChecked");
            }
        }

        public string DatabaseId { get { return Device.DatabaseId; } }
    }
}