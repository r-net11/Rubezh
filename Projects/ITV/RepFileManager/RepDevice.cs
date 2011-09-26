using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using FiresecAPI.Models;
using FiresecClient;
using ItvIntergation.Ngi;
using System.IO;

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
            Device.id = driver.DriverType.ToString();

            if (driver.DriverType == DriverType.Computer)
                Device.root = true;

            CreateChildren();
            CreateEvents();
            CreateStates();
            //CreateStateRules();
            CreateCommands();
            CreateProperties();
            CreateImages();
        }

        void CreateChildren()
        {
            var children = new List<repositoryModuleDeviceChild>();
            foreach (var childDriverUID in _driver.Children)
            {
                var childDriver = FiresecManager.Drivers.FirstOrDefault(x => x.UID == childDriverUID);
                var childDevice = new repositoryModuleDeviceChild();
                childDevice.id = childDriver.DriverType.ToString();
                children.Add(childDevice);
            }
            Device.childs = children.ToArray();
        }

        void CreateEvents()
        {
            var deviceEvents = new List<repositoryModuleDeviceEvent>();

            foreach (var driverState in _driver.States)
            {
                var deviceEvent = new repositoryModuleDeviceEvent();
                deviceEvent.id = driverState.Code; // driverState.Name - на русском языке для локализации
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

        //void CreateStateRules()
        //{
        //    var stateRules = new List<repositoryModuleDeviceStaterule>();
        //    foreach (var stateType in AllStates)
        //    {
        //        var stateRule = new repositoryModuleDeviceStaterule();
        //        stateRule.@event = stateType.ToString();
        //        stateRule.from = "*";
        //        stateRule.to = stateType.ToString();
        //        stateRules.Add(stateRule);
        //    }
        //    Device.staterules = stateRules.ToArray();
        //}

        void CreateCommands()
        {
            if (_driver.IsIgnore)
            {
                var commands = new List<repositoryModuleDeviceCommand>();
                commands.Add(new repositoryModuleDeviceCommand() { id = "Enable" }); // Включить(убрать из списка обхода)
                commands.Add(new repositoryModuleDeviceCommand() { id = "Dasable" }); // Выключить(добавить в список обхода)
                Device.commands = commands.ToArray();
            }
            if (_driver.DriverType == DriverType.Valve)
            {
                var commands = new List<repositoryModuleDeviceCommand>();
                commands.Add(new repositoryModuleDeviceCommand() { id = "BoltClose" }); // Закрыть
                commands.Add(new repositoryModuleDeviceCommand() { id = "BoltStop" }); // Стоп
                commands.Add(new repositoryModuleDeviceCommand() { id = "BoltOpen" }); // Открыть
                commands.Add(new repositoryModuleDeviceCommand() { id = "BoltAutoOn" }); // Включить автоматику
                commands.Add(new repositoryModuleDeviceCommand() { id = "BoltAutoOff" }); // Выключить автоматику
                Device.commands = commands.ToArray();
            }
        }

        void CreateProperties()
        {
            if (_driver.HasAddress)
            {
                List<PropertyStringType> stringProperties = new List<PropertyStringType>();

                foreach (var driverProperty in _driver.Properties)
                {
                    if (driverProperty.DriverPropertyType == DriverPropertyTypeEnum.StringType)
                    {
                        var stringProperty = new PropertyStringType();
                        stringProperty.id = driverProperty.Name; // driverProperty.Caption - на русском языке для локализации
                        stringProperty.value = driverProperty.Default;
                        stringProperties.Add(stringProperty);
                    }
                }

                Device.properties = stringProperties.ToArray();
            }
        }

        void CreateImages()
        {
            var libraryDevice = FiresecManager.LibraryConfiguration.Devices.FirstOrDefault(x => x.DriverId == _driver.UID);
            if (libraryDevice != null)
            {
                foreach (var stateType in AllStates)
                {
                    var state = libraryDevice.States.FirstOrDefault(x=>x.StateType == stateType && x.Code == null);
                    if (state == null)
                        state = libraryDevice.States.FirstOrDefault(x => x.StateType == StateType.No);

                    var name = Directory.GetCurrentDirectory() + "/BMP/" + _driver.DriverType.ToString() + "." + stateType.ToString() + ".bmp";
                    var canvas = ImageHelper.XmlToCanvas(state.Frames[0].Image);

                    if (canvas.Children.Count == 0)
                    {
                        state = libraryDevice.States.FirstOrDefault(x => x.StateType == StateType.No);
                        canvas = ImageHelper.XmlToCanvas(state.Frames[0].Image);
                    }

                    canvas.Background = new SolidColorBrush(Color.FromRgb(0, 128, 128));
                    ImageHelper.XAMLToBitmap(canvas, name);
                }
            }
        }

        List<StateType> AllStates
        {
            get { return new List<StateType>(Enum.GetValues(typeof(StateType)).Cast<StateType>()); }
        }
    }
}
