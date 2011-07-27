using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using FiresecClient.Models;
using System.IO;
using ServiceAPI.Models;

namespace ServiceAPI
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

        [OperationContract]
        List<JournalItem> ReadJournal(int startIndex, int count);
    }
}
