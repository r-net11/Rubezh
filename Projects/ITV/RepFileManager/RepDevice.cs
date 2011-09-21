using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using ItvIntergation.Ngi;

namespace RepFileManager
{
    public class RepDevice
    {
        public repositoryModuleDevice Device { get; private set; }
        Driver _driver;

        public void Initialize(Driver driver)
        {
            _driver = driver;
            Device = new repositoryModuleDevice();
            Device.id = driver.Name + "." + driver.DriverType;

            if (driver.DriverType == DriverType.Computer)
                Device.root = true;

            CreateChildren();
            CreateEvents();
            CreateStates();
            CreateStateRules();
            CreateCommands();
            CreateProperties();
        }

        void CreateChildren()
        {
            var children = new List<repositoryModuleDeviceChild>();
            foreach (var childDriverUID in _driver.Children)
            {
                var childDriver = FiresecManager.Drivers.FirstOrDefault(x => x.UID == childDriverUID);
                var childDevice = new repositoryModuleDeviceChild();
                childDevice.id = childDriver.Name + "." + childDriver.DriverType;
                children.Add(childDevice);
            }
            Device.childs = children.ToArray();
        }

        void CreateEvents()
        {
            var deviceEvents = new List<repositoryModuleDeviceEvent>();
            foreach (var stateType in AllStates)
            {
                var deviceEvent = new repositoryModuleDeviceEvent();
                deviceEvent.id = stateType.ToString();
                deviceEvents.Add(deviceEvent);
            }
            Device.events = deviceEvents.ToArray();
        }

        void CreateStates()
        {
            List<repositoryModuleDeviceState> deviceStates = new List<repositoryModuleDeviceState>();
            foreach (var stateType in AllStates)
            {
                var deviceState = new repositoryModuleDeviceState();
                deviceState.id = stateType.ToString();
                deviceState.image = _driver.DriverType.ToString() + "." + stateType.ToString() + ".bmp";
                deviceStates.Add(deviceState);
            }
            Device.states = deviceStates.ToArray();
        }

        void CreateStateRules()
        {
            var stateRules = new List<repositoryModuleDeviceStaterule>();
            foreach (var stateType in AllStates)
            {
                var stateRule = new repositoryModuleDeviceStaterule();
                stateRule.@event = stateType.ToString();
                stateRule.from = "*";
                stateRule.to = stateType.ToString();
                stateRules.Add(stateRule);
            }
            Device.staterules = stateRules.ToArray();
        }

        void CreateCommands()
        {
            if (_driver.IsIgnore)
            {
                var commands = new List<repositoryModuleDeviceCommand>();
                commands.Add(new repositoryModuleDeviceCommand() { id = "включить" });
                commands.Add(new repositoryModuleDeviceCommand() { id = "выключить" });
                Device.commands = commands.ToArray();
            }
        }

        void CreateProperties()
        {
            if (_driver.HasAddress)
            {
                List<PropertyStringType> stringProperties = new List<PropertyStringType>();
                var stringProperty = new PropertyStringType();
                stringProperty.id = "Адрес";
                stringProperty.value = "";
                stringProperties.Add(stringProperty);
                Device.properties = stringProperties.ToArray();
            }
        }

        List<StateType> AllStates
        {
            get { return new List<StateType>(Enum.GetValues(typeof(StateType)).Cast<StateType>()); }
        }
    }
}
