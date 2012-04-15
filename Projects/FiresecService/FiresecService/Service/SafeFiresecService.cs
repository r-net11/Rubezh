using System;
using System.Collections.Generic;
using System.ServiceModel;
using FiresecAPI;
using FiresecAPI.Models.Skud;
using Common;

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
				Error = "Ошибка при выполнении операции на сервере"
			};
			return operationResult;
		}

		OperationResult<T> SafeOperationCall<T>(Func<OperationResult<T>> func)
		{
			try
			{
                var result = func();
                return result;
			}
			catch(Exception e)
			{
                Logger.Error(e);
			}
			return CreateEmptyOperationResult<T>();
		}

		T SafeOperationCall<T>(Func<T> func)
		{
			try
			{
				return func();
			}
            catch (Exception e)
            {
                Logger.Error(e);
            }
			return default(T);
		}

		void SafeOperationCall(Action action)
		{
			try
			{
				action();
			}
			catch (Exception e)
			{
                Logger.Error(e);
			}
		}

        public OperationResult<bool> Connect(string clientType, string clientCallbackAddress, string userName, string password)
		{
			return SafeOperationCall(() => { return FiresecService.Connect(clientType, clientCallbackAddress, userName, password); });
		}

        public OperationResult<bool> Reconnect(string userName, string password)
		{
			return SafeOperationCall(() => { return FiresecService.Reconnect(userName, password); });
		}

		public void Disconnect()
		{
			SafeOperationCall(() => { FiresecService.Disconnect(); });
		}

		public void Subscribe()
		{
			SafeOperationCall(() => { FiresecService.Subscribe(); });
		}

		public void CancelProgress()
		{
			SafeOperationCall(() => { FiresecService.CancelProgress(); });
		}

        public string GetStatus()
		{
            return SafeOperationCall(() => { return FiresecService.GetStatus(); });
		}

		public List<FiresecAPI.Models.Driver> GetDrivers()
		{
			return SafeOperationCall(() => { return FiresecService.GetDrivers(); });
		}

		public FiresecAPI.Models.DeviceConfiguration GetDeviceConfiguration()
		{
			return SafeOperationCall(() => { return FiresecService.GetDeviceConfiguration(); });
		}

		public OperationResult<bool> SetDeviceConfiguration(FiresecAPI.Models.DeviceConfiguration deviceConfiguration)
		{
			return SafeOperationCall(() => { return FiresecService.SetDeviceConfiguration(deviceConfiguration); });
		}

		public OperationResult<bool> DeviceWriteConfiguration(FiresecAPI.Models.DeviceConfiguration deviceConfiguration, Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.DeviceWriteConfiguration(deviceConfiguration, deviceUID); });
		}

		public OperationResult<bool> DeviceWriteAllConfiguration(FiresecAPI.Models.DeviceConfiguration deviceConfiguration)
		{
			return SafeOperationCall(() => { return FiresecService.DeviceWriteAllConfiguration(deviceConfiguration); });
		}

		public OperationResult<bool> DeviceSetPassword(FiresecAPI.Models.DeviceConfiguration deviceConfiguration, Guid deviceUID, FiresecAPI.Models.DevicePasswordType devicePasswordType, string password)
		{
			return SafeOperationCall(() => { return FiresecService.DeviceSetPassword(deviceConfiguration, deviceUID, devicePasswordType, password); });
		}

		public OperationResult<bool> DeviceDatetimeSync(FiresecAPI.Models.DeviceConfiguration deviceConfiguration, Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.DeviceDatetimeSync(deviceConfiguration, deviceUID); });
		}

		public OperationResult<string> DeviceGetInformation(FiresecAPI.Models.DeviceConfiguration deviceConfiguration, Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.DeviceGetInformation(deviceConfiguration, deviceUID); });
		}

		public OperationResult<List<string>> DeviceGetSerialList(FiresecAPI.Models.DeviceConfiguration deviceConfiguration, Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.DeviceGetSerialList(deviceConfiguration, deviceUID); });
		}

		public OperationResult<string> DeviceUpdateFirmware(FiresecAPI.Models.DeviceConfiguration deviceConfiguration, Guid deviceUID, byte[] bytes, string fileName)
		{
			return SafeOperationCall(() => { return FiresecService.DeviceUpdateFirmware(deviceConfiguration, deviceUID, bytes, fileName); });
		}

		public OperationResult<string> DeviceVerifyFirmwareVersion(FiresecAPI.Models.DeviceConfiguration deviceConfiguration, Guid deviceUID, byte[] bytes, string fileName)
		{
			return SafeOperationCall(() => { return FiresecService.DeviceVerifyFirmwareVersion(deviceConfiguration, deviceUID, bytes, fileName); });
		}

		public OperationResult<string> DeviceReadEventLog(FiresecAPI.Models.DeviceConfiguration deviceConfiguration, Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.DeviceReadEventLog(deviceConfiguration, deviceUID); });
		}

		public OperationResult<FiresecAPI.Models.DeviceConfiguration> DeviceAutoDetectChildren(FiresecAPI.Models.DeviceConfiguration deviceConfiguration, Guid deviceUID, bool fastSearch)
		{
			return SafeOperationCall(() => { return FiresecService.DeviceAutoDetectChildren(deviceConfiguration, deviceUID, fastSearch); });
		}

		public OperationResult<FiresecAPI.Models.DeviceConfiguration> DeviceReadConfiguration(FiresecAPI.Models.DeviceConfiguration deviceConfiguration, Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.DeviceReadConfiguration(deviceConfiguration, deviceUID); });
		}

		public OperationResult<List<FiresecAPI.Models.DeviceCustomFunction>> DeviceCustomFunctionList(Guid driverUID)
		{
			return SafeOperationCall(() => { return FiresecService.DeviceCustomFunctionList(driverUID); });
		}

		public OperationResult<string> DeviceCustomFunctionExecute(FiresecAPI.Models.DeviceConfiguration deviceConfiguration, Guid deviceUID, string functionName)
		{
			return SafeOperationCall(() => { return FiresecService.DeviceCustomFunctionExecute(deviceConfiguration, deviceUID, functionName); });
		}

		public OperationResult<string> DeviceGetGuardUsersList(FiresecAPI.Models.DeviceConfiguration deviceConfiguration, Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.DeviceGetGuardUsersList(deviceConfiguration, deviceUID); });
		}

		public OperationResult<bool> DeviceSetGuardUsersList(FiresecAPI.Models.DeviceConfiguration deviceConfiguration, Guid deviceUID, string users)
		{
			return SafeOperationCall(() => { return FiresecService.DeviceSetGuardUsersList(deviceConfiguration, deviceUID, users); });
		}

		public OperationResult<string> DeviceGetMDS5Data(FiresecAPI.Models.DeviceConfiguration deviceConfiguration, Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.DeviceGetMDS5Data(deviceConfiguration, deviceUID); });
		}

		public FiresecAPI.Models.SystemConfiguration GetSystemConfiguration()
		{
			return SafeOperationCall(() => { return FiresecService.GetSystemConfiguration(); });
		}

		public void SetSystemConfiguration(FiresecAPI.Models.SystemConfiguration systemConfiguration)
		{
			SafeOperationCall(() => { FiresecService.SetSystemConfiguration(systemConfiguration); });
		}

		public FiresecAPI.Models.LibraryConfiguration GetLibraryConfiguration()
		{
			return SafeOperationCall(() => { return FiresecService.GetLibraryConfiguration(); });
		}

		public void SetLibraryConfiguration(FiresecAPI.Models.LibraryConfiguration libraryConfiguration)
		{
			SafeOperationCall(() => { FiresecService.SetLibraryConfiguration(libraryConfiguration); });
		}

		public FiresecAPI.Models.PlansConfiguration GetPlansConfiguration()
		{
			return SafeOperationCall(() => { return FiresecService.GetPlansConfiguration(); });
		}

		public void SetPlansConfiguration(FiresecAPI.Models.PlansConfiguration plansConfiguration)
		{
			SafeOperationCall(() => { FiresecService.SetPlansConfiguration(plansConfiguration); });
		}

		public FiresecAPI.Models.SecurityConfiguration GetSecurityConfiguration()
		{
			return SafeOperationCall(() => { return FiresecService.GetSecurityConfiguration(); });
		}

		public void SetSecurityConfiguration(FiresecAPI.Models.SecurityConfiguration securityConfiguration)
		{
			SafeOperationCall(() => { FiresecService.SetSecurityConfiguration(securityConfiguration); });
		}

		public FiresecAPI.Models.DeviceConfigurationStates GetStates()
		{
			return SafeOperationCall(() => { return FiresecService.GetStates(); });
		}

		public OperationResult<List<FiresecAPI.Models.JournalRecord>> ReadJournal(int startIndex, int count)
		{
			return SafeOperationCall(() => { return FiresecService.ReadJournal(startIndex, count); });
		}

		public OperationResult<List<FiresecAPI.Models.JournalRecord>> GetFilteredJournal(FiresecAPI.Models.JournalFilter journalFilter)
		{
			return SafeOperationCall(() => { return FiresecService.GetFilteredJournal(journalFilter); });
		}

		public OperationResult<List<FiresecAPI.Models.JournalRecord>> GetFilteredArchive(FiresecAPI.Models.ArchiveFilter archiveFilter)
		{
			return SafeOperationCall(() => { return FiresecService.GetFilteredArchive(archiveFilter); });
		}

		public OperationResult<List<FiresecAPI.Models.JournalRecord>> GetDistinctRecords()
		{
			return SafeOperationCall(() => { return FiresecService.GetDistinctRecords(); });
		}

		public OperationResult<DateTime> GetArchiveStartDate()
		{
			return SafeOperationCall(() => { return FiresecService.GetArchiveStartDate(); });
		}

		public OperationResult<bool> AddToIgnoreList(List<Guid> deviceUIDs)
		{
			return SafeOperationCall(() => { return FiresecService.AddToIgnoreList(deviceUIDs); });
		}

		public OperationResult<bool> RemoveFromIgnoreList(List<Guid> deviceUIDs)
		{
			return SafeOperationCall(() => { return FiresecService.RemoveFromIgnoreList(deviceUIDs); });
		}

		public OperationResult<bool> ResetStates(List<FiresecAPI.Models.ResetItem> resetItems)
		{
			return SafeOperationCall(() => { return FiresecService.ResetStates(resetItems); });
		}

		public OperationResult<bool> AddUserMessage(string message)
		{
			return SafeOperationCall(() => { return FiresecService.AddUserMessage(message); });
		}

		public OperationResult<bool> AddJournalRecord(FiresecAPI.Models.JournalRecord journalRecord)
		{
			return SafeOperationCall(() => { return FiresecService.AddJournalRecord(journalRecord); });
		}

		public OperationResult<bool> ExecuteCommand(Guid deviceUID, string methodName)
		{
			return SafeOperationCall(() => { return FiresecService.ExecuteCommand(deviceUID, methodName); });
		}

		public OperationResult<bool> CheckHaspPresence()
		{
			return SafeOperationCall(() => { return FiresecService.CheckHaspPresence(); });
		}

		public List<string> GetFileNamesList(string directory)
		{
			return SafeOperationCall(() => { return FiresecService.GetFileNamesList(directory); });
		}

		public Dictionary<string, string> GetDirectoryHash(string directory)
		{
			return SafeOperationCall(() => { return FiresecService.GetDirectoryHash(directory); });
		}

		public System.IO.Stream GetFile(string dirAndFileName)
		{
			return SafeOperationCall(() => { return FiresecService.GetFile(dirAndFileName); });
		}

		public void ConvertConfiguration()
		{
			SafeOperationCall(() => { FiresecService.ConvertConfiguration(); });
		}

		public void ConvertJournal()
		{
			SafeOperationCall(() => { FiresecService.ConvertJournal(); });
		}

		public string Ping()
		{
			return SafeOperationCall(() => { return FiresecService.Ping(); });
		}

		public string Test()
		{
			return SafeOperationCall(() => { return FiresecService.Test(); });
		}

		public void SetXDeviceConfiguration(XFiresecAPI.XDeviceConfiguration xDeviceConfiguration)
		{
			SafeOperationCall(() => { FiresecService.SetXDeviceConfiguration(xDeviceConfiguration); });
		}

		public XFiresecAPI.XDeviceConfiguration GetXDeviceConfiguration()
		{
			return SafeOperationCall(() => { return FiresecService.GetXDeviceConfiguration(); });
		}

		public IEnumerable<EmployeeCard> GetEmployees()
		{
			return SafeContext.Execute<IEnumerable<EmployeeCard>>(() => FiresecService.GetEmployees());
		}
	}
}