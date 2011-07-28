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
        CurrentConfiguration GetConfiguration();

        [OperationContract]
        CurrentStates GetStates();

        [OperationContract]
        void SetConfiguration(CurrentConfiguration currentConfiguration);

        [OperationContract]
        void WriteConfiguration(string devicePath);

        [OperationContract]
        List<JournalItem> ReadJournal(int startIndex, int count);

        [OperationContract]
        void AddToIgnoreList(List<string> devicePaths);

        [OperationContract]
        void RemoveFromIgnoreList(List<string> devicePaths);

        [OperationContract]
        void ResetStates(List<ResetItem> resetItems);

        [OperationContract]
        void AddUserMessage(string message);

        [OperationContract]
        void ExecuteCommand(string devicePath, string methodName);

        [OperationContract]
        Stream GetFile();
    }
}
