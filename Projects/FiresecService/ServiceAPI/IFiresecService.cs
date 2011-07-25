using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using FiresecClient.Models;

namespace ServiceAPI
{
    [ServiceContract(CallbackContract = typeof(IFiresecCallback))]
    public interface IFiresecService
    {
        [OperationContract]
        void Initialize();

        [OperationContract]
        string Ping();

        [OperationContract]
        CurrentConfiguration GetCoreConfig();

        [OperationContract]
        CurrentStates GetCoreState();

        [OperationContract]
        string GetMetaData();

        [OperationContract]
        string GetCoreDeviceParams();

        [OperationContract]
        string ReadEvents(int fromId, int limit);

        [OperationContract]
        void SetNewConfig(string config);

        [OperationContract]
        void DeviceWriteConfig(string config, string devicePath);

        [OperationContract]
        void ResetStates(string config);
    }
}
