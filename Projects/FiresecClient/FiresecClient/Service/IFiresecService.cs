using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using FiresecClient.Models;
using System.IO;

namespace FiresecClient
{
    [ServiceContract(CallbackContract = typeof(IFiresecCallback))]
    public interface IFiresecService
    {
        [OperationContract]
        Stream GetFile();

        [OperationContract]
        CurrentConfiguration GetCoreConfig();

        [OperationContract]
        CurrentStates GetCurrentStates();
    }
}
