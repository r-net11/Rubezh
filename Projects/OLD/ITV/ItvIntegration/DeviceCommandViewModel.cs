using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using FiresecClient.Itv;

namespace ItvIntegration
{
    public class DeviceCommandViewModel : BaseViewModel
    {
        DriverProperty DriverProperty;
        Device Device;

        public DeviceCommandViewModel(Device device, DriverProperty driverProperty)
        {
            ExecuteCommand = new RelayCommand(OnExecute);
            Device = device;
            DriverProperty = driverProperty;
        }

        public string ConmmandName
        {
            get { return DriverProperty.Caption; }
        }

        public RelayCommand ExecuteCommand { get; private set; }
        void OnExecute()
        {
            ItvManager.ExecuteCommand(Device.UID, DriverProperty.Name);
        }
    }
}