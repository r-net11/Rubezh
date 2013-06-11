using System;
using System.Collections.Generic;
using FiresecAPI.Models;
using FS2Api;
using FiresecAPI;

namespace FS2Client
{
	public partial class FS2ClientContract
	{
		#region Main
		public List<FS2Callbac> Poll(Guid clientUID)
		{
			return SafeOperationCall(() => { return FS2Contract.Poll(clientUID); }, "Poll");
		}

		public void CancelPoll(Guid clientUID)
		{
			SafeOperationCall(() => { FS2Contract.CancelPoll(clientUID); }, "CancelPoll");
		}

		public void CancelProgress()
		{
			SafeOperationCall(() => { FS2Contract.CancelProgress(); }, "CanceProgress");
		}
		#endregion

		#region Common
		public OperationResult<DeviceConfiguration> GetDeviceConfiguration()
		{
			return SafeOperationCall(() => { return FS2Contract.GetDeviceConfiguration(); }, "GetDeviceConfiguration");
		}

		public OperationResult<DriversConfiguration> GetDriversConfiguration()
		{
			return SafeOperationCall(() => { return FS2Contract.GetDriversConfiguration(); }, "GetDriversConfiguration");
		}
		#endregion

		#region Monitor
		public OperationResult<List<DeviceState>> GetDeviceStates()
		{
			return SafeOperationCall(() => { return FS2Contract.GetDeviceStates(); }, "GetDeviceStates");
		}

		public OperationResult<List<DeviceState>> GetDeviceParameters()
		{
			return SafeOperationCall(() => { return FS2Contract.GetDeviceParameters(); }, "GetDeviceParameters");
		}

		public OperationResult<List<ZoneState>> GetZoneStates()
		{
			return SafeOperationCall(() => { return FS2Contract.GetZoneStates(); }, "GetZoneStates");
		}

		public OperationResult AddToIgnoreList(List<Guid> deviceUIDs)
		{
			return SafeOperationCall(() => { return FS2Contract.AddToIgnoreList(deviceUIDs); }, "AddToIgnoreList");
		}

		public OperationResult RemoveFromIgnoreList(List<Guid> deviceUIDs)
		{
			return SafeOperationCall(() => { return FS2Contract.RemoveFromIgnoreList(deviceUIDs); }, "RemoveFromIgnoreList");
		}

		public OperationResult SetZoneGuard(Guid zoneUID)
		{
			return SafeOperationCall(() => { return FS2Contract.SetZoneGuard(zoneUID); }, "SetZoneGuard");
		}

		public OperationResult UnSetZoneGuard(Guid zoneUID)
		{
			return SafeOperationCall(() => { return FS2Contract.UnSetZoneGuard(zoneUID); }, "UnSetZoneGuard");
		}

		public OperationResult SetDeviceGuard(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FS2Contract.SetDeviceGuard(deviceUID); }, "SetDeviceGuard");
		}

		public OperationResult UnSetDeviceGuard(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FS2Contract.UnSetDeviceGuard(deviceUID); }, "UnSetDeviceGuard");
		}

		public OperationResult ResetStates(List<PanelResetItem> panelResetItems)
		{
			return SafeOperationCall(() => { return FS2Contract.ResetStates(panelResetItems); }, "ResetStates");
		}

		public OperationResult ExecuteCommand(Guid deviceUID, string methodName)
		{
			return SafeOperationCall(() => { return FS2Contract.ExecuteCommand(deviceUID, methodName); }, "ExecuteCommand");
		}

		public OperationResult<bool> CheckHaspPresence()
		{
			return SafeOperationCall<bool>(() => { return FS2Contract.CheckHaspPresence(); }, "CheckHaspPresence");
		}
		#endregion

		#region Administrator
		public OperationResult SetNewConfig(FiresecAPI.Models.DeviceConfiguration deviceConfiguration)
		{
			return SafeOperationCall(() => { return FS2Contract.SetNewConfig(deviceConfiguration); }, "SetNewConfig");
		}

		public OperationResult DeviceWriteConfig(Guid deviceUID, bool isUSB)
		{
			return SafeOperationCall(() => { return FS2Contract.DeviceWriteConfig(deviceUID, isUSB); }, "DeviceWriteConfig");
		}

		public OperationResult DeviceSetPassword(Guid deviceUID, bool isUSB, DevicePasswordType devicePasswordType, string password)
		{
			return SafeOperationCall(() => { return FS2Contract.DeviceSetPassword(deviceUID, isUSB, devicePasswordType, password); }, "DeviceSetPassword");
		}

		public OperationResult DeviceDatetimeSync(Guid deviceUID, bool isUSB)
		{
			return SafeOperationCall(() => { return FS2Contract.DeviceDatetimeSync(deviceUID, isUSB); }, "DeviceDatetimeSync");
		}

		public OperationResult<string> DeviceGetInformation(Guid deviceUID, bool isUSB)
		{
			return SafeOperationCall(() => { return FS2Contract.DeviceGetInformation(deviceUID, isUSB); }, "DeviceGetInformation");
		}

		public OperationResult<List<string>> DeviceGetSerialList(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FS2Contract.DeviceGetSerialList(deviceUID); }, "DeviceGetSerialList");
		}

		public OperationResult<string> DeviceUpdateFirmware(Guid deviceUID, bool isUSB, string fileName)
		{
			return SafeOperationCall(() => { return FS2Contract.DeviceUpdateFirmware(deviceUID, isUSB, fileName); }, "DeviceUpdateFirmware");
		}

		public OperationResult<string> DeviceVerifyFirmwareVersion(Guid deviceUID, bool isUSB, string fileName)
		{
			return SafeOperationCall(() => { return FS2Contract.DeviceVerifyFirmwareVersion(deviceUID, isUSB, fileName); }, "DeviceVerifyFirmwareVersion");
		}

		public OperationResult<DeviceConfiguration> DeviceReadConfig(Guid deviceUID, bool isUSB)
		{
			return SafeOperationCall(() => { return FS2Contract.DeviceReadConfig(deviceUID, isUSB); }, "DeviceReadConfig");
		}

		public OperationResult<List<FS2JournalItem>> DeviceReadEventLog(Guid deviceUID, bool isUSB)
		{
			return SafeOperationCall(() => { return FS2Contract.DeviceReadEventLog(deviceUID, isUSB); }, "DeviceReadEventLog");
		}

		public OperationResult<DeviceConfiguration> DeviceAutoDetectChildren(Guid deviceUID, bool fastSearch)
		{
			return SafeOperationCall(() => { return FS2Contract.DeviceAutoDetectChildren(deviceUID, fastSearch); }, "DeviceAutoDetectChildren");
		}

		public OperationResult<List<DeviceCustomFunction>> DeviceCustomFunctionList(DriverType driverType)
		{
			return SafeOperationCall(() => { return FS2Contract.DeviceCustomFunctionList(driverType); }, "DeviceCustomFunctionList");
		}

		public OperationResult DeviceCustomFunctionExecute(Guid deviceUID, bool isUSB, string functionName)
		{
			return SafeOperationCall(() => { return FS2Contract.DeviceCustomFunctionExecute(deviceUID, isUSB, functionName); }, "DeviceCustomFunctionExecute");
		}

		public OperationResult<string> DeviceGetGuardUsersList(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FS2Contract.DeviceGetGuardUsersList(deviceUID); }, "DeviceGetGuardUsersList");
		}

		public OperationResult DeviceSetGuardUsersList(Guid deviceUID, string users)
		{
			return SafeOperationCall(() => { return FS2Contract.DeviceSetGuardUsersList(deviceUID, users); }, "DeviceSetGuardUsersList");
		}

		public OperationResult<string> DeviceGetMDS5Data(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FS2Contract.DeviceGetMDS5Data(deviceUID); }, "DeviceGetMDS5Data");
		}

		public OperationResult SetConfigurationParameters(Guid deviceUID, List<Property> properties)
		{
			return SafeOperationCall(() => { return FS2Contract.SetConfigurationParameters(deviceUID, properties); }, "SetConfigurationParameters");
		}

		public OperationResult<List<Property>> GetConfigurationParameters(Guid deviceUID)
		{
			return SafeOperationCall(() => { return FS2Contract.GetConfigurationParameters(deviceUID); }, "GetConfigurationParameters");
		}
		#endregion

		#region Journal
		public OperationResult<List<FS2JournalItem>> GetFilteredJournal(JournalFilter journalFilter)
		{
			return SafeOperationCall(() => { return FS2Contract.GetFilteredJournal(journalFilter); }, "GetFilteredJournal");
		}

		public OperationResult<List<FS2JournalItem>> GetFilteredArchive(ArchiveFilter archiveFilter)
		{
			return SafeOperationCall(() => { return FS2Contract.GetFilteredArchive(archiveFilter); }, "GetFilteredArchive");
		}

		public OperationResult BeginGetFilteredArchive(ArchiveFilter archiveFilter)
		{
			return SafeOperationCall(() => { return FS2Contract.BeginGetFilteredArchive(archiveFilter); }, "BeginGetFilteredArchive");
		}

		public OperationResult<List<JournalDescriptionItem>> GetDistinctDescriptions()
		{
			return SafeOperationCall(() => { return FS2Contract.GetDistinctDescriptions(); }, "GetDistinctDescriptions");
		}

		public OperationResult<DateTime> GetArchiveStartDate()
		{
			return SafeOperationCall(() => { return FS2Contract.GetArchiveStartDate(); }, "GetArchiveStartDate");
		}

		public OperationResult AddJournalRecords(List<FS2JournalItem> journalItems)
		{
			return SafeOperationCall(() => { return FS2Contract.AddJournalRecords(journalItems); }, "AddJournalRecords");
		}
		#endregion
	}
}