using System;
using System.Collections.Generic;
using System.ServiceModel;
using FiresecAPI;

namespace FiresecService
{
    [ServiceBehavior(MaxItemsInObjectGraph = 2147483647, UseSynchronizationContext = true,
    InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class SafeFiresecService : IFiresecService
    {
        public FiresecService FiresecService { get; set; }

        public SafeFiresecService()
        {
            FiresecService = new FiresecService();
        }

        public OperationResult<T> CreateEmptyOperationResult<T>()
        {
            var operationResult = new OperationResult<T>
            {
                Result = default(T),
                HasError = true,
                Error = new Exception("Ошибка при выполнении операции на сервере")
            };
            return operationResult;
        }

        public string Connect(string clientType, string clientCallbackAddress, string userName, string password)
        {
            try
            {
                return FiresecService.Connect(clientType, clientCallbackAddress, userName, password);
            }
            catch
            {
                return null;
            }
        }

        public string Reconnect(string userName, string password)
        {
            try
            {
                return FiresecService.Reconnect(userName, password);
            }
            catch
            {
                return null;
            }
        }

        public void Disconnect()
        {
            try
            {
                FiresecService.Disconnect();
            }
            catch
            {
            }
        }

        public void Subscribe()
        {
            try
            {
                FiresecService.Subscribe();
            }
            catch
            {
            }
        }

        public void CancelProgress()
        {
            try
            {
                FiresecService.CancelProgress();
            }
            catch
            {
            }
        }

        public List<FiresecAPI.Models.Driver> GetDrivers()
        {
            try
            {
                return FiresecService.GetDrivers();
            }
            catch
            {
                return null;
            }
        }

        public FiresecAPI.Models.DeviceConfiguration GetDeviceConfiguration()
        {
            try
            {
                return FiresecService.GetDeviceConfiguration();
            }
            catch
            {
                return null;
            }
        }

        public OperationResult<bool> SetDeviceConfiguration(FiresecAPI.Models.DeviceConfiguration deviceConfiguration)
        {
            try
            {
                return FiresecService.SetDeviceConfiguration(deviceConfiguration);
            }
            catch
            {
                return CreateEmptyOperationResult<bool>();
            }
        }

        public OperationResult<bool> DeviceWriteConfiguration(FiresecAPI.Models.DeviceConfiguration deviceConfiguration, Guid deviceUID)
        {
            try
            {
                return FiresecService.DeviceWriteConfiguration(deviceConfiguration, deviceUID);
            }
            catch
            {
                return CreateEmptyOperationResult<bool>();
            }
        }

        public OperationResult<bool> DeviceWriteAllConfiguration(FiresecAPI.Models.DeviceConfiguration deviceConfiguration)
        {
            try
            {
                return FiresecService.DeviceWriteAllConfiguration(deviceConfiguration);
            }
            catch
            {
                return CreateEmptyOperationResult<bool>();
            }
        }

        public OperationResult<bool> DeviceSetPassword(FiresecAPI.Models.DeviceConfiguration deviceConfiguration, Guid deviceUID, FiresecAPI.Models.DevicePasswordType devicePasswordType, string password)
        {
            try
            {
                return FiresecService.DeviceSetPassword(deviceConfiguration, deviceUID, devicePasswordType, password);
            }
            catch
            {
                return CreateEmptyOperationResult<bool>();
            }
        }

        public OperationResult<bool> DeviceDatetimeSync(FiresecAPI.Models.DeviceConfiguration deviceConfiguration, Guid deviceUID)
        {
            try
            {
                return FiresecService.DeviceDatetimeSync(deviceConfiguration, deviceUID);
            }
            catch
            {
                return CreateEmptyOperationResult<bool>();
            }
        }

        public OperationResult<string> DeviceGetInformation(FiresecAPI.Models.DeviceConfiguration deviceConfiguration, Guid deviceUID)
        {
            try
            {
                return FiresecService.DeviceGetInformation(deviceConfiguration, deviceUID);
            }
            catch
            {
                return CreateEmptyOperationResult<string>();
            }
        }

        public OperationResult<List<string>> DeviceGetSerialList(FiresecAPI.Models.DeviceConfiguration deviceConfiguration, Guid deviceUID)
        {
            try
            {
                return FiresecService.DeviceGetSerialList(deviceConfiguration, deviceUID);
            }
            catch
            {
                return CreateEmptyOperationResult<List<string>>();
            }
        }

        public OperationResult<string> DeviceUpdateFirmware(FiresecAPI.Models.DeviceConfiguration deviceConfiguration, Guid deviceUID, byte[] bytes, string fileName)
        {
            try
            {
                return FiresecService.DeviceUpdateFirmware(deviceConfiguration, deviceUID, bytes, fileName);
            }
            catch
            {
                return CreateEmptyOperationResult<string>();
            }
        }

        public OperationResult<string> DeviceVerifyFirmwareVersion(FiresecAPI.Models.DeviceConfiguration deviceConfiguration, Guid deviceUID, byte[] bytes, string fileName)
        {
            try
            {
                return FiresecService.DeviceVerifyFirmwareVersion(deviceConfiguration, deviceUID, bytes, fileName);
            }
            catch
            {
                return CreateEmptyOperationResult<string>();
            }
        }

        public OperationResult<string> DeviceReadEventLog(FiresecAPI.Models.DeviceConfiguration deviceConfiguration, Guid deviceUID)
        {
            try
            {
                return FiresecService.DeviceReadEventLog(deviceConfiguration, deviceUID);
            }
            catch
            {
                return CreateEmptyOperationResult<string>();
            }
        }

        public FiresecAPI.Models.DeviceConfiguration DeviceAutoDetectChildren(FiresecAPI.Models.DeviceConfiguration deviceConfiguration, Guid deviceUID, bool fastSearch)
        {
            try
            {
                return FiresecService.DeviceAutoDetectChildren(deviceConfiguration, deviceUID, fastSearch);
            }
            catch
            {
                return null;
            }
        }

        public FiresecAPI.Models.DeviceConfiguration DeviceReadConfiguration(FiresecAPI.Models.DeviceConfiguration deviceConfiguration, Guid deviceUID)
        {
            try
            {
                return FiresecService.DeviceReadConfiguration(deviceConfiguration, deviceUID);
            }
            catch
            {
                return null;
            }
        }

        public List<FiresecAPI.Models.DeviceCustomFunction> DeviceCustomFunctionList(Guid driverUID)
        {
            try
            {
                return FiresecService.DeviceCustomFunctionList(driverUID);
            }
            catch
            {
                return null;
            }
        }

        public OperationResult<string> DeviceCustomFunctionExecute(FiresecAPI.Models.DeviceConfiguration deviceConfiguration, Guid deviceUID, string functionName)
        {
            try
            {
                return FiresecService.DeviceCustomFunctionExecute(deviceConfiguration, deviceUID, functionName);
            }
            catch
            {
                return CreateEmptyOperationResult<string>();
            }
        }

        public OperationResult<string> DeviceGetGuardUsersList(FiresecAPI.Models.DeviceConfiguration deviceConfiguration, Guid deviceUID)
        {
            try
            {
                return FiresecService.DeviceGetGuardUsersList(deviceConfiguration, deviceUID);
            }
            catch
            {
                return CreateEmptyOperationResult<string>();
            }
        }

        public OperationResult<bool> DeviceSetGuardUsersList(FiresecAPI.Models.DeviceConfiguration deviceConfiguration, Guid deviceUID, string users)
        {
            try
            {
                return FiresecService.DeviceSetGuardUsersList(deviceConfiguration, deviceUID, users);
            }
            catch
            {
                return CreateEmptyOperationResult<bool>();
            }
        }

        public OperationResult<string> DeviceGetMDS5Data(FiresecAPI.Models.DeviceConfiguration deviceConfiguration, Guid deviceUID)
        {
            try
            {
                return FiresecService.DeviceGetMDS5Data(deviceConfiguration, deviceUID);
            }
            catch
            {
                return CreateEmptyOperationResult<string>();
            }
        }

        public FiresecAPI.Models.SystemConfiguration GetSystemConfiguration()
        {
            try
            {
                return FiresecService.GetSystemConfiguration();
            }
            catch
            {
                return null;
            }
        }

        public void SetSystemConfiguration(FiresecAPI.Models.SystemConfiguration systemConfiguration)
        {
            try
            {
                FiresecService.SetSystemConfiguration(systemConfiguration);
            }
            catch
            {
            }
        }

        public FiresecAPI.Models.LibraryConfiguration GetLibraryConfiguration()
        {
            try
            {
                return FiresecService.GetLibraryConfiguration();
            }
            catch
            {
                return null;
            }
        }

        public void SetLibraryConfiguration(FiresecAPI.Models.LibraryConfiguration libraryConfiguration)
        {
            try
            {
                FiresecService.SetLibraryConfiguration(libraryConfiguration);
            }
            catch
            {
            }
        }

        public FiresecAPI.Models.PlansConfiguration GetPlansConfiguration()
        {
            try
            {
                return FiresecService.GetPlansConfiguration();
            }
            catch
            {
                return null;
            }
        }

        public void SetPlansConfiguration(FiresecAPI.Models.PlansConfiguration plansConfiguration)
        {
            try
            {
                FiresecService.SetPlansConfiguration(plansConfiguration);
            }
            catch
            {
            }
        }

        public FiresecAPI.Models.SecurityConfiguration GetSecurityConfiguration()
        {
            try
            {
                return FiresecService.GetSecurityConfiguration();
            }
            catch
            {
                return null;
            }
        }

        public void SetSecurityConfiguration(FiresecAPI.Models.SecurityConfiguration securityConfiguration)
        {
            try
            {
                FiresecService.SetSecurityConfiguration(securityConfiguration);
            }
            catch
            {
            }
        }

        public FiresecAPI.Models.DeviceConfigurationStates GetStates()
        {
            try
            {
                return FiresecService.GetStates();
            }
            catch
            {
                return null;
            }
        }

        public List<FiresecAPI.Models.JournalRecord> ReadJournal(int startIndex, int count)
        {
            try
            {
                return FiresecService.ReadJournal(startIndex, count);
            }
            catch
            {
                return null;
            }
        }

        public IEnumerable<FiresecAPI.Models.JournalRecord> GetFilteredJournal(FiresecAPI.Models.JournalFilter journalFilter)
        {
            try
            {
                return FiresecService.GetFilteredJournal(journalFilter);
            }
            catch
            {
                return null;
            }
        }

        public IEnumerable<FiresecAPI.Models.JournalRecord> GetFilteredArchive(FiresecAPI.Models.ArchiveFilter archiveFilter)
        {
            try
            {
                return FiresecService.GetFilteredArchive(archiveFilter);
            }
            catch
            {
                return null;
            }
        }

        public IEnumerable<FiresecAPI.Models.JournalRecord> GetDistinctRecords()
        {
            try
            {
                return FiresecService.GetDistinctRecords();
            }
            catch
            {
                return null;
            }
        }

        public DateTime GetArchiveStartDate()
        {
            try
            {
                return FiresecService.GetArchiveStartDate();
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        public OperationResult<bool> AddToIgnoreList(List<Guid> deviceUIDs)
        {
            try
            {
                return FiresecService.AddToIgnoreList(deviceUIDs);
            }
            catch
            {
                return CreateEmptyOperationResult<bool>();
            }
        }

        public OperationResult<bool> RemoveFromIgnoreList(List<Guid> deviceUIDs)
        {
            try
            {
                return FiresecService.RemoveFromIgnoreList(deviceUIDs);
            }
            catch
            {
                return CreateEmptyOperationResult<bool>();
            }
        }

        public OperationResult<bool> ResetStates(List<FiresecAPI.Models.ResetItem> resetItems)
        {
            try
            {
                return FiresecService.ResetStates(resetItems);
            }
            catch
            {
                return CreateEmptyOperationResult<bool>();
            }
        }

        public OperationResult<bool> AddUserMessage(string message)
        {
            try
            {
                return FiresecService.AddUserMessage(message);
            }
            catch
            {
                return CreateEmptyOperationResult<bool>();
            };
        }

        public void AddJournalRecord(FiresecAPI.Models.JournalRecord journalRecord)
        {
            try
            {
                FiresecService.AddJournalRecord(journalRecord);
            }
            catch
            {
            };
        }

        public OperationResult<bool> ExecuteCommand(Guid deviceUID, string methodName)
        {
            try
            {
                return FiresecService.ExecuteCommand(deviceUID, methodName);
            }
            catch
            {
                return CreateEmptyOperationResult<bool>();
            };
        }

        public OperationResult<string> CheckHaspPresence()
        {
            try
            {
                return FiresecService.CheckHaspPresence();
            }
            catch
            {
                return CreateEmptyOperationResult<string>();
            };
        }

        public List<string> GetFileNamesList(string directory)
        {
            try
            {
                return FiresecService.GetFileNamesList(directory);
            }
            catch
            {
                return null;
            }
        }

        public Dictionary<string, string> GetDirectoryHash(string directory)
        {
            try
            {
                return FiresecService.GetDirectoryHash(directory);
            }
            catch
            {
                return null;
            }
        }

        public System.IO.Stream GetFile(string dirAndFileName)
        {
            try
            {
                return FiresecService.GetFile(dirAndFileName);
            }
            catch
            {
                return null;
            }
        }

        public void ConvertConfiguration()
        {
            try
            {
                FiresecService.ConvertConfiguration();
            }
            catch
            {
            };
        }

        public void ConvertJournal()
        {
            try
            {
                FiresecService.ConvertJournal();
            }
            catch
            {
            };
        }

        public string Ping()
        {
            try
            {
                return FiresecService.Ping();
            }
            catch
            {
                return null;
            }
        }

        public string Test()
        {
            try
            {
                return FiresecService.Test();
            }
            catch
            {
                return null;
            }
        }

        public void SetXDeviceConfiguration(XFiresecAPI.XDeviceConfiguration xDeviceConfiguration)
        {
            try
            {
                FiresecService.SetXDeviceConfiguration(xDeviceConfiguration);
            }
            catch
            {
            }
        }

        public XFiresecAPI.XDeviceConfiguration GetXDeviceConfiguration()
        {
            try
            {
                return FiresecService.GetXDeviceConfiguration();
            }
            catch
            {
                return null;
            }
        }
    }
}