using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;

namespace Firesec.Imitator.ViewModels
{
    public class DeviceViewModel : BaseViewModel
    {
        public DeviceViewModel(DeviceState deviceState)
        {
            DeviceState = deviceState;
            StateType = DeviceState.StateType;
            Name = DeviceState.Device.Driver.ShortName + " - " + DeviceState.Device.DottedAddress;
            Level = DeviceState.Device.AllParents.Count;
            ImageSource = DeviceState.Device.Driver.ImageSource;

            DriverStates = new List<DeviceStateViewModel>();
            foreach (var driverState in from x in DeviceState.Device.Driver.States orderby x.StateType select x)
            {
                if (!string.IsNullOrEmpty(driverState.Name))
                {
                    var deviceStateViewModel = new DeviceStateViewModel(driverState);
                    DriverStates.Add(deviceStateViewModel);
                }
            }

            foreach (var deviceDriverState in deviceState.States)
            {
                var state = DriverStates.FirstOrDefault(x => x.DriverState.Code == deviceDriverState.Code);
                state._isActive = true;
            }
        }

        public DeviceState DeviceState { get; private set; }
        public string Name { get; private set; }
        public StateType StateType { get; private set; }
        public List<DeviceStateViewModel> DriverStates { get; private set; }
        public int Level { get; private set; }
        public string ImageSource { get; private set; }
    }
}