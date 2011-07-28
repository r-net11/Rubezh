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
        Stream GetFile();

        [OperationContract]
        CurrentConfiguration GetCoreConfig();

        [OperationContract]
        CurrentStates GetCurrentStates();

        [OperationContract]
        List<JournalItem> ReadJournal(int startIndex, int count);
    }
}
