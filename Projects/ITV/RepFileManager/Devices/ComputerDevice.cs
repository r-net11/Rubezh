using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using ItvIntergation.Ngi;

namespace RepFileManager
{
    public class ComputerDevice
    {
        public repositoryModuleDevice Device { get; private set; }

        public ComputerDevice()
        {
            Device = new repositoryModuleDevice()
            {
                id = "ComputerDevice",
                root = true
            };

            CreateChildren();
            CreateStates();
            CreateEvents();
            CreateButtons();
            CreateProperties();
        }

        void CreateChildren()
        {
            var children = new List<repositoryModuleDeviceChild>();
            var childDevice = new repositoryModuleDeviceChild()
            {
                id = "CommunicationDevice"
            };
            children.Add(childDevice);
            Device.childs = children.ToArray();
        }

        void CreateStates()
        {
            var deviceStates = new List<repositoryModuleDeviceState>();
            foreach (var stateType in Helper.AllStates)
            {
                var deviceState = new repositoryModuleDeviceState()
                {
                    id = stateType.ToString(),
                    image = null
                };
                deviceStates.Add(deviceState);
            }
            Device.states = deviceStates.ToArray();
        }

        void CreateEvents()
        {
            var driver = FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.Computer);

            var deviceEvents = new List<repositoryModuleDeviceEvent>();
            foreach (var driverState in driver.States)
            {
                if (driverState.Code.StartsWith("Reserved_"))
                    continue;

                var deviceEvent = new repositoryModuleDeviceEvent()
                {
                    id = driverState.Code
                };
                deviceEvents.Add(deviceEvent);
            }

            Device.events = deviceEvents.ToArray();
        }

        void CreateButtons()
        {
            var buttons = new List<repositoryModuleDeviceButton>();
            var button = new repositoryModuleDeviceButton()
            {
                id = "LoadConfiguration",
                Value = null
            };
            buttons.Add(button);
            Device.buttons = buttons.ToArray();
        }

        void CreateProperties()
        {
            var driver = FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.Computer);

            var allProperties = Helper.CreateProperties(driver.Properties);
            if (allProperties.Count > 0)
            {
                Device.properties = allProperties.ToArray();
            }
        }
    }
}
