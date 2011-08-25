using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using FiresecAPI;
using FiresecAPI.Models;
using System.Timers;

namespace FiresecClient
{
    public class FiresecManager
    {
        public static List<Driver> Drivers { get; set; }
        public static DeviceConfiguration DeviceConfiguration { get; set; }
        public static DeviceConfigurationStates DeviceStates { get; set; }
        public static LibraryConfiguration LibraryConfiguration { get; set; }
        public static SystemConfiguration SystemConfiguration { get; set; }
        public static PlansConfiguration PlansConfiguration { get; set; }
        public static SecurityConfiguration SecurityConfiguration { get; set; }

        static SafeFiresecService _firesecService;

        public static bool Connect(string login, string password)
        {
            IFiresecService firesecService = FiresecServiceFactory.Create();
            _firesecService = new SafeFiresecService(firesecService);

            bool result = _firesecService.Connect(login, password);
            if (result)
            {
                FileHelper.Synchronize();
                Drivers = _firesecService.GetDrivers();
                SystemConfiguration = _firesecService.GetSystemConfiguration();
                LibraryConfiguration = _firesecService.GetLibraryConfiguration();
                PlansConfiguration = _firesecService.GetPlansConfiguration();
                SecurityConfiguration = _firesecService.GetSecurityConfiguration();
                DeviceConfiguration = _firesecService.GetDeviceConfiguration();
                DeviceStates = _firesecService.GetStates();
                Update();

                _loggedInUserName = login;

                _firesecService.StartPing();
                _firesecService.Test();
            }

            return result;
        }

        public static bool Reconnect(string login, string password)
        {
            bool result = _firesecService.Reconnect(login, password);
            if (result)
            {
                _loggedInUserName = login;
            }
            return result;
        }

        static void Update()
        {
            DeviceConfiguration.Update();

            foreach (var driver in Drivers)
            {
                driver.ImageSource = FileHelper.GetIconFilePath(driver.ImageSource) + ".ico";
            }

            foreach (var device in DeviceConfiguration.Devices)
            {
                device.Driver = FiresecManager.Drivers.FirstOrDefault(x => x.Id == device.DriverId);

                if ((device.Driver.IsIndicatorDevice) || (device.IndicatorLogic != null))
                {
                    var indicatorDevice = DeviceConfiguration.Devices.FirstOrDefault(x=>x.UID == device.IndicatorLogic.DeviceUID);
                    device.IndicatorLogic.Device = indicatorDevice;
                }

                if (device.Driver.IsZoneLogicDevice)
                {
                    foreach (var clause in device.ZoneLogic.Clauses)
                    {
                        if (clause.DeviceUID != null)
                        {
                            clause.Device = DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == clause.DeviceUID);
                        }
                    }
                }
            }

            foreach (var deviceState in DeviceStates.DeviceStates)
            {
                deviceState.Device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.Id == deviceState.Id);

                foreach (var state in deviceState.States)
                {
                    state.DriverState = deviceState.Device.Driver.States.FirstOrDefault(x => x.Code == state.Code);
                }

                foreach (var parentState in deviceState.ParentStates)
                {
                    parentState.ParentDevice = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.Id == parentState.ParentDeviceId);
                    parentState.DriverState = parentState.ParentDevice.Driver.States.FirstOrDefault(x => x.Code == parentState.Code);
                }
            }
        }

        static string _loggedInUserName;
        public static User CurrentUser
        {
            get { return SecurityConfiguration.Users.FirstOrDefault(x => x.Name == _loggedInUserName); }
        }

        public static List<Perimission> CurrentPermissions
        {
            get
            {
                var permissionIds = new List<string>();

                foreach (var groupId in CurrentUser.Groups)
                {
                    var group = SecurityConfiguration.UserGroups.FirstOrDefault(x => x.Id == groupId);
                    if (group != null)
                        permissionIds.AddRange(group.Permissions);
                }
                permissionIds.AddRange(CurrentUser.Permissions);

                foreach (var permissionId in CurrentUser.RemovedPermissions)
                {
                    permissionIds.Remove(permissionId);
                }

                var permissions = new List<Perimission>(from permission in SecurityConfiguration.Perimissions
                                                        where permissionIds.Contains(permission.Id)
                                                        select permission);
                return permissions;
            }
        }

        public static void Disconnect()
        {
            _firesecService.Disconnect();
            FiresecServiceFactory.Dispose();
        }

        public static void SetConfiguration()
        {
            _firesecService.SetSystemConfiguration(SystemConfiguration);
            _firesecService.SetPlansConfiguration(PlansConfiguration);

            //LibraryConfiguration = new FiresecAPI.Models.LibraryConfiguration();
            //foreach (var device in DeviceLibraryConfiguration.Devices)
            //{
            //    var ldevice = new LibraryDevice()
            //    {
            //        DriverId = device.Id,
            //        States = new List<LibraryState>()
            //    };

            //    foreach (var state in device.States)
            //    {
            //        var lstate = new LibraryState()
            //        {
            //            Code = state.Code,
            //            StateType = state.StateType,
            //            Frames = new List<LibraryFrame>()
            //        };
            //        foreach (var frame in state.Frames)
            //        {
            //            lstate.Frames.Add(new LibraryFrame()
            //            {
            //                Duration = frame.Duration,
            //                Id = frame.Id,
            //                Image = frame.Image,
            //                Layer = frame.Layer
            //            });
            //        }

            //        ldevice.States.Add(lstate);
            //    }

            //    LibraryConfiguration.Devices.Add(ldevice);
            //}

            _firesecService.SetLibraryConfiguration(LibraryConfiguration);
            _firesecService.SetDeviceConfiguration(DeviceConfiguration);
        }

        public static void AddToIgnoreList(List<string> deviceIds)
        {
            _firesecService.AddToIgnoreList(deviceIds);
        }

        public static void RemoveFromIgnoreList(List<string> deviceIds)
        {
            _firesecService.RemoveFromIgnoreList(deviceIds);
        }

        public static void ResetStates(List<ResetItem> resetItems)
        {
            _firesecService.ResetStates(resetItems);
        }

        public static void AddUserMessage(string message)
        {
            _firesecService.AddUserMessage(message);
        }

        public static void ExecuteCommand(string deviceId, string methodName)
        {
            _firesecService.ExecuteCommand(deviceId, methodName);
        }

        public static List<JournalRecord> ReadJournal(int startIndex, int count)
        {
            return _firesecService.ReadJournal(startIndex, count);
        }

        public static List<string> GetFileNamesList(string directory)
        {
            return _firesecService.GetFileNamesList(directory);
        }

        public static Dictionary<string, string> GetDirectoryHash(string directory)
        {
            return _firesecService.GetDirectoryHash(directory);
        }

        public static Stream GetFile(string filepath)
        {
            return _firesecService.GetFile(filepath);
        }

        public static void LoadFromFile(string fileName)
        {
            var dataContractSerializer = new DataContractSerializer(typeof(DeviceConfiguration));
            using (var fileStream = new FileStream(fileName, FileMode.Open))
            {
                FiresecManager.DeviceConfiguration = (DeviceConfiguration) dataContractSerializer.ReadObject(fileStream);
            }

            Update();
        }

        public static void SaveToFile(string fileName)
        {
            var dataContractSerializer = new DataContractSerializer(typeof(DeviceConfiguration));
            using (var fileStream = new FileStream(fileName, FileMode.Create))
            {
                dataContractSerializer.WriteObject(fileStream, FiresecManager.DeviceConfiguration);
            }
        }

        public static void Test()
        {
            _firesecService.Test();
        }
    }
}