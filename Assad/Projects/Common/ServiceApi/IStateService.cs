using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Common;

namespace ServiceApi
{
    [ServiceContract(CallbackContract = typeof(ICallback))]
    public interface IStateService
    {
        [OperationContract]
        void Initialize();

        [OperationContract]
        StateConfiguration GetConfiguration();

        [OperationContract]
        ShortStates GetStates();

        [OperationContract]
        void ExecuteCommand(string devicePath, string command);

        [OperationContract]
        void SetConfiguration(StateConfiguration configuration);
    }
}
