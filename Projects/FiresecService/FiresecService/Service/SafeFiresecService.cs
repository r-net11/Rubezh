using System;
using System.Collections.Generic;
using System.ServiceModel;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecAPI.Models.Skud;
using FiresecService.ViewModels;

namespace FiresecService.Service
{
	[ServiceBehavior(MaxItemsInObjectGraph = Int32.MaxValue, UseSynchronizationContext = true,
	InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple)]
	public class SafeFiresecService : IFiresecService
	{
		public FiresecService FiresecService { get; set; }

		public SafeFiresecService()
		{
			FiresecService = new FiresecService();
		}

		public void BeginOperation(string operationName)
		{
			MainViewModel.Current.BeginAddOperation(FiresecService.UID, OperationDirection.ClientToServer, operationName);
		}

		public void EndOperation()
		{
			MainViewModel.Current.EndAddOperation(FiresecService.UID, OperationDirection.ClientToServer);
		}

		public OperationResult<T> CreateEmptyOperationResult<T>(string message)
		{
			var operationResult = new OperationResult<T>
			{
				Result = default(T),
				HasError = true,
				Error = "Ошибка при выполнении операции на сервере" + "\n\r" + message
			};
			return operationResult;
		}

		OperationResult<T> SafeOperationCall<T>(Func<OperationResult<T>> func, string operationName)
		{
			try
			{
				ReconnectIfClientCallbackFaulted();
				BeginOperation(operationName);
				var result = func();
				EndOperation();
				return result;
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове SafeFiresecService.SafeOperationCall. operationName = " + operationName);
				return CreateEmptyOperationResult<T>(e.Message + "\n" + e.StackTrace);
			}
		}

		T SafeOperationCall<T>(Func<T> func, string operationName)
		{
			try
			{
				ReconnectIfClientCallbackFaulted();
				BeginOperation(operationName);
				var result = func();
				EndOperation();
				return result;
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове SafeFiresecService.SafeOperationCall. operationName = " + operationName);
			}
			return default(T);
		}

		void SafeOperationCall(Action action, string operationName)
		{
			try
			{
				ReconnectIfClientCallbackFaulted();
				BeginOperation(operationName);
				action();
				EndOperation();
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове SafeFiresecService.SafeOperationCall. operationName = " + operationName);
			}
		}

		void ReconnectIfClientCallbackFaulted()
		{
			if (FiresecService.IsClientCallbackFaulted)
			{
				FiresecService.ReconnectToClient();
			}
		}

		public OperationResult<bool> Connect(ClientCredentials clientCredentials, bool isNew)
		{
			return SafeOperationCall(() => { return FiresecService.Connect(clientCredentials, isNew); }, "Connect");
		}

		public OperationResult<bool> Reconnect(string userName, string password)
		{
			return SafeOperationCall(() => { return FiresecService.Reconnect(userName, password); }, "Reconnect");
		}

		public void Disconnect()
		{
			SafeOperationCall(() => { FiresecService.Disconnect(); }, "Disconnect");
		}

		public void CancelProgress()
		{
			SafeOperationCall(() => { FiresecService.CancelProgress(); }, "CancelProgress");
		}

		public string GetStatus()
		{
			return SafeOperationCall(() => { return FiresecService.GetStatus(); }, "GetStatus");
		}

		public List<FiresecAPI.Models.Driver> GetDrivers()
		{
			return SafeOperationCall(() => { return FiresecService.GetDrivers(); }, "GetDrivers");
		}

		public FiresecAPI.Models.DeviceConfiguration GetDeviceConfiguration()
		{
			return SafeOperationCall(() => { return FiresecService.GetDeviceConfiguration(); }, "GetDeviceConfiguration");
		}

		public OperationResult<bool> SetDeviceConfiguration(FiresecAPI.Models.DeviceConfiguration deviceConfiguration)
		{
			return SafeOperationCall(() => { return FiresecService.SetDeviceConfiguration(deviceConfiguration); }, "SetDeviceConfiguration");
		}

		public OperationResult<bool> DeviceWriteConfiguration(FiresecAPI.Models.DeviceConfiguration deviceConfiguration, Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.DeviceWriteConfiguration(deviceConfiguration, deviceUID); }, "DeviceWriteConfiguration");
		}

		public OperationResult<bool> DeviceWriteAllConfiguration(FiresecAPI.Models.DeviceConfiguration deviceConfiguration)
		{
			return SafeOperationCall(() => { return FiresecService.DeviceWriteAllConfiguration(deviceConfiguration); }, "DeviceWriteAllConfiguration");
		}

		public OperationResult<bool> DeviceSetPassword(FiresecAPI.Models.DeviceConfiguration deviceConfiguration, Guid deviceUID, FiresecAPI.Models.DevicePasswordType devicePasswordType, string password)
		{
			return SafeOperationCall(() => { return FiresecService.DeviceSetPassword(deviceConfiguration, deviceUID, devicePasswordType, password); }, "DeviceSetPassword");
		}

		public OperationResult<bool> DeviceDatetimeSync(FiresecAPI.Models.DeviceConfiguration deviceConfiguration, Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.DeviceDatetimeSync(deviceConfiguration, deviceUID); }, "DeviceDatetimeSync");
		}

		public OperationResult<string> DeviceGetInformation(FiresecAPI.Models.DeviceConfiguration deviceConfiguration, Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.DeviceGetInformation(deviceConfiguration, deviceUID); }, "DeviceGetInformation");
		}

		public OperationResult<List<string>> DeviceGetSerialList(FiresecAPI.Models.DeviceConfiguration deviceConfiguration, Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.DeviceGetSerialList(deviceConfiguration, deviceUID); }, "DeviceGetSerialList");
		}

		public OperationResult<string> DeviceUpdateFirmware(FiresecAPI.Models.DeviceConfiguration deviceConfiguration, Guid deviceUID, byte[] bytes, string fileName)
		{
			return SafeOperationCall(() => { return FiresecService.DeviceUpdateFirmware(deviceConfiguration, deviceUID, bytes, fileName); }, "DeviceUpdateFirmware");
		}

		public OperationResult<string> DeviceVerifyFirmwareVersion(FiresecAPI.Models.DeviceConfiguration deviceConfiguration, Guid deviceUID, byte[] bytes, string fileName)
		{
			return SafeOperationCall(() => { return FiresecService.DeviceVerifyFirmwareVersion(deviceConfiguration, deviceUID, bytes, fileName); }, "DeviceVerifyFirmwareVersion");
		}

		public OperationResult<string> DeviceReadEventLog(FiresecAPI.Models.DeviceConfiguration deviceConfiguration, Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.DeviceReadEventLog(deviceConfiguration, deviceUID); }, "DeviceReadEventLog");
		}

		public OperationResult<FiresecAPI.Models.DeviceConfiguration> DeviceAutoDetectChildren(FiresecAPI.Models.DeviceConfiguration deviceConfiguration, Guid deviceUID, bool fastSearch)
		{
			return SafeOperationCall(() => { return FiresecService.DeviceAutoDetectChildren(deviceConfiguration, deviceUID, fastSearch); }, "DeviceAutoDetectChildren");
		}

		public OperationResult<FiresecAPI.Models.DeviceConfiguration> DeviceReadConfiguration(FiresecAPI.Models.DeviceConfiguration deviceConfiguration, Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.DeviceReadConfiguration(deviceConfiguration, deviceUID); }, "DeviceReadConfiguration");
		}

		public OperationResult<List<FiresecAPI.Models.DeviceCustomFunction>> DeviceCustomFunctionList(Guid driverUID)
		{
			return SafeOperationCall(() => { return FiresecService.DeviceCustomFunctionList(driverUID); }, "DeviceCustomFunctionList");
		}

		public OperationResult<string> DeviceCustomFunctionExecute(FiresecAPI.Models.DeviceConfiguration deviceConfiguration, Guid deviceUID, string functionName)
		{
			return SafeOperationCall(() => { return FiresecService.DeviceCustomFunctionExecute(deviceConfiguration, deviceUID, functionName); }, "DeviceCustomFunctionExecute");
		}

		public OperationResult<string> DeviceGetGuardUsersList(FiresecAPI.Models.DeviceConfiguration deviceConfiguration, Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.DeviceGetGuardUsersList(deviceConfiguration, deviceUID); }, "DeviceGetGuardUsersList");
		}

		public OperationResult<bool> DeviceSetGuardUsersList(FiresecAPI.Models.DeviceConfiguration deviceConfiguration, Guid deviceUID, string users)
		{
			return SafeOperationCall(() => { return FiresecService.DeviceSetGuardUsersList(deviceConfiguration, deviceUID, users); }, "DeviceSetGuardUsersList");
		}

		public OperationResult<string> DeviceGetMDS5Data(FiresecAPI.Models.DeviceConfiguration deviceConfiguration, Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.DeviceGetMDS5Data(deviceConfiguration, deviceUID); }, "DeviceGetMDS5Data");
		}

		public FiresecAPI.Models.SystemConfiguration GetSystemConfiguration()
		{
			return SafeOperationCall(() => { return FiresecService.GetSystemConfiguration(); }, "GetSystemConfiguration");
		}

		public void SetSystemConfiguration(FiresecAPI.Models.SystemConfiguration systemConfiguration)
		{
			SafeOperationCall(() => { FiresecService.SetSystemConfiguration(systemConfiguration); }, "SetSystemConfiguration");
		}

		public FiresecAPI.Models.LibraryConfiguration GetLibraryConfiguration()
		{
			return SafeOperationCall(() => { return FiresecService.GetLibraryConfiguration(); }, "GetLibraryConfiguration");
		}

		public void SetLibraryConfiguration(FiresecAPI.Models.LibraryConfiguration libraryConfiguration)
		{
			SafeOperationCall(() => { FiresecService.SetLibraryConfiguration(libraryConfiguration); }, "SetLibraryConfiguration");
		}

		public FiresecAPI.Models.PlansConfiguration GetPlansConfiguration()
		{
			return SafeOperationCall(() => { return FiresecService.GetPlansConfiguration(); }, "GetPlansConfiguration");
		}

		public void SetPlansConfiguration(FiresecAPI.Models.PlansConfiguration plansConfiguration)
		{
			SafeOperationCall(() => { FiresecService.SetPlansConfiguration(plansConfiguration); }, "SetPlansConfiguration");
		}

		public FiresecAPI.Models.SecurityConfiguration GetSecurityConfiguration()
		{
			return SafeOperationCall(() => { return FiresecService.GetSecurityConfiguration(); }, "GetSecurityConfiguration");
		}

		public void SetSecurityConfiguration(FiresecAPI.Models.SecurityConfiguration securityConfiguration)
		{
			SafeOperationCall(() => { FiresecService.SetSecurityConfiguration(securityConfiguration); }, "SetSecurityConfiguration");
		}

		public FiresecAPI.Models.DeviceConfigurationStates GetStates(bool forceConvert = false)
		{
			return SafeOperationCall(() => { return FiresecService.GetStates(forceConvert); }, "GetStates");
		}

		public OperationResult<List<FiresecAPI.Models.JournalRecord>> GetFilteredJournal(FiresecAPI.Models.JournalFilter journalFilter)
		{
			return SafeOperationCall(() => { return FiresecService.GetFilteredJournal(journalFilter); }, "GetFilteredJournal");
		}

		public OperationResult<List<JournalRecord>> GetFilteredArchive(ArchiveFilter archiveFilter)
		{
			return SafeOperationCall(() => { return FiresecService.GetFilteredArchive(archiveFilter); }, "GetFilteredArchive");
		}

		public void BeginGetFilteredArchive(FiresecAPI.Models.ArchiveFilter archiveFilter)
		{
			SafeOperationCall(() => { FiresecService.BeginGetFilteredArchive(archiveFilter); }, "BeginGetFilteredArchive");
		}

		public OperationResult<List<FiresecAPI.Models.JournalDescriptionItem>> GetDistinctDescriptions()
		{
			return SafeOperationCall(() => { return FiresecService.GetDistinctDescriptions(); }, "GetDistinctDescriptions");
		}

		public OperationResult<DateTime> GetArchiveStartDate()
		{
			return SafeOperationCall(() => { return FiresecService.GetArchiveStartDate(); }, "GetArchiveStartDate");
		}

		public void AddToIgnoreList(List<Guid> deviceUIDs)
		{
			SafeOperationCall(() => { FiresecService.AddToIgnoreList(deviceUIDs); }, "AddToIgnoreList");
		}

		public void RemoveFromIgnoreList(List<Guid> deviceUIDs)
		{
			SafeOperationCall(() => { FiresecService.RemoveFromIgnoreList(deviceUIDs); }, "RemoveFromIgnoreList");
		}

		public void ResetStates(List<FiresecAPI.Models.ResetItem> resetItems)
		{
			SafeOperationCall(() => { FiresecService.ResetStates(resetItems); }, "ResetStates");
		}

		public void SetZoneGuard(Guid secPanelUID, int localZoneNo)
		{
			SafeOperationCall(() => { FiresecService.SetZoneGuard(secPanelUID, localZoneNo); }, "SetZoneGuard");
		}

		public void UnSetZoneGuard(Guid secPanelUID, int localZoneNo)
		{
			SafeOperationCall(() => { FiresecService.UnSetZoneGuard(secPanelUID, localZoneNo); }, "UnSetZoneGuard");
		}

		public void AddUserMessage(string message)
		{
			SafeOperationCall(() => { FiresecService.AddUserMessage(message); }, "AddUserMessage");
		}

		public void AddJournalRecord(FiresecAPI.Models.JournalRecord journalRecord)
		{
			SafeOperationCall(() => { FiresecService.AddJournalRecord(journalRecord); }, "AddJournalRecord");
		}

		public OperationResult<bool> ExecuteCommand(Guid deviceUID, string methodName)
		{
			return SafeOperationCall(() => { return FiresecService.ExecuteCommand(deviceUID, methodName); }, "ExecuteCommand");
		}

		public OperationResult<bool> CheckHaspPresence()
		{
			return SafeOperationCall(() => { return FiresecService.CheckHaspPresence(); }, "CheckHaspPresence");
		}

		public OperationResult<List<Property>> GetConfigurationParameters(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.GetConfigurationParameters(deviceUID); }, "GetConfigurationParameters");
		}

		public void SetConfigurationParameters(Guid deviceUID, List<Property> properties)
		{
			SafeOperationCall(() => { FiresecService.SetConfigurationParameters(deviceUID, properties); }, "SetConfigurationParameters");
		}

		public List<string> GetFileNamesList(string directory)
		{
			return SafeOperationCall(() => { return FiresecService.GetFileNamesList(directory); }, "GetFileNamesList");
		}

		public Dictionary<string, string> GetDirectoryHash(string directory)
		{
			return SafeOperationCall(() => { return FiresecService.GetDirectoryHash(directory); }, "GetDirectoryHash");
		}

		public System.IO.Stream GetFile(string dirAndFileName)
		{
			return SafeOperationCall(() => { return FiresecService.GetFile(dirAndFileName); }, "GetFile");
		}

		public void ConvertConfiguration()
		{
			SafeOperationCall(() => { FiresecService.ConvertConfiguration(); }, "ConvertConfiguration");
		}

		public void ConvertJournal()
		{
			SafeOperationCall(() => { FiresecService.ConvertJournal(); }, "ConvertJournal");
		}

		public string Ping()
		{
			return SafeOperationCall(() => { return FiresecService.Ping(); }, "Ping");
		}

		public string Test()
		{
#if (DEBUG)
			//throw new Exception("Test");
#endif
			return SafeOperationCall(() => { return FiresecService.Test(); }, "Test");
		}

		public void SetXDeviceConfiguration(XFiresecAPI.XDeviceConfiguration xDeviceConfiguration)
		{
			SafeOperationCall(() => { FiresecService.SetXDeviceConfiguration(xDeviceConfiguration); }, "SetXDeviceConfiguration");
		}

		public XFiresecAPI.XDeviceConfiguration GetXDeviceConfiguration()
		{
			return SafeOperationCall(() => { return FiresecService.GetXDeviceConfiguration(); }, "GetXDeviceConfiguration");
		}

		public IEnumerable<EmployeeCard> GetEmployees(EmployeeCardIndexFilter filter)
		{
			return SafeContext.Execute<IEnumerable<EmployeeCard>>(() => FiresecService.GetEmployees(filter));
		}
		public bool DeleteEmployee(int id)
		{
			return SafeContext.Execute<bool>(() => FiresecService.DeleteEmployee(id));
		}
		public EmployeeCardDetails GetEmployeeCard(int id)
		{
			return SafeContext.Execute<EmployeeCardDetails>(() => FiresecService.GetEmployeeCard(id));
		}
		public int SaveEmployeeCard(EmployeeCardDetails employeeCard)
		{
			return SafeContext.Execute<int>(() => FiresecService.SaveEmployeeCard(employeeCard));
		}
		public IEnumerable<EmployeeDepartment> GetEmployeeDepartments()
		{
			return SafeContext.Execute<IEnumerable<EmployeeDepartment>>(() => FiresecService.GetEmployeeDepartments());
		}
		public IEnumerable<EmployeeGroup> GetEmployeeGroups()
		{
			return SafeContext.Execute<IEnumerable<EmployeeGroup>>(() => FiresecService.GetEmployeeGroups());
		}
		public IEnumerable<EmployeePosition> GetEmployeePositions()
		{
			return SafeContext.Execute<IEnumerable<EmployeePosition>>(() => FiresecService.GetEmployeePositions());
		}

		public void OPCRefresh()
		{
			SafeOperationCall(() => { FiresecService.OPCRefresh(); }, "OPCRefresh");
		}

		public void OPCRegister()
			{
				SafeOperationCall(() => { FiresecService.OPCRegister(); }, "OPCRegister");
			}

		public void OPCUnRegister()
		{
			SafeOperationCall(() => { FiresecService.OPCUnRegister(); }, "OPCUnRegister");
		}
	}
}