using FiresecAPI.Models;
using Infrastructure.Common;
using System.Collections.Generic;
using FiresecClient;
using System.Linq;

namespace DevicesModule.ViewModels
{
    public class AutoDetectedDeviceViewModel : BaseViewModel
    {
        public Device Device { get; private set; }

        public AutoDetectedDeviceViewModel(Device device)
        {
            Device = device;
            Children = new List<AutoDetectedDeviceViewModel>();
        }

        bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }

        public bool CanSelect
        {
            get
            {
                return !FiresecManager.DeviceConfiguration.Devices.Any(x => x.Id == Device.Id);
            }
        }

        public string Name
        {
            get
            {
                return Device.PresentationAddress + Device.Driver.ShortName;
            }
        }

        public List<AutoDetectedDeviceViewModel> Children { get; set; }
    }
}
