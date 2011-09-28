using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItvIntergation.Ngi;
using FiresecClient;
using FiresecAPI.Models;
using System.IO;
using System.Windows.Media;
using System.Diagnostics;

namespace RepFileManager
{
    public class CommonPanelDevice
    {
        public repositoryModuleDevice Device { get; private set; }

        public CommonPanelDevice()
        {
            Device = new repositoryModuleDevice();
            Device.id = "CommonPanelDevice";

            CreateStates();
            CreateEvents();
            CreateProperties();
            CreateImages();
        }

        void CreateStates()
        {
            var driver = FiresecManager.Drivers.FirstOrDefault(x=>x.DriverType == DriverType.Rubezh_2AM);

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
            foreach (var driver in Helper.CommonPanelDrivers)
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

            Device.events.ToList().ForEach((x) => { Trace.WriteLine(x.id); });
        }

        void CreateProperties()
        {
            var stringProperties = new HashSet<PropertyStringType>();

            var properties = new List<PropertyStringEnumType>();

            var property = new PropertyStringEnumType();
            var propertyValues = new List<PropertyStringEnumTypeValue>();

            foreach (var driver in Helper.CommonPanelDrivers)
            {
                var propertyValue = new PropertyStringEnumTypeValue();
                propertyValue.Value = driver.DriverType.ToString();
                propertyValues.Add(propertyValue);
            }

            property.value = propertyValues.ToArray();
            properties.Add(property);

            Device.properties = properties.ToArray();
        }

        void CreateImages()
        {
            var driver = FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.Rubezh_2AM);

            var libraryDevice = FiresecManager.LibraryConfiguration.Devices.FirstOrDefault(x => x.DriverId == driver.UID);
            if (libraryDevice != null)
            {
                foreach (var stateType in Helper.AllStates)
                {
                    var state = libraryDevice.States.FirstOrDefault(x => x.StateType == stateType && x.Code == null);
                    if (state == null)
                        state = libraryDevice.States.FirstOrDefault(x => x.StateType == StateType.No);

                    var name = Directory.GetCurrentDirectory() + "/BMP/" + "CommonPanelDevice" + "." + stateType.ToString() + ".bmp";
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
