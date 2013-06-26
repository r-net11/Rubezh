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
		OperationResult<List<ZoneState>> GetZoneStates();

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
		OperationResult ResetStates(List<PanelResetItem> panelResetItems);

		[OperationContract]
		OperationResult ExecuteCommand(Guid deviceUID, string methodName);

		[OperationContract]
		OperationResult<bool> CheckHaspPresence();
		#endregion

		#region Administrator
		[OperationContract]
		OperationResult SetNewConfiguration(DeviceConfiguration deviceConfiguration);

		[OperationContract]
		OperationResult DeviceWriteConfiguration(Guid deviceUID, bool isUSB);

		[OperationContract]
		OperationResult DeviceWriteAllConfiguration();

		[OperationContract]
		OperationResult DeviceSetPassword(Guid deviceUID, bool isUSB, DevicePasswordType devicePasswordType, string password);

		[OperationContract]
		OperationResult DeviceDatetimeSync(Guid deviceUID, bool isUSB);

		[OperationContract]
		OperationResult<string> DeviceGetInformation(Guid deviceUID, bool isUSB);

		[OperationContract]
		OperationResult<List<string>> DeviceGetSerialList(Guid deviceUID);

		[OperationContract]
		OperationResult<string> DeviceVerifyFirmwareVersion(Guid deviceUID, bool isUSB, string fileName);

		[OperationContract]
		OperationResult DeviceUpdateFirmware(Guid deviceUID, bool isUSB, string fileName);

		[OperationContract]
		OperationResult<DeviceConfiguration> DeviceReadConfiguration(Guid deviceUID, bool isUSB);

		[OperationContract]
		OperationResult<FS2JournalItemsCollection> DeviceReadJournal(Guid deviceUID, bool isUSB);

		[OperationContract]
		OperationResult<DeviceConfiguration> DeviceAutoDetectChildren(Guid deviceUID, bool fastSearch);

		[OperationContract]
		OperationResult<List<DeviceCustomFunction>> DeviceGetCustomFunctions(DriverType driverType);

		[OperationContract]
		OperationResult DeviceExecuteCustomFunction(Guid deviceUID, bool isUSB, string functionName);

		[OperationContract]
		OperationResult<List<GuardUser>> DeviceGetGuardUsers(Guid deviceUID);

		[OperationContract]
		OperationResult DeviceSetGuardUsers(Guid deviceUID, List<GuardUser> guardUser);

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