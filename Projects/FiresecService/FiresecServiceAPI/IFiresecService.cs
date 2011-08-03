using System.Collections.Generic;
using System.ServiceModel;
using System.IO;
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
        SystemConfiguration GetSystemConfiguration();

        [OperationContract]
        DeviceConfigurationStates GetStates();

        [OperationContract]
        void SetDeviceConfiguration(DeviceConfiguration deviceConfiguration);

        [OperationContract]
        void WriteConfiguration(string id);

        [OperationContract]
        void SetSystemConfiguration(SystemConfiguration systemConfiguration);

        [OperationContract]
        List<JournalRecord> ReadJournal(int startIndex, int count);

        [OperationContract]
        void AddToIgnoreList(List<string> ids);

        [OperationContract]
        void RemoveFromIgnoreList(List<string> ids);

        [OperationContract]
        void ResetStates(List<ResetItem> resetItems);

        [OperationContract]
        void AddUserMessage(string message);

        [OperationContract]
        void ExecuteCommand(string id, string methodName);

        [OperationContract]
        List<string> GetSoundsFileName();

        [OperationContract]
        Dictionary<string, string> GetHashAndNameSoundFiles();

        [OperationContract]
        Stream GetFile(string filepath);
    }
}
