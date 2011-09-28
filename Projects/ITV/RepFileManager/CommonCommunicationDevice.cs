using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItvIntergation.Ngi;
using System.Diagnostics;
using FiresecClient;
using FiresecAPI.Models;

namespace RepFileManager
{
    public class CommonCommunicationDevice
    {
        public repositoryModuleDevice Device { get; private set; }

        public CommonCommunicationDevice()
        {
            Device = new repositoryModuleDevice();
            Device.id = "CommonCommunicationDevice";

            CreateStates();
            CreateEvents();
        }

        void CreateStates()
        {
            var driver = FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.Rubezh_2AM);

            List<repositoryModuleDeviceState> deviceStates = new List<repositoryModuleDeviceState>();
            foreach (var stateType in Helper.AllStates)
            {
                var deviceState = new repositoryModuleDeviceState();
                deviceState.id = stateType.ToString();
                deviceState.image = driver.DriverType.ToString() + "." + stateType.ToString() + ".bmp";
                deviceStates.Add(deviceState);
            }
            Device.states = deviceStates.ToArray();
        }

        void CreateEvents()
        {
            var states = new HashSet<string>();
            foreach (var driver in Helper.CommonCommunicationDrivers)
            {
                foreach (var driverState in driver.States)
                {
                    if (driverState.Code.StartsWith("Reserved_"))
                        continue;

                    states.Add(driverState.Code);
                }
            }

            var deviceEvents = new List<repositoryModuleDeviceEvent>();
            foreach (var state in states)
            {
                var deviceEvent = new repositoryModuleDeviceEvent();
                deviceEvent.id = state; // driverState.Name - на русском языке для локализации
                deviceEvents.Add(deviceEvent);
            }
            Device.events = deviceEvents.ToArray();
        }
    }
}
