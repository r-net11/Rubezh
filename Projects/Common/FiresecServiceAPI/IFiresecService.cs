using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using FiresecAPI.Models;

namespace FiresecAPI
{
    [ServiceContract(CallbackContract = typeof(IFiresecCallback), SessionMode = SessionMode.Required)]
    public interface IFiresecService
    {
        [OperationContract(IsInitiating = true)]
        string Connect(string clientCallbackAddress, string userName, string password);

        [OperationContract]
        string Reconnect(string userName, string password);

        [OperationContract(IsTerminating = true, IsOneWay = true)]
        void Disconnect();

        [OperationContract(IsOneWay = true)]
        void Subscribe();

        [OperationContract]
        List<Driver> GetDrivers();

        [OperationContract]
        DeviceConfiguration GetDeviceConfiguration();

        [OperationContract]
        void SetDeviceConfiguration(DeviceConfiguration deviceConfiguration);

        [OperationContract]
        void DeviceWriteConfiguration(DeviceConfiguration deviceConfiguration, Guid deviceUID);

        [OperationContract]
        void DeviceWriteAllConfiguration(DeviceConfiguration deviceConfiguration);

        [OperationContract]
        void DeviceSetPassword(DeviceConfiguration deviceConfiguration, Guid deviceUID, DevicePasswordType devicePasswordType, string password);

        [OperationContract]
        void DeviceDatetimeSync(DeviceConfiguration deviceConfiguration, Guid deviceUID);

        [OperationContract]
        void DeviceRestart(DeviceConfiguration deviceConfiguration, Guid deviceUID);

        [OperationContract]
        string DeviceGetInformation(DeviceConfiguration deviceConfiguration, Guid deviceUID);

        [OperationContract]
        List<string> DeviceGetSerialList(DeviceConfiguration deviceConfiguration, Guid deviceUID);

        [OperationContract]
        string DeviceUpdateFirmware(DeviceConfiguration deviceConfiguration, Guid deviceUID, byte[] bytes, string fileName);

        [OperationContract]
        string DeviceVerifyFirmwareVersion(DeviceConfiguration deviceConfiguration, Guid deviceUID, byte[] bytes, string fileName);

        [OperationContract]
        string DeviceReadEventLog(DeviceConfiguration deviceConfiguration, Guid deviceUID);

        [OperationContract]
        DeviceConfiguration DeviceAutoDetectChildren(DeviceConfiguration deviceConfiguration, Guid deviceUID, bool fastSearch);

        [OperationContract]
        DeviceConfiguration DeviceReadConfiguration(DeviceConfiguration deviceConfiguration, Guid deviceUID);

        [OperationContract]
        List<DeviceCustomFunction> DeviceCustomFunctionList(Guid driverUID);

        [OperationContract]
        string DeviceCustomFunctionExecute(DeviceConfiguration deviceConfiguration, Guid deviceUID, string functionName);

        [OperationContract]
        string DeviceGetGuardUsersList(DeviceConfiguration deviceConfiguration, Guid deviceUID);

        [OperationContract]
        string DeviceSetGuardUsersList(DeviceConfiguration deviceConfiguration, Guid deviceUID, string users);

        [OperationContract]
        string DeviceGetMDS5Data(DeviceConfiguration deviceConfiguration, Guid deviceUID);

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

        [OperationContract]
        List<JournalRecord> ReadJournal(int startIndex, int count);

        [OperationContract]
        IEnumerable<JournalRecord> GetFilteredJournal(JournalFilter journalFilter);

        [OperationContract]
        IEnumerable<JournalRecord> GetFilteredArchive(ArchiveFilter archiveFilter);

        [OperationContract]
        IEnumerable<JournalRecord> GetDistinctRecords();

        [OperationContract]
        DateTime GetArchiveStartDate();

        [OperationContract(IsOneWay = true)]
        void AddToIgnoreList(List<Guid> deviceUIDs);

        [OperationContract(IsOneWay = true)]
        void RemoveFromIgnoreList(List<Guid> deviceUIDs);

        [OperationContract(IsOneWay = true)]
        void ResetStates(List<ResetItem> resetItems);

        [OperationContract(IsOneWay = true)]
        void AddUserMessage(string message);

        [OperationContract(IsOneWay = true)]
        void AddJournalRecord(JournalRecord journalRecord);

        [OperationContract]
        void ExecuteCommand(Guid deviceUID, string methodName);

        [OperationContract]
        List<string> GetFileNamesList(string directory);

        [OperationContract]
        Dictionary<string, string> GetDirectoryHash(string directory);

        [OperationContract]
        Stream GetFile(string dirAndFileName);

        [OperationContract]
        string Ping();

        [OperationContract]
        [FaultContract(typeof(FiresecException))]
        string Test();
    }

    public class FiresecException : Exception
    {
    }
}