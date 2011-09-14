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
        bool Connect(string userName, string password);

        [OperationContract]
        bool Reconnect(string userName, string password);

        [OperationContract(IsTerminating = true)]
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
        void WriteConfiguration(Guid deviceUID);

        [OperationContract]
        void DeviceSetPassword(DeviceConfiguration deviceConfiguration, Guid deviceUID, string password);

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