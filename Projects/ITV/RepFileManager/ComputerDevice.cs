using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItvIntergation.Ngi;
using FiresecClient;
using FiresecAPI.Models;
using System.Diagnostics;

namespace RepFileManager
{
    public class ComputerDevice
    {
        public repositoryModuleDevice Device { get; private set; }

        public ComputerDevice()
        {
            Device = new repositoryModuleDevice();
            Device.id = "ComputerDevice";

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
            HashSet<repositoryModuleDeviceEvent> deviceEvents = new HashSet<repositoryModuleDeviceEvent>();

            foreach (var driver in Helper.CommonCommunicationDrivers)
            {
                foreach (var driverState in driver.States)
                {
                    if (driverState.Code.StartsWith("Reserved_"))
                        continue;

                    var deviceEvent = new repositoryModuleDeviceEvent();
                    deviceEvent.id = driverState.Code; // driverState.Name - на русском языке для локализации
                    deviceEvents.Add(deviceEvent);
                }
            }

            Device.events = deviceEvents.ToArray();

            Device.events.ToList().ForEach((x) => { Trace.WriteLine(x.id); });
        }
    }
}
