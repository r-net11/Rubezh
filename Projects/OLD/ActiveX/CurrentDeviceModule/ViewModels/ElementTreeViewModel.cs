using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using Infrastructure.Common;
using System.Collections.ObjectModel;

namespace CurrentDeviceModule.ViewModels
{
    public class ElementTreeViewModel : TreeBaseViewModel<ElementTreeViewModel>
    {
        public ElementTreeViewModel()
        {

        }

        public void Initialize(Device device, ObservableCollection<ElementTreeViewModel> sourceDevices)
        {
            Source = sourceDevices;
            Device = device;
        }

        public Device Device { get; private set; }

        public Guid DeviceId
        {
            get { return Device.UID; }
        }

        public string DriverShortName
        {
            get { return Device.Driver.ShortName; }
        }

        public string DeviceDescription
        {
            get { return Device.Description; }
        }

        public string DeviceAddress
        {
            get { return Device.PresentationAddress; }
        }

    }
}
