//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using XFiresecAPI;
//using Infrastructure.Common;
//using System.IO;
//using Ionic.Zip;

//namespace GKImitator.Processor
//{
//    public static class ConfigurationManager
//    {
//        public static XDeviceConfiguration DeviceConfiguration { get; set; }
//        public static XDriversConfiguration DriversConfiguration { get; set; }

//        public static void Load()
//        {
//            var serverConfigName = AppDataFolderHelper.GetServerAppDataPath("Config.fscp");
//            var folderName = AppDataFolderHelper.GetFolder("Server2");
//            var configFileName = Path.Combine(folderName, "Config.fscp");
//            if (Directory.Exists(folderName))
//                Directory.Delete(folderName, true);
//            Directory.CreateDirectory(folderName);
//            File.Copy(serverConfigName, configFileName);

//            var zipFile = ZipFile.Read(configFileName, new ReadOptions { Encoding = Encoding.GetEncoding("cp866") });
//            var fileInfo = new FileInfo(configFileName);
//            var unzipFolderPath = Path.Combine(fileInfo.Directory.FullName, "Unzip");
//            zipFile.ExtractAll(unzipFolderPath);

//            var configurationFileName = Path.Combine(unzipFolderPath, "XDeviceConfiguration.xml");
//            DeviceConfiguration = ZipSerializeHelper.DeSerialize<XDeviceConfiguration>(configurationFileName);

//            configurationFileName = Path.Combine(unzipFolderPath, "DriversConfiguration.xml");
//            DriversConfiguration = ZipSerializeHelper.DeSerialize<DriversConfiguration>(configurationFileName);
//            DriversConfiguration.Drivers.ForEach(x => x.Properties.RemoveAll(z => z.IsAUParameter));
//            DriverConfigurationParametersHelper.CreateKnownProperties(DriversConfiguration.Drivers);
//            AddTempStates();
//            Update();
//        }

		

//        public static List<XDevice> Devices
//        {
//            get { return DeviceConfiguration.Devices; }
//        }

//        public static List<XZone> Zones
//        {
//            get { return DeviceConfiguration.Zones; }
//        }

//        public static List<XDirection> Directions
//        {
//            get { return DeviceConfiguration.Directions; }
//        }

//        public static List<XDriver> Drivers
//        {
//            get { return DriversConfiguration.XDrivers; }
//        }

//        public static void Update()
//        {
//            DeviceConfiguration.Update();
//            DeviceConfiguration.Reorder();
//            foreach (var device in Devices)
//            {
//                device.Driver = DriversConfiguration.XDrivers.FirstOrDefault(x => x.UID == device.DriverUID);
//                if (device.Driver == null)
//                {
//                    continue;
//                }
//            }
//            DeviceConfiguration.InvalidateConfiguration();
//            DeviceConfiguration.UpdateCrossReferences();
//            foreach (var device in Devices)
//            {
//                device.UpdateHasExternalDevices();
//                device.DeviceState = new DeviceState()
//                {
//                    DeviceUID = device.UID,
//                    Device = device
//                };
//            }

//            foreach (var zone in DeviceConfiguration.Zones)
//            {
//                zone.ZoneState = new ZoneState()
//                {
//                    Zone = zone,
//                    ZoneUID = zone.UID
//                };
//            }
//        }
//    }
//}