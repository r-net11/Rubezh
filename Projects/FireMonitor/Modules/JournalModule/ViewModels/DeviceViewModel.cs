using FiresecAPI.Models;
using Infrastructure.Common;

namespace JournalModule.ViewModels
{
    public class DeviceViewModel : BaseViewModel
    {
        Device _device;

        public DeviceViewModel(Device device)
        {
            _device = device;
            IsChecked = true;
        }

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

        public string DatabaseId { get { return _device.DatabaseId; } }
        public string ImageSource { get { return _device.Driver.ImageSource; } }
        public string Name { get { return _device.Driver.ShortName; } }
        public string Address { get { return _device.DottedAddress; } }
    }
}