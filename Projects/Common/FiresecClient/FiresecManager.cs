using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using FiresecAPI.Models;
using System.ServiceModel;

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

        static public SafeFiresecService FiresecService { get; private set; }

        public static string Connect(string clientCallbackAddress, string serverAddress, string login, string password)
        {
            FiresecCallbackServiceManager.Open(clientCallbackAddress);

            FiresecService = new SafeFiresecService(FiresecServiceFactory.Create(serverAddress));

            string result = FiresecService.Connect(clientCallbackAddress, login, password);
            if (result != null)
            {
                return result;
            }

            _userLogin = login;
            return null;
        }

        public static void SelectiveFetch(bool updateFiles = true)
        {
            if (updateFiles)
                FileHelper.Synchronize();

            SystemConfiguration = FiresecService.GetSystemConfiguration();
            LibraryConfiguration = FiresecService.GetLibraryConfiguration();
            PlansConfiguration = FiresecService.GetPlansConfiguration();
            SecurityConfiguration = FiresecService.GetSecurityConfiguration();
            Drivers = FiresecService.GetDrivers();
            DeviceConfiguration = FiresecService.GetDeviceConfiguration();
            DeviceStates = FiresecService.GetStates();

            UpdateDrivers();
            UpdateConfiguration();
            UpdatePlansConfiguration();
            UpdateStates();

            FiresecService.Subscribe();
            FiresecService.StartPing();
        }

        public static void ActiveXFetch()
        {
            Drivers = FiresecService.GetDrivers();
            LibraryConfiguration = FiresecService.GetLibraryConfiguration();
            DeviceConfiguration = FiresecService.GetDeviceConfiguration();
            DeviceStates = FiresecService.GetStates();

            //UpdateDrivers();
            UpdateActiveXConfiguration();
            UpdateStates();

            FiresecService.Subscribe();
            FiresecService.StartPing();
        }

        public static string Reconnect(string login, string password)
        {
            string result = FiresecService.Reconnect(login, password);
            if (result != null)
            {
                return result;
            }

            _userLogin = login;
            return null;
        }

        public static void UpdateDrivers()
        {
            Drivers.ForEach(x => x.ImageSource = FileHelper.GetIconFilePath(x.ImageSource) + ".ico");
        }

        public static void UpdateActiveXConfiguration()
        {
            DeviceConfiguration.Update();

            foreach (var device in DeviceConfiguration.Devices)
            {
                device.Driver = FiresecManager.Drivers.FirstOrDefault(x => x.UID == device.DriverUID);
                if (device.Driver.IsIndicatorDevice || device.IndicatorLogic != null)
                    device.IndicatorLogic.Device = DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == device.IndicatorLogic.DeviceUID);

                if (device.Driver.IsZoneLogicDevice && device.ZoneLogic != null)
                {
                    foreach (var clause in device.ZoneLogic.Clauses.Where(x => x.DeviceUID != Guid.Empty))
                    {
                        clause.Device = DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == clause.DeviceUID);
                    }
                }
            }
        }

        public static void UpdateConfiguration()
        {
            PlansConfiguration.Update();
            DeviceConfiguration.Update();

            foreach (var device in DeviceConfiguration.Devices)
            {
                device.Driver = FiresecManager.Drivers.FirstOrDefault(x => x.UID == device.DriverUID);
                if (device.Driver.IsIndicatorDevice || device.IndicatorLogic != null)
                    device.IndicatorLogic.Device = DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == device.IndicatorLogic.DeviceUID);

                if (device.Driver.IsZoneLogicDevice && device.ZoneLogic != null)
                {
                    foreach (var clause in device.ZoneLogic.Clauses.Where(x => x.DeviceUID != Guid.Empty))
                    {
                        clause.Device = DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == clause.DeviceUID);
                    }
                }
            }
        }

        public static void UpdatePlansConfiguration()
        {
            FiresecManager.DeviceConfiguration.Devices.ForEach(x => { x.PlanElementUIDs = new List<Guid>(); });

            foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
            {
                for (int i = plan.ElementDevices.Count(); i > 0; i--)
                {
                    var elementDevice = plan.ElementDevices[i - 1];

                    var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == elementDevice.DeviceUID);
                    if (device != null)
                    {
                        device.PlanElementUIDs.Add(elementDevice.UID);
                        elementDevice.Device = device;
                    }
                    else
                    {
                        plan.ElementDevices.RemoveAt(i - 1);
                    }
                    var deviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.UID == elementDevice.DeviceUID);
                    if (deviceState != null)
                    {
                        elementDevice.DeviceState = deviceState;
                    }
                }

                foreach (var elementZone in plan.ElementPolygonZones)
                {
                    if (elementZone.ZoneNo.HasValue)
                    {
                        elementZone.Zone = FiresecManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == elementZone.ZoneNo.Value);
                    }
                }

                foreach (var elementZone in plan.ElementRectangleZones)
                {
                    if (elementZone.ZoneNo.HasValue)
                    {
                        elementZone.Zone = FiresecManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == elementZone.ZoneNo.Value);
                    }
                }

                foreach (var elementSubPlan in plan.ElementSubPlans)
                {
                    elementSubPlan.Plan = FiresecManager.PlansConfiguration.AllPlans.FirstOrDefault(x => x.UID == elementSubPlan.PlanUID);
                }
            }
        }

        public static void UpdateStates()
        {
            foreach (var deviceState in DeviceStates.DeviceStates)
            {
                deviceState.Device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceState.UID);
                if (deviceState.Device == null)
                    continue;

                foreach (var state in deviceState.States)
                {
                    state.DriverState = deviceState.Device.Driver.States.FirstOrDefault(x => x.Code == state.Code);
                }

                foreach (var parentState in deviceState.ParentStates)
                {
                    parentState.ParentDevice = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == parentState.ParentDeviceId);
                    if (parentState.ParentDevice != null)
                        parentState.DriverState = parentState.ParentDevice.Driver.States.FirstOrDefault(x => x.Code == parentState.Code);
                }
            }
        }

        static string _userLogin;
        public static User CurrentUser
        {
            get { return SecurityConfiguration.Users.FirstOrDefault(x => x.Login == _userLogin); }
        }

        public static void Disconnect()
        {
            if (FiresecService != null)
            {
                FiresecService.StopPing();
                FiresecService.Disconnect();
            }
            FiresecServiceFactory.Dispose();
            FiresecCallbackServiceManager.Close();
        }

        public static void AddToIgnoreList(List<Guid> deviceUIDs)
        {
            FiresecService.AddToIgnoreList(deviceUIDs);
        }

        public static void RemoveFromIgnoreList(List<Guid> deviceUIDs)
        {
            FiresecService.RemoveFromIgnoreList(deviceUIDs);
        }

        public static void ResetStates(List<ResetItem> resetItems)
        {
            FiresecService.ResetStates(resetItems);
        }

        public static void AddUserMessage(string message)
        {
            FiresecService.AddUserMessage(message);
        }

        public static void AddJournalRecord(JournalRecord journalRecord)
        {
            FiresecService.AddJournalRecord(journalRecord);
        }

        public static void ExecuteCommand(Guid deviceUID, string methodName)
        {
            FiresecService.ExecuteCommand(deviceUID, methodName);
        }

        public static List<JournalRecord> ReadJournal(int startIndex, int count)
        {
            return FiresecService.ReadJournal(startIndex, count);
        }

        public static IEnumerable<JournalRecord> GetFilteredJournal(JournalFilter journalFilter)
        {
            return FiresecService.GetFilteredJournal(journalFilter);
        }

        public static IEnumerable<JournalRecord> GetFilteredArchive(ArchiveFilter archiveFilter)
        {
            return FiresecService.GetFilteredArchive(archiveFilter);
        }

        public static IEnumerable<JournalRecord> GetDistinctRecords()
        {
            return FiresecService.GetDistinctRecords();
        }

        public static DateTime GetArchiveStartDate()
        {
            return FiresecService.GetArchiveStartDate();
        }

        public static List<string> GetFileNamesList(string directory)
        {
            return FiresecService.GetFileNamesList(directory);
        }

        public static Dictionary<string, string> GetDirectoryHash(string directory)
        {
            return FiresecService.GetDirectoryHash(directory);
        }

        public static Stream GetFile(string filepath)
        {
            return FiresecService.GetFile(filepath);
        }

        public static DeviceConfiguration AutoDetectDevice(Guid deviceUID, bool fastSearch)
        {
            return FiresecService.DeviceAutoDetectChildren(DeviceConfiguration.CopyOneBranch(deviceUID, false), deviceUID, fastSearch);
        }

        public static DeviceConfiguration DeviceReadConfiguration(Guid deviceUID, bool isUsb)
        {
            return FiresecService.DeviceReadConfiguration(DeviceConfiguration.CopyOneBranch(deviceUID, isUsb), deviceUID);
        }

        public static void DeviceWriteConfiguration(Guid deviceUID, bool isUsb)
        {
            FiresecService.DeviceWriteConfiguration(DeviceConfiguration.CopyOneBranch(deviceUID, isUsb), deviceUID);
        }

        public static void WriteAllDeviceConfiguration()
        {
            FiresecService.DeviceWriteAllConfiguration(DeviceConfiguration);
        }

        public static string ReadDeviceJournal(Guid deviceUID, bool isUsb)
        {
            return FiresecService.DeviceReadEventLog(DeviceConfiguration.CopyOneBranch(deviceUID, isUsb), deviceUID);
        }

        public static void SynchronizeDevice(Guid deviceUID, bool isUsb)
        {
            FiresecService.DeviceDatetimeSync(DeviceConfiguration.CopyOneBranch(deviceUID, isUsb), deviceUID);
        }

        public static string DeviceUpdateFirmware(Guid deviceUID, bool isUsb, byte[] bytes, string fileName)
        {
            return FiresecService.DeviceUpdateFirmware(DeviceConfiguration.CopyOneBranch(deviceUID, isUsb), deviceUID, bytes, fileName);
        }

        public static string DeviceVerifyFirmwareVersion(Guid deviceUID, bool isUsb, byte[] bytes, string fileName)
        {
            return FiresecService.DeviceVerifyFirmwareVersion(DeviceConfiguration.CopyOneBranch(deviceUID, isUsb), deviceUID, bytes, fileName);
        }

        public static string DeviceGetInformation(Guid deviceUID, bool isUsb)
        {
            return FiresecService.DeviceGetInformation(DeviceConfiguration.CopyOneBranch(deviceUID, isUsb), deviceUID);
        }

        public static List<string> DeviceGetSerialList(Guid deviceUID)
        {
            return FiresecService.DeviceGetSerialList(DeviceConfiguration.CopyOneBranch(deviceUID, false), deviceUID);
        }

        public static void SetPassword(Guid deviceUID, bool isUsb, DevicePasswordType devicePasswordType, string password)
        {
            FiresecService.DeviceSetPassword(DeviceConfiguration.CopyOneBranch(deviceUID, isUsb), deviceUID, devicePasswordType, password);
        }

        public static List<DeviceCustomFunction> DeviceCustomFunctionList(Guid driverUID)
        {
            return FiresecService.DeviceCustomFunctionList(driverUID);
        }

        public static string DeviceCustomFunctionExecute(Guid deviceUID, string functionName)
        {
            return FiresecService.DeviceCustomFunctionExecute(DeviceConfiguration.CopyOneBranch(deviceUID, false), deviceUID, functionName);
        }

        public static string DeviceGetGuardUsersList(Guid deviceUID)
        {
            return FiresecService.DeviceGetGuardUsersList(DeviceConfiguration.CopyOneBranch(deviceUID, false), deviceUID);
        }

        public static string DeviceSetGuardUsersList(Guid deviceUID, string users)
        {
            return FiresecService.DeviceSetGuardUsersList(DeviceConfiguration.CopyOneBranch(deviceUID, false), deviceUID, users);
        }

        public static string DeviceGetMDS5Data(Guid deviceUID)
        {
            return FiresecService.DeviceGetMDS5Data(DeviceConfiguration.CopyOneBranch(deviceUID, false), deviceUID);
        }

        public static void Test()
        {
            FiresecService.Test();
        }
    }
}