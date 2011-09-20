using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;

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

                Children.ForEach(x => x.IsSelected = value);
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
                var name = "";
                if (Device.Driver.HasAddress)
                    name = Device.PresentationAddress + " - ";
                name += Device.Driver.Name;
                return name;
            }
        }

        public List<AutoDetectedDeviceViewModel> Children { get; set; }
    }
}
