using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Common;

namespace ServiceApi
{
    [ServiceContract(CallbackContract = typeof(ICallback))]
    //[ServiceKnownType(typeof(TreeBase))]
    public interface IStateService
    {
        [OperationContract]
        void Initialize();

        [OperationContract]
        Configuration GetConfiguration();

        [OperationContract]
        void ExecuteCommand(string devicePath, string command);

        [OperationContract]
        void SetConfiguration(Configuration configuration);
    }
}
