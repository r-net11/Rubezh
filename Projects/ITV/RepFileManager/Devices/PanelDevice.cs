using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using ItvIntergation.Ngi;

namespace RepFileManager
{
    public class PanelDevice
    {
        public repositoryModuleDevice Device { get; private set; }

        public PanelDevice()
        {
            Device = new repositoryModuleDevice()
            {
                id = "PanelDevice"
            };

            CreateChildren();
            CreateStates();
            CreateImages();
            CreateEvents();
            CreateProperties();
        }

        void CreateChildren()
        {
            var allChildren = new HashSet<Guid>();
            foreach (var driver in Helper.PanelDrivers)
            {
                foreach (var childDriver in driver.Children)
                {
                    allChildren.Add(childDriver);
                }
            }

            var children = new List<repositoryModuleDeviceChild>();
            foreach (var childDriverUID in allChildren)
            {
                var childDriver = ItvManager.Drivers.FirstOrDefault(x => x.UID == childDriverUID);
                var childDevice = new repositoryModuleDeviceChild()
                {
                    id = childDriver.DriverType.ToString()
                };
                children.Add(childDevice);
            }
            Device.children = children.ToArray();
        }

        void CreateStates()
        {
            var deviceStates = new List<repositoryModuleDeviceState>();
            foreach (var stateType in Helper.AllStates)
            {
                var deviceState = new repositoryModuleDeviceState()
                {
                    id = stateType.ToString(),
                    image = "PanelDevice" + "." + stateType.ToString() + ".bmp"
                };
                deviceStates.Add(deviceState);
            }
            Device.states = deviceStates.ToArray();
        }

        void CreateImages()
        {
            var driver = ItvManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.Rubezh_2AM);
            Helper.CreateImages(driver, "PanelDevice");
        }

        void CreateEvents()
        {
            var states = new HashSet<string>();
            foreach (var driver in Helper.PanelDrivers)
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
                var deviceEvent = new repositoryModuleDeviceEvent()
                {
                    id = state
                };
                deviceEvents.Add(deviceEvent);
            }
            Device.events = deviceEvents.ToArray();
        }

        void CreateProperties()
        {
            var driverProperties = new List<DriverProperty>();
            foreach (var driver in Helper.PanelDrivers)
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
            allProperties.Add(CreatePanelTypeProperty());
            allProperties.Add(CreateChannelNoProperty());

            Device.properties = allProperties.ToArray();
        }

        PropertyStringEnumType CreatePanelTypeProperty()
        {
            var panelTypePropertyValues = new List<PropertyStringEnumTypeValue>();

            foreach (var driver in Helper.PanelDrivers)
            {
                var propertyValue = new PropertyStringEnumTypeValue()
                {
                    Value = driver.DriverType.ToString()
                };
                panelTypePropertyValues.Add(propertyValue);
            }

            var panelTypeProperty = new PropertyStringEnumType()
            {
                id = "PanelType",
                value = panelTypePropertyValues.ToArray()
            };

            return panelTypeProperty;
        }

        PropertyStringEnumType CreateChannelNoProperty()
        {
            var channelNoPropertyValues = new List<PropertyStringEnumTypeValue>();

            var propertyValue1 = new PropertyStringEnumTypeValue()
            {
                Value = "ChanneOne"
            };
            channelNoPropertyValues.Add(propertyValue1);

            var propertyValue2 = new PropertyStringEnumTypeValue()
            {
                Value = "ChanneTwo"
            };
            channelNoPropertyValues.Add(propertyValue2);

            var channelNoProperty = new PropertyStringEnumType()
            {
                id = "ChannelNo",
                value = channelNoPropertyValues.ToArray()
            };

            return channelNoProperty;
        }
    }
}
