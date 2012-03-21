using System;
using System.Collections.Generic;
using System.Linq;
using GroupControllerModule.Models;
using GroupControllerModule.ViewModels;
using Infrastructure;
using System.Text;

namespace GroupControllerModule.Converter
{
    public static class BinConverter
    {
        public static void Convert()
        {
            foreach (var zone in XManager.DeviceConfiguration.Zones)
            {
                zone.KAUDevices = new List<XDevice>();
                foreach (var deviceUID in zone.DeviceUIDs)
                {
                    var device = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
                    var kauDevice = device.AllParents.FirstOrDefault(x => x.Driver.DriverType == XDriverType.KAU);

                    if (zone.KAUDevices.Any(x => x.UID == kauDevice.UID) == false)
                        zone.KAUDevices.Add(kauDevice);
                }
            }

            //var message = new StringBuilder();

            foreach (var device in XManager.DeviceConfiguration.Devices)
            {
                if (device.Driver.DriverType == XDriverType.KAU)
                {
                    device.InternalKAUNo = 1;
                    short currentNo = 3;

                    foreach (var childDevice in device.Children)
                    {
                        if (childDevice.Driver.DriverType == XDriverType.KAUIndicator)
                            childDevice.InternalKAUNo = 2;
                        else
                            childDevice.InternalKAUNo = currentNo++;
                    }

                    var indicatorDevice = device.Children.FirstOrDefault(x => x.Driver.DriverType == XDriverType.KAUIndicator);
                    var indicatorBytes = GetDeviceBytes(indicatorDevice);
                    //message.AppendLine(indicatorDevice.Driver.ShortName + " - " + indicatorDevice.Address + ": " + BytesToString(indicatorBytes));

                    foreach (var childDevice in device.Children)
                    {
                        if (childDevice.Driver.DriverType != XDriverType.KAUIndicator)
                        {
                            var bytes = GetDeviceBytes(childDevice);
                            //message.AppendLine(childDevice.Driver.ShortName + " - " + childDevice.Address + ": " + BytesToString(bytes));
                        }
                    }

                    foreach (var zone in XManager.DeviceConfiguration.Zones)
                    {
                        if ((zone.KAUDevices.Count == 1) && (zone.KAUDevices[0].UID == device.UID))
                        {
                            zone.InternalKAUNo = currentNo++;
                            var bytes = GetZoneBytes(zone);
                            //message.AppendLine(zone.No.ToString() + " - " + zone.Name + ": " + BytesToString(bytes));
                        }
                    }
                }
            }

            var deviceConverterViewModel = new DeviceConverterViewModel();
            //deviceConverterViewModel.BinText = message.ToString();
            ServiceFactory.UserDialogs.ShowModalWindow(deviceConverterViewModel);
        }

        static List<byte> GetZoneBytes(XZone zone)
        {
            var zoneBytes = new List<byte>();
            return zoneBytes;
        }

        static List<byte> GetDeviceBytes(XDevice device)
        {
            short type = device.Driver.DriverTypeNo;
            short address = 0;
            if (device.Driver.IsDeviceOnShleif)
                address = (short)(device.ShleifNo * 256 + device.IntAddress);
            List<byte> objectOutDependencesBytes = GetObjectOutDependencesBytes();
            short outDependensesCount = (short)(objectOutDependencesBytes.Count() / 2);
            List<byte> formulaBytes = GetFormulaBytes();
            short initializationTableOffset = (short)(8 + objectOutDependencesBytes.Count() + formulaBytes.Count());
            List<byte> propertiesBytes = GetPropertiesBytes(device);
            short propertiesBytesCount = (short)(propertiesBytes.Count() / 4);

            var deviceBytes = new List<byte>();
            deviceBytes.AddRange(BitConverter.GetBytes(type));
            deviceBytes.AddRange(BitConverter.GetBytes(address));
            deviceBytes.AddRange(BitConverter.GetBytes(initializationTableOffset));
            deviceBytes.AddRange(BitConverter.GetBytes(outDependensesCount));
            deviceBytes.AddRange(objectOutDependencesBytes);
            deviceBytes.AddRange(formulaBytes);
            deviceBytes.AddRange(BitConverter.GetBytes(propertiesBytesCount));
            deviceBytes.AddRange(propertiesBytes);
            return deviceBytes;
        }

        static List<byte> GetObjectOutDependencesBytes()
        {
            var bytes = new List<byte>();
            for (int i = 0; i < 10; i++)
            {
                short objectNo = (short)i;
                bytes.AddRange(BitConverter.GetBytes(objectNo));
            }
            return bytes;
        }

        static List<byte> GetFormulaBytes()
        {
            var bytes = new List<byte>();

            var formulaOperationType = FormulaOperationType.END;
            byte operationType = (byte)formulaOperationType;

            byte firstOperand = 0;
            short secondOperand = 0;

            bytes.Add(operationType);
            bytes.Add(firstOperand);
            bytes.AddRange(BitConverter.GetBytes(secondOperand));

            return bytes;
        }

        static List<byte> GetPropertiesBytes(XDevice xDevice)
        {
            var bytes = new List<byte>();

            foreach (var property in xDevice.Properties)
            {
                var driverProperty = xDevice.Driver.Properties.FirstOrDefault(x=>x.Name == property.Name);
                if (driverProperty.IsInternalDeviceParameter)
                {
                    byte parameterNo = driverProperty.No;
                    short parameterValue = (short)property.Value;

                    bytes.Add(parameterNo);
                    bytes.AddRange(BitConverter.GetBytes(parameterValue));
                    bytes.Add(0);
                }
            }

            return bytes;
        }

        //static string BytesToString(List<byte> bytes)
        //{
        //    var message = new StringBuilder();
        //    foreach (var b in bytes)
        //    {
        //        message.Append(b.ToString("x2") + " ");
        //    }
        //    return message.ToString();
        //}
    }
}