using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using FiresecAPI.Models;

namespace FiresecAPI
{
    [ServiceContract(CallbackContract = typeof(IFiresecCallback))]
    public interface IFiresecService
    {
        [OperationContract]
        void Connect();

        [OperationContract]
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
        SecurityConfiguration GetSecurityConfiguration();

        [OperationContract]
        void SetSecurityConfiguration(SecurityConfiguration securityConfiguration);

        [OperationContract]
        DeviceConfigurationStates GetStates();

        [OperationContract]
        List<JournalRecord> ReadJournal(int startIndex, int count);

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
        Dictionary<string, string> GetHashAndNameFiles(string directory);

        [OperationContract]
        Stream GetFile(string dirAndFileName);

        [OperationContract]
        string Ping();
    }
}
