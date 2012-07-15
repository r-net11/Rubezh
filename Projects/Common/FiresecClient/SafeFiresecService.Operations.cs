using System;
using System.Collections.Generic;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecAPI.Models.Skud;

namespace FiresecClient
{
	public partial class SafeFiresecService
	{
		public OperationResult<bool> Reconnect(string userName, string password)
		{
			return SafeOperationCall(() => { return FiresecService.Reconnect(userName, password); });
		}

		public void Disconnect()
		{
			Logger.Info("SafeFiresecService.Disconnect");
			SafeOperationCall(() => { FiresecService.Disconnect(); });
		}

		public string GetStatus()
		{
			return SafeOperationCall(() => { return FiresecService.GetStatus(); });
		}

		public void CancelProgress()
		{
			SafeOperationCall(() => { FiresecService.CancelProgress(); });
		}

		public List<Driver> GetDrivers()
		{
			return SafeOperationCall(() => { return FiresecService.GetDrivers(); });
		}

		public DeviceConfiguration GetDeviceConfiguration()
		{
			return SafeOperationCall(() => { return FiresecService.GetDeviceConfiguration(); });
		}

		public OperationResult<DeviceConfiguration> DeviceReadConfiguration(DeviceConfiguration deviceConfiguration, Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.DeviceReadConfiguration(deviceConfiguration, deviceUID); });
		}

		public OperationResult<DeviceConfiguration> DeviceAutoDetectChildren(DeviceConfiguration deviceConfiguration, Guid deviceUID, bool fastSearch)
		{
			return SafeOperationCall(() => { return FiresecService.DeviceAutoDetectChildren(deviceConfiguration, deviceUID, fastSearch); });
		}

		public OperationResult<List<DeviceCustomFunction>> DeviceCustomFunctionList(Guid driverUID)
		{
			return SafeOperationCall(() => { return FiresecService.DeviceCustomFunctionList(driverUID); });
		}

		public PlansConfiguration GetPlansConfiguration()
		{
			return SafeOperationCall(() => { return FiresecService.GetPlansConfiguration(); });
		}

		public void SetPlansConfiguration(PlansConfiguration plansConfiguration)
		{
			SafeOperationCall(() => { FiresecService.SetPlansConfiguration(plansConfiguration); });
		}

		public SystemConfiguration GetSystemConfiguration()
		{
			return SafeOperationCall(() => { return FiresecService.GetSystemConfiguration(); });
		}

		public void SetSystemConfiguration(SystemConfiguration systemConfiguration)
		{
			SafeOperationCall(() => { FiresecService.SetSystemConfiguration(systemConfiguration); });
		}

		public LibraryConfiguration GetLibraryConfiguration()
		{
			return SafeOperationCall(() => { return FiresecService.GetLibraryConfiguration(); });
		}

		public void SetLibraryConfiguration(LibraryConfiguration libraryConfiguration)
		{
			SafeOperationCall(() => { FiresecService.SetLibraryConfiguration(libraryConfiguration); });
		}

		public SecurityConfiguration GetSecurityConfiguration()
		{
			return SafeOperationCall(() => { return FiresecService.GetSecurityConfiguration(); });
		}

		public void SetSecurityConfiguration(SecurityConfiguration securityConfiguration)
		{
			SafeOperationCall(() => { FiresecService.SetSecurityConfiguration(securityConfiguration); });
		}

		public DeviceConfigurationStates GetStates(bool forceConvert = false)
		{
			return SafeOperationCall(() => { return FiresecService.GetStates(forceConvert); });
		}

		public OperationResult<List<JournalRecord>> GetFilteredJournal(JournalFilter journalFilter)
		{
			return SafeOperationCall(() => { return FiresecService.GetFilteredJournal(journalFilter); });
		}

		public OperationResult<List<JournalRecord>> GetFilteredArchive(ArchiveFilter archiveFilter)
		{
			return SafeOperationCall(() => { return FiresecService.GetFilteredArchive(archiveFilter); });
		}

		public void BeginGetFilteredArchive(ArchiveFilter archiveFilter)
		{
			SafeOperationCall(() => { FiresecService.BeginGetFilteredArchive(archiveFilter); });
		}

		public OperationResult<List<JournalDescriptionItem>> GetDistinctDescriptions()
		{
			return SafeOperationCall(() => { return FiresecService.GetDistinctDescriptions(); });
		}

		public OperationResult<DateTime> GetArchiveStartDate()
		{
			return SafeOperationCall(() => { return FiresecService.GetArchiveStartDate(); });
		}

		public void AddJournalRecord(JournalRecord journalRecord)
		{
			SafeOperationCall(() => { FiresecService.AddJournalRecord(journalRecord); });
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

		public OperationResult<bool> SetDeviceConfiguration(DeviceConfiguration deviceConfiguration)
		{
			return SafeOperationCall(() => { return FiresecService.SetDeviceConfiguration(deviceConfiguration); });
		}

		public OperationResult<bool> DeviceWriteConfiguration(DeviceConfiguration deviceConfiguration, Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.DeviceWriteConfiguration(deviceConfiguration, deviceUID); });
		}

		public OperationResult<bool> DeviceWriteAllConfiguration(DeviceConfiguration deviceConfiguration)
		{
			return SafeOperationCall(() => { return FiresecService.DeviceWriteAllConfiguration(deviceConfiguration); });
		}

		public OperationResult<bool> DeviceSetPassword(DeviceConfiguration deviceConfiguration, Guid deviceUID, DevicePasswordType devicePasswordType, string password)
		{
			return SafeOperationCall(() => { return FiresecService.DeviceSetPassword(deviceConfiguration, deviceUID, devicePasswordType, password); });
		}

		public OperationResult<bool> DeviceDatetimeSync(DeviceConfiguration deviceConfiguration, Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.DeviceDatetimeSync(deviceConfiguration, deviceUID); });
		}

		public OperationResult<string> DeviceGetInformation(DeviceConfiguration deviceConfiguration, Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.DeviceGetInformation(deviceConfiguration, deviceUID); });
		}

		public OperationResult<List<string>> DeviceGetSerialList(DeviceConfiguration deviceConfiguration, Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.DeviceGetSerialList(deviceConfiguration, deviceUID); });
		}

		public OperationResult<string> DeviceUpdateFirmware(DeviceConfiguration deviceConfiguration, Guid deviceUID, byte[] bytes, string fileName)
		{
			return SafeOperationCall(() => { return FiresecService.DeviceUpdateFirmware(deviceConfiguration, deviceUID, bytes, fileName); });
		}

		public OperationResult<string> DeviceVerifyFirmwareVersion(DeviceConfiguration deviceConfiguration, Guid deviceUID, byte[] bytes, string fileName)
		{
			return SafeOperationCall(() => { return FiresecService.DeviceVerifyFirmwareVersion(deviceConfiguration, deviceUID, bytes, fileName); });
		}

		public OperationResult<string> DeviceReadEventLog(DeviceConfiguration deviceConfiguration, Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.DeviceReadEventLog(deviceConfiguration, deviceUID); });
		}

		public OperationResult<string> DeviceCustomFunctionExecute(DeviceConfiguration deviceConfiguration, Guid deviceUID, string functionName)
		{
			return SafeOperationCall(() => { return FiresecService.DeviceCustomFunctionExecute(deviceConfiguration, deviceUID, functionName); });
		}

		public OperationResult<string> DeviceGetGuardUsersList(DeviceConfiguration deviceConfiguration, Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.DeviceGetGuardUsersList(deviceConfiguration, deviceUID); });
		}

		public OperationResult<bool> DeviceSetGuardUsersList(DeviceConfiguration deviceConfiguration, Guid deviceUID, string users)
		{
			return SafeOperationCall(() => { return FiresecService.DeviceSetGuardUsersList(deviceConfiguration, deviceUID, users); });
		}

		public OperationResult<string> DeviceGetMDS5Data(DeviceConfiguration deviceConfiguration, Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.DeviceGetMDS5Data(deviceConfiguration, deviceUID); });
		}

		public void AddToIgnoreList(List<Guid> deviceUIDs)
		{
			SafeOperationCall(() => { FiresecService.AddToIgnoreList(deviceUIDs); });
		}

		public void RemoveFromIgnoreList(List<Guid> deviceUIDs)
		{
			SafeOperationCall(() => { FiresecService.RemoveFromIgnoreList(deviceUIDs); });
		}

		public void ResetStates(List<ResetItem> resetItems)
		{
			SafeOperationCall(() => { FiresecService.ResetStates(resetItems); });
		}

		public void SetZoneGuard(ulong zoneNo)
		{
			SafeOperationCall(() => { FiresecService.SetZoneGuard(zoneNo); });
		}

		public void UnSetZoneGuard(ulong zoneNo)
		{
			SafeOperationCall(() => { FiresecService.UnSetZoneGuard(zoneNo); });
		}

		public void AddUserMessage(string message)
		{
			SafeOperationCall(() => { FiresecService.AddUserMessage(message); });
		}

		public OperationResult<bool> ExecuteCommand(Guid deviceUID, string methodName)
		{
			return SafeOperationCall(() => { return FiresecService.ExecuteCommand(deviceUID, methodName); });
		}

		public OperationResult<bool> CheckHaspPresence()
		{
			return SafeOperationCall(() => { return FiresecService.CheckHaspPresence(); });
		}

		public OperationResult<List<Property>> GetConfigurationParameters(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FiresecService.GetConfigurationParameters(deviceUID); });
		}

		public void SetConfigurationParameters(Guid deviceUID, List<Property> properties)
		{
			SafeOperationCall(() => { FiresecService.SetConfigurationParameters(deviceUID, properties); });
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

		public void OPCRefresh(DeviceConfiguration deviceConfiguration)
		{
			SafeOperationCall(() => { FiresecService.OPCRefresh(deviceConfiguration); });
		}

		public void OPCRegister()
		{
			SafeOperationCall(() => { FiresecService.OPCRegister(); });
		}

		public void OPCUnRegister()
		{
			SafeOperationCall(() => { FiresecService.OPCUnRegister(); });
		}
	}
}