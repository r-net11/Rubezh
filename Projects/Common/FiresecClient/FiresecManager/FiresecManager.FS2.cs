//using System;
//using System.IO;
//using System.Linq;
//using Common;
//using FiresecAPI.Models;
//using FS2Client;
//using Infrastructure.Common;

//namespace FiresecClient
//{
//    public partial class FiresecManager
//    {
//        public static bool IsFS2Enabled
//        {
//            get
//            {
//                return File.Exists("C:/FS2.txt");
//            }
//        }

//        public static FS2ClientContract FS2ClientContract { get; private set; }

//        public static void InitializeFS2()
//        {
//            try
//            {
//                FS2ClientContract = new FS2ClientContract(ConnectionSettingsManager.FS2ServerAddress);
//            }
//            catch (Exception e)
//            {
//                Logger.Error(e, "FiresecManager.InitializeFiresecDriver");
//            }
//        }

//        public static void FS2UpdateDeviceStates()
//        {
//            var deviceStates = FS2ClientContract.GetDeviceStates();
//            if (!deviceStates.HasError && deviceStates.Result != null)
//            {
//                foreach (var deviceState in deviceStates.Result)
//                {
//                    var device = Devices.FirstOrDefault(x => x.UID == deviceState.DeviceUID);
//                    if (device != null)
//                    {
//                        CopyDeviceStatesFromFS2Server(device, deviceState);
//                    }
//                }
//            }

//            var zoneStates = FS2ClientContract.GetZoneStates();
//            if (!zoneStates.HasError && zoneStates.Result != null)
//            {
//                foreach (var zoneState in zoneStates.Result)
//                {
//                    var zone = Zones.FirstOrDefault(x => x.UID == zoneState.ZoneUID);
//                    if (zone != null)
//                    {
//                        zone.ZoneState.StateType = zoneState.StateType;
//                    }
//                }
//            }
//        }

//        public static void CopyDeviceStatesFromFS2Server(Device device, DeviceState deviceState)
//        {
//            device.DeviceState.States = deviceState.SerializableStates;
//            device.DeviceState.ParentStates = deviceState.ParentStates;
//            device.DeviceState.ChildStates = deviceState.ChildStates;
//            device.DeviceState.Parameters = deviceState.Parameters;

//            if (device.DeviceState.ParentStates != null)
//            {
//                foreach (var parentDeviceState in device.DeviceState.ParentStates)
//                {
//                    parentDeviceState.ParentDevice = FiresecManager.Devices.FirstOrDefault(x => x.UID == parentDeviceState.ParentDeviceUID);
//                }
//                device.DeviceState.ParentStates.RemoveAll(x => x.ParentDevice == null);
//            }

//            if (device.DeviceState.ChildStates != null)
//            {
//                foreach (var childDeviceState in device.DeviceState.ChildStates)
//                {
//                    childDeviceState.ChildDevice = FiresecManager.Devices.FirstOrDefault(x => x.UID == childDeviceState.ChildDeviceUID);
//                }
//                device.DeviceState.ChildStates.RemoveAll(x => x.ChildDevice == null);
//            }
//        }
//    }
//}