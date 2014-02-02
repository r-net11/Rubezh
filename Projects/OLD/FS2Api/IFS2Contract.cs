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
		OperationResult AddToIgnoreList(List<Guid> deviceUIDs, string userName);

		[OperationContract]
		OperationResult RemoveFromIgnoreList(List<Guid> deviceUIDs, string userName);

		[OperationContract]
		OperationResult SetZoneGuard(Guid zoneUID, string userName);

		[OperationContract]
		OperationResult UnSetZoneGuard(Guid zoneUID, string userName);

		[OperationContract]
		OperationResult SetDeviceGuard(Guid deviceUID, string userName);

		[OperationContract]
		OperationResult UnSetDeviceGuard(Guid deviceUID, string userName);

		[OperationContract]
		OperationResult ResetStates(List<PanelResetItem> panelResetItems, string userName);

		[OperationContract]
		OperationResult ExecuteCommand(Guid deviceUID, string methodName, string userName);

		[OperationContract]
		OperationResult<bool> CheckHaspPresence();
		#endregion

		#region Administrator
		[OperationContract]
		OperationResult SetNewConfiguration(DeviceConfiguration deviceConfiguration, string userName);

		[OperationContract]
		OperationResult<bool> DeviceWriteConfiguration(Guid deviceUID, bool isUSB, string userName);

		[OperationContract]
		OperationResult<List<Guid>> DeviceWriteAllConfiguration(List<Guid> deviceUIDs, string userName);

		[OperationContract]
		OperationResult DeviceSetPassword(Guid deviceUID, bool isUSB, DevicePasswordType devicePasswordType, string password, string userName);

		[OperationContract]
		OperationResult DeviceDatetimeSync(Guid deviceUID, bool isUSB, string userName);

		[OperationContract]
		OperationResult<string> DeviceGetInformation(Guid deviceUID, bool isUSB, string userName);

		[OperationContract]
		OperationResult<List<string>> DeviceGetSerialList(Guid deviceUID, string userName);

		[OperationContract]
		OperationResult<string> DeviceVerifyFirmwareVersion(Guid deviceUID, bool isUSB, string fileName);

		[OperationContract]
		OperationResult DeviceUpdateFirmware(Guid deviceUID, bool isUSB, string fileName, string userName);

		[OperationContract]
		OperationResult<DeviceConfiguration> DeviceReadConfiguration(Guid deviceUID, bool isUSB, string userName);

		[OperationContract]
		OperationResult<FS2JournalItemsCollection> DeviceReadJournal(Guid deviceUID, bool isUSB, string userName);

		[OperationContract]
		OperationResult<DeviceConfiguration> DeviceAutoDetectChildren(Guid deviceUID, bool fastSearch, string userName);

		[OperationContract]
		OperationResult<List<DeviceCustomFunction>> DeviceGetCustomFunctions(DriverType driverType);

		[OperationContract]
		OperationResult DeviceExecuteCustomFunction(Guid deviceUID, bool isUSB, string functionName, string userName);

		[OperationContract]
		OperationResult<List<GuardUser>> DeviceGetGuardUsers(Guid deviceUID, string userName);

		[OperationContract]
		OperationResult DeviceSetGuardUsers(Guid deviceUID, List<GuardUser> guardUser, string userName);

		[OperationContract]
		OperationResult<string> DeviceGetMDS5Data(Guid deviceUID, string userName);

		[OperationContract]
		OperationResult SetAuParameters(Guid deviceUID, List<Property> properties, string userName);

		[OperationContract]
		OperationResult<List<Property>> GetAuParameters(Guid deviceUID, string userName);
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
		OperationResult AddJournalItems(List<FS2JournalItem> journalItems);
		#endregion
	}
}