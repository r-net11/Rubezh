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
        bool Connect(string userName, string passwordHash);

        [OperationContract(IsInitiating = true)]
        bool Reconnect(string userName, string passwordHash);

        [OperationContract(IsTerminating = true)]
        void Disconnect();

        [OperationContract]
        List<Driver> GetDrivers();

        [OperationContract]
        DeviceConfiguration GetDeviceConfiguration();

        [OperationContract]
        void SetDeviceConfiguration(DeviceConfiguration deviceConfiguration);

        [OperationContract]
        void WriteConfiguration(string deviceId);

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
        IEnumerable<JournalRecord> GetDistinctRecords();

        [OperationContract]
        void AddToIgnoreList(List<string> deviceIds);

        [OperationContract]
        void RemoveFromIgnoreList(List<string> deviceIds);

        [OperationContract]
        void ResetStates(List<ResetItem> resetItems);

        [OperationContract]
        void AddUserMessage(string message);

        [OperationContract]
        void ExecuteCommand(string deviceId, string methodName);

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