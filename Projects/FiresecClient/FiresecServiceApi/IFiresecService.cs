using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace FiresecServiceApi
{
    [ServiceContract(CallbackContract = typeof(IFiresecCallback))]
    public interface IFiresecService
    {
        [OperationContract]
        void Initialize();

        [OperationContract]
        string Ping();

        [OperationContract]
        string GetCoreConfig();

        [OperationContract]
        string GetCoreState();

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
