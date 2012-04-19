using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using FiresecAPI.Models;
using XFiresecAPI;
using FiresecAPI.Models.Skud;

namespace FiresecAPI
{
	[ServiceContract(CallbackContract = typeof(IFiresecCallback), SessionMode = SessionMode.Required)]
	public interface IFiresecService : IFiresecServiceSKUD
	{
		#region Service
		[OperationContract(IsInitiating = true)]
		OperationResult<bool> Connect(string clientType, string clientCallbackAddress, string userName, string password);

		[OperationContract]
		OperationResult<bool> Reconnect(string userName, string password);

		[OperationContract(IsTerminating = true, IsOneWay = true)]
		void Disconnect();

		[OperationContract(IsOneWay = true)]
		void Subscribe();

		[OperationContract(IsOneWay = true)]
		void CancelProgress();

        [OperationContract]
        string GetStatus();

		[OperationContract]
		string Ping();

		[OperationContract]
		[FaultContract(typeof(FiresecException))]
		string Test();
		#endregion

		#region Configuration
		[OperationContract]
		List<Driver> GetDrivers();

		[OperationContract]
		DeviceConfiguration GetDeviceConfiguration();

		[OperationContract]
		SystemConfiguration GetSystemConfiguration();

		[OperationContract]
		void SetSystemConfiguration(SystemConfiguration systemConfiguration);

		[OperationContract]
		LibraryConfiguration GetLibraryConfiguration();

		[OperationContract]
		void SetLibraryConfiguration(LibraryConfiguration libraryConfiguration);

		[OperationContract]
		PlansConfiguration GetPlansConfiguration();

		[OperationContract]
		void SetPlansConfiguration(PlansConfiguration plansConfiguration);

		[OperationContract]
		SecurityConfiguration GetSecurityConfiguration();

		[OperationContract]
		void SetSecurityConfiguration(SecurityConfiguration securityConfiguration);

		[OperationContract]
		DeviceConfigurationStates GetStates();
		#endregion

		#region Devices
		[OperationContract]
		OperationResult<bool> SetDeviceConfiguration(DeviceConfiguration deviceConfiguration);

		[OperationContract]
		OperationResult<bool> DeviceWriteConfiguration(DeviceConfiguration deviceConfiguration, Guid deviceUID);

		[OperationContract]
		OperationResult<bool> DeviceWriteAllConfiguration(DeviceConfiguration deviceConfiguration);

		[OperationContract]
		OperationResult<bool> DeviceSetPassword(DeviceConfiguration deviceConfiguration, Guid deviceUID, DevicePasswordType devicePasswordType, string password);

		[OperationContract]
		OperationResult<bool> DeviceDatetimeSync(DeviceConfiguration deviceConfiguration, Guid deviceUID);

		[OperationContract]
		OperationResult<string> DeviceGetInformation(DeviceConfiguration deviceConfiguration, Guid deviceUID);

		[OperationContract]
		OperationResult<List<string>> DeviceGetSerialList(DeviceConfiguration deviceConfiguration, Guid deviceUID);

		[OperationContract]
		OperationResult<string> DeviceUpdateFirmware(DeviceConfiguration deviceConfiguration, Guid deviceUID, byte[] bytes, string fileName);

		[OperationContract]
		OperationResult<string> DeviceVerifyFirmwareVersion(DeviceConfiguration deviceConfiguration, Guid deviceUID, byte[] bytes, string fileName);

		[OperationContract]
		OperationResult<string> DeviceReadEventLog(DeviceConfiguration deviceConfiguration, Guid deviceUID);

		[OperationContract]
		OperationResult<DeviceConfiguration> DeviceAutoDetectChildren(DeviceConfiguration deviceConfiguration, Guid deviceUID, bool fastSearch);

		[OperationContract]
		OperationResult<DeviceConfiguration> DeviceReadConfiguration(DeviceConfiguration deviceConfiguration, Guid deviceUID);

		[OperationContract]
		OperationResult<List<DeviceCustomFunction>> DeviceCustomFunctionList(Guid driverUID);

		[OperationContract]
		OperationResult<string> DeviceCustomFunctionExecute(DeviceConfiguration deviceConfiguration, Guid deviceUID, string functionName);

		[OperationContract]
		OperationResult<string> DeviceGetGuardUsersList(DeviceConfiguration deviceConfiguration, Guid deviceUID);

		[OperationContract]
		OperationResult<bool> DeviceSetGuardUsersList(DeviceConfiguration deviceConfiguration, Guid deviceUID, string users);

		[OperationContract]
		OperationResult<string> DeviceGetMDS5Data(DeviceConfiguration deviceConfiguration, Guid deviceUID);

		[OperationContract()]
		OperationResult<bool> AddToIgnoreList(List<Guid> deviceUIDs);

		[OperationContract()]
		OperationResult<bool> RemoveFromIgnoreList(List<Guid> deviceUIDs);

		[OperationContract(IsOneWay=true)]
		void ResetStates(List<ResetItem> resetItems);

		[OperationContract]
		OperationResult<bool> ExecuteCommand(Guid deviceUID, string methodName);

		[OperationContract]
		OperationResult<bool> CheckHaspPresence();
		#endregion

		#region Journal
		[OperationContract]
		OperationResult<List<JournalRecord>> ReadJournal(int startIndex, int count);

		[OperationContract]
		OperationResult<List<JournalRecord>> GetFilteredJournal(JournalFilter journalFilter);

		[OperationContract]
		OperationResult<List<JournalRecord>> GetFilteredArchive(ArchiveFilter archiveFilter);

		[OperationContract]
		OperationResult<List<JournalRecord>> GetDistinctRecords();

		[OperationContract]
		OperationResult<DateTime> GetArchiveStartDate();

		[OperationContract()]
		void AddUserMessage(string message);

		[OperationContract()]
		void AddJournalRecord(JournalRecord journalRecord);
		#endregion

		#region Files
		[OperationContract]
		List<string> GetFileNamesList(string directory);

		[OperationContract]
		Dictionary<string, string> GetDirectoryHash(string directory);

		[OperationContract]
		Stream GetFile(string dirAndFileName);
		#endregion

		#region Convertation
		[OperationContract]
		void ConvertConfiguration();

		[OperationContract]
		void ConvertJournal();
		#endregion

		#region XSystem
		[OperationContract]
		void SetXDeviceConfiguration(XDeviceConfiguration xDeviceConfiguration);

		[OperationContract]
		XDeviceConfiguration GetXDeviceConfiguration();
		#endregion
	}

	public class FiresecException : Exception
	{
	}
}