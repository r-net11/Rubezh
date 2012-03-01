using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media;
using FiresecAPI.Models;
using FiresecClient;
using ItvIntergation.Ngi;

namespace RepFileManager
{
    public static class Helper
    {
        public static List<StateType> AllStates
        {
            get { return new List<StateType>(Enum.GetValues(typeof(StateType)).Cast<StateType>()); }
        }

        public static List<Driver> CommunicationDrivers
        {
            get
            {
                var drivers = new List<Driver>();
                drivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.MS_1));
                drivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.MS_2));
                return drivers;
            }
        }

        public static List<Driver> PanelDrivers
        {
            get
            {
                var drivers = new List<Driver>();
                drivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.BUNS));
                drivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.Rubezh_10AM));
                drivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.Rubezh_2AM));
                drivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.Rubezh_2OP));
                drivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.Rubezh_4A));
                return drivers;
            }
        }

        public static List<Driver> CommonCommunicationDrivers
        {
            get
            {
                var drivers = new List<Driver>();
                drivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.MS_1));
                drivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.MS_2));
                return drivers;
            }
        }

        public static List<Driver> RealDevices
        {
            get
            {
                var drivers = new List<Driver>();
                drivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.RM_1));
                drivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.MPT));
                drivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.SmokeDetector));
                drivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.HeatDetector));
                drivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.CombinedDetector));
                drivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.AM_1));
                drivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.StopButton));
                drivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.StartButton));
                drivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.AutomaticButton));
                drivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.ShuzOnButton));
                drivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.ShuzOffButton));
                drivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.ShuzUnblockButton));
                drivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.HandDetector));
                drivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.AM1_O));
                drivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.PumpStation));
                drivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.Pump));
                drivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.JokeyPump));
                drivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.Compressor));
                drivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.DrenazhPump));
                drivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.CompensationPump));
                drivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.AMP_4));
                drivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.MRO));
                drivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.Valve));
                drivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.AM1_T));
                drivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.ASPT));
                drivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.MDU));
                drivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.Exit));
                drivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.MRK_30));
                drivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.RadioHandDetector));
                drivers.Add(FiresecManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.RadioSmokeDetector));
                return drivers;
            }
        }

        public static List<object> CreateProperties(List<DriverProperty> driverProperties)
        {
            var stringProperties = new List<PropertyStringType>();
            var boolProperties = new List<PropertyBoolType>();
            var intProperties = new List<PropertyIntType>();
            var stringEnumProperties = new List<PropertyStringEnumType>();
            var allProperties = new List<object>();

            foreach (var driverProperty in driverProperties)
            {
                if (driverProperty.Visible == false)
                    continue;

                if (driverProperty.DriverPropertyType == DriverPropertyTypeEnum.StringType)
                {
                    var stringProperty = new PropertyStringType()
                    {
                        id = driverProperty.Name,
                        value = driverProperty.Default
                    };
                    stringProperties.Add(stringProperty);
                }

                if (driverProperty.DriverPropertyType == DriverPropertyTypeEnum.BoolType)
                {
                    var boolPropertiey = new PropertyBoolType()
                    {
                        id = driverProperty.Name,
                        value = (driverProperty.Default != "0")
                    };
                    boolProperties.Add(boolPropertiey);
                }

                if ((driverProperty.DriverPropertyType == DriverPropertyTypeEnum.IntType) || (driverProperty.DriverPropertyType == DriverPropertyTypeEnum.ByteType))
                {
                    var intPropertiey = new PropertyIntType()
                    {
                        id = driverProperty.Name,
                        value = int.Parse(driverProperty.Default)
                    };
                    intProperties.Add(intPropertiey);
                }

                if (driverProperty.DriverPropertyType == DriverPropertyTypeEnum.EnumType)
                {
                    var stringEnumProperty = new PropertyStringEnumType()
                    {
                        id = driverProperty.Name
                    };

                    var propertyValues = new List<PropertyStringEnumTypeValue>();
                    foreach (var enumPropertyValue in driverProperty.Parameters)
                    {
                        var propertyValue = new PropertyStringEnumTypeValue()
                        {
                            Value = enumPropertyValue.Name
                        };
                        propertyValues.Add(propertyValue);
                    }
                    stringEnumProperty.value = propertyValues.ToArray();

                    stringEnumProperties.Add(stringEnumProperty);
                }
            }

            allProperties.AddRange(stringProperties);
            allProperties.AddRange(boolProperties);
            allProperties.AddRange(intProperties);
            allProperties.AddRange(stringEnumProperties);

            return allProperties;
        }

        public static void CreateImages(Driver driver, string driverName)
        {
            var libraryDevice = FiresecManager.LibraryConfiguration.Devices.FirstOrDefault(x => x.DriverId == driver.UID);
            if (libraryDevice != null)
            {
                foreach (var stateType in Helper.AllStates)
                {
                    var state = libraryDevice.States.FirstOrDefault(x => x.StateType == stateType && x.Code == null);
                    if (state == null)
                        state = libraryDevice.States.FirstOrDefault(x => x.StateType == StateType.No);

                    var name = Directory.GetCurrentDirectory() + "/BMP/" + driverName + "." + stateType.ToString() + ".bmp";
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
