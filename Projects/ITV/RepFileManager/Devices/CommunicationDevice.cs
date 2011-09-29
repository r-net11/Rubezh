using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using ItvIntergation.Ngi;

namespace RepFileManager
{
    public class CommunicationDevice
    {
        public repositoryModuleDevice Device { get; private set; }

        public CommunicationDevice()
        {
            Device = new repositoryModuleDevice()
            {
                id = "CommunicationDevice"
            };

            CreateChildren();
            CreateStates();
            CreateEvents();
            CreateProperties();
        }

        void CreateChildren()
        {
            var children = new List<repositoryModuleDeviceChild>();
            var childDevice = new repositoryModuleDeviceChild()
            {
                id = "PanelDevice"
            };
            children.Add(childDevice);
            Device.childs = children.ToArray();
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
                deviceEvent.id = state;
                deviceEvents.Add(deviceEvent);
            }
            Device.events = deviceEvents.ToArray();
        }

        void CreateProperties()
        {
            var driverProperties = new List<DriverProperty>();
            foreach (var driver in Helper.CommunicationDrivers)
            {
                foreach (var driverProperty in driver.Properties)
                {
                    if (driverProperty.Visible)
                    {
                        if (driverProperties.Any(x => x.Name == driverProperty.Name) == false)
                            driverProperties.Add(driverProperty);
                    }
                }
            }

            var allProperties = Helper.CreateProperties(driverProperties);
            var customProperties = CreateCustomProperties();
            allProperties.AddRange(customProperties);

            Device.properties = allProperties.ToArray();
        }

        List<object> CreateCustomProperties()
        {
            var stringEnumProperties = new List<PropertyStringEnumType>();

            var property = new PropertyStringEnumType()
            {
                id = "MsType"
            };
            var propertyValues = new List<PropertyStringEnumTypeValue>();

            foreach (var driver in Helper.CommunicationDrivers)
            {
                var propertyValue = new PropertyStringEnumTypeValue()
                {
                    Value = driver.DriverType.ToString()
                };
                propertyValues.Add(propertyValue);
            }

            property.value = propertyValues.ToArray();
            stringEnumProperties.Add(property);

            var channelProperties1 = Helper.CreateProperties(FiresecManager.Drivers.FirstOrDefault(x=>x.DriverType == DriverType.USB_Channel_1).Properties);
            ((PropertyStringEnumType)channelProperties1[0]).id = "CnannelOneAddress";

            var channelProperties2 = Helper.CreateProperties(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.USB_Channel_1).Properties);
            ((PropertyStringEnumType)channelProperties2[0]).id = "CnannelTwoAddress";

            var properties = new List<object>();
            properties.AddRange(stringEnumProperties);
            properties.AddRange(channelProperties1);
            properties.AddRange(channelProperties2);
            return properties;
        }
    }
}
