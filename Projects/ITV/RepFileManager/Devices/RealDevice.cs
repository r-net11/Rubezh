using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media;
using FiresecAPI.Models;
using FiresecClient;
using ItvIntergation.Ngi;

namespace RepFileManager
{
    public class RealDevice
    {
        public repositoryModuleDevice Device { get; private set; }
        Driver _driver;

        public void Initialize(Driver driver)
        {
            _driver = driver;
            Device = new repositoryModuleDevice()
            {
                id = driver.DriverType.ToString()
            };

            CreateChildren();
            CreateEvents();
            CreateStates();
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
                var childDevice = new repositoryModuleDeviceChild()
                {
                    id = childDriver.DriverType.ToString()
                };
                children.Add(childDevice);
            }

            if (children.Count > 0)
                Device.childs = children.ToArray();
        }

        void CreateEvents()
        {
            var deviceEvents = new List<repositoryModuleDeviceEvent>();

            foreach (var driverState in _driver.States)
            {
                if (driverState.Code.StartsWith("Reserved_"))
                    continue;

                var deviceEvent = new repositoryModuleDeviceEvent()
                {
                    id = driverState.Code
                };
                deviceEvents.Add(deviceEvent);
            }

            if (deviceEvents.Count > 0)
                Device.events = deviceEvents.ToArray();
        }

        void CreateStates()
        {
            var deviceStates = new List<repositoryModuleDeviceState>();
            foreach (var stateType in Helper.AllStates)
            {
                var deviceState = new repositoryModuleDeviceState();
                deviceState.id = stateType.ToString();
                deviceState.image = _driver.DriverType.ToString() + "." + stateType.ToString() + ".bmp";
                deviceStates.Add(deviceState);
            }
            Device.states = deviceStates.ToArray();
        }

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
            var allProperties = Helper.CreateProperties(_driver.Properties);
            if (allProperties.Count > 0)
            {
                Device.properties = allProperties.ToArray();
            }
        }

        void CreateImages()
        {
            var libraryDevice = FiresecManager.LibraryConfiguration.Devices.FirstOrDefault(x => x.DriverId == _driver.UID);
            if (libraryDevice != null)
            {
                foreach (var stateType in Helper.AllStates)
                {
                    var state = libraryDevice.States.FirstOrDefault(x => x.StateType == stateType && x.Code == null);
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
    }
}
