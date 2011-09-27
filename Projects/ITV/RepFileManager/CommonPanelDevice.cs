using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItvIntergation.Ngi;
using FiresecClient;
using FiresecAPI.Models;
using System.IO;
using System.Windows.Media;

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

        void CreateProperties()
        {
            List<PropertyStringType> stringProperties = new List<PropertyStringType>();

            //HashSet<DriverProperty> driverProperties;

            foreach (var driver in Helper.CommonPanelDrivers)
            {
                //driver.Properties
            }

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
