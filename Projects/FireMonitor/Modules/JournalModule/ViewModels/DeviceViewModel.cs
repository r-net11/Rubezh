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
            DatabaseId = device.DatabaseId;
            ImageSource = device.Driver.ImageSource;
            Name = device.Driver.ShortName;
            Address = device.DottedAddress;
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

        public string DatabaseId { get; private set; }
        public string ImageSource { get; private set; }
        public string Name { get; private set; }
        public string Address { get; private set; }
    }
}