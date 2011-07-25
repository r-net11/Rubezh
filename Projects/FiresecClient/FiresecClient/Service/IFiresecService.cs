using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using FiresecClient.Models;

namespace FiresecClient
{
    [ServiceContract(CallbackContract = typeof(IFiresecCallback))]
    public interface IFiresecService
    {
        [OperationContract]
        string GetName();

        [OperationContract]
        CurrentConfiguration GetCoreConfig();
    }
}
