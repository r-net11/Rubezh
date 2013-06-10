using System;
using System.Collections.Generic;
using System.ServiceModel;
using FiresecAPI;
using FiresecAPI.Models;

namespace FS2Api
{
	[ServiceContract]
	public interface IFS2Contract
	{
		#region Main
		[OperationContract]
		List<FS2Callbac> Poll(Guid clientUID);

		[OperationContract]
		void CancelPoll(Guid clientUID);

		[OperationContract]
		void CancelProgress();
		#endregion

		#region Common
		[OperationContract]
		OperationResult<DeviceConfiguration> GetDeviceConfiguration();

		[OperationContract]
		OperationResult<DriversConfiguration> GetDriversConfiguration();
		#endregion

		#region Monitor
		[OperationContract]
		OperationResult<List<DeviceState>> GetDeviceStates();

		[OperationContract]
		OperationResult<List<DeviceState>> GetDeviceParameters();

		[OperationContract]
		OperationResult AddToIgnoreList(List<Guid> deviceUIDs);

		[OperationContract]
		OperationResult RemoveFromIgnoreList(List<Guid> deviceUIDs);

		[OperationContract]
		OperationResult SetZoneGuard(Guid zoneUID);

		[OperationContract]
		OperationResult UnSetZoneGuard(Guid zoneUID);

		[OperationContract]
		OperationResult SetDeviceGuard(Guid deviceUID);

		[OperationContract]
		OperationResult UnSetDeviceGuard(Guid deviceUID);

		[OperationContract]
		OperationResult ResetStates(List<PaneleResetItem> paneleResetItems);

		[OperationContract]
		OperationResult ExecuteCommand(Guid deviceUID, string methodName);

		[OperationContract]
		OperationResult<bool> CheckHaspPresence();
		#endregion

		#region Administrator
		[OperationContract]
		OperationResult SetNewConfig(DeviceConfiguration deviceConfiguration);

		[OperationContract]
		OperationResult DeviceWriteConfig(Guid deviceUID, bool isUSB);

		[OperationContract]
		OperationResult DeviceSetPassword(Guid deviceUID, bool isUSB, DevicePasswordType devicePasswordType, string password);

		[OperationContract]
		OperationResult DeviceDatetimeSync(Guid deviceUID, bool isUSB);

		[OperationContract]
		OperationResult<string> DeviceGetInformation(Guid deviceUID, bool isUSB);

		[OperationContract]
		OperationResult<List<string>> DeviceGetSerialList(Guid deviceUID);

		[OperationContract]
		OperationResult<string> DeviceUpdateFirmware(Guid deviceUID, bool isUSB, string fileName);

		[OperationContract]
		OperationResult<string> DeviceVerifyFirmwareVersion(Guid deviceUID, bool isUSB, string fileName);

		[OperationContract]
		OperationResult<DeviceConfiguration> DeviceReadConfig(Guid deviceUID, bool isUSB);

		[OperationContract]
		OperationResult<List<FS2JournalItem>> DeviceReadEventLog(Guid deviceUID, bool isUSB);

		[OperationContract]
		OperationResult<DeviceConfiguration> DeviceAutoDetectChildren(Guid deviceUID, bool fastSearch);

		[OperationContract]
		OperationResult<List<DeviceCustomFunction>> DeviceCustomFunctionList(DriverType driverType);

		[OperationContract]
		OperationResult DeviceCustomFunctionExecute(Guid deviceUID, bool isUSB, string functionName);

		[OperationContract]
		OperationResult<string> DeviceGetGuardUsersList(Guid deviceUID);

		[OperationContract]
		OperationResult DeviceSetGuardUsersList(Guid deviceUID, string users);

		[OperationContract]
		OperationResult<string> DeviceGetMDS5Data(Guid deviceUID);

		[OperationContract]
		OperationResult SetConfigurationParameters(Guid deviceUID, List<Property> properties);

		[OperationContract]
		OperationResult<List<Property>> GetConfigurationParameters(Guid deviceUID);
		#endregion

		#region Journal
		[OperationContract]
		OperationResult<List<FS2JournalItem>> GetFilteredJournal(JournalFilter journalFilter);

		[OperationContract]
		OperationResult<List<FS2JournalItem>> GetFilteredArchive(ArchiveFilter archiveFilter);

		[OperationContract]
		OperationResult BeginGetFilteredArchive(ArchiveFilter archiveFilter);

		[OperationContract]
		OperationResult<List<JournalDescriptionItem>> GetDistinctDescriptions();

		[OperationContract]
		OperationResult<DateTime> GetArchiveStartDate();

		[OperationContract()]
		OperationResult AddJournalRecords(List<FS2JournalItem> journalItems);
		#endregion
	}
}