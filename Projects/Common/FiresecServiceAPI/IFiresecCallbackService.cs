using System.ServiceModel;
using FiresecAPI.Models;

namespace FiresecAPI
{
    [ServiceContract(SessionMode = SessionMode.Required)]
    public interface IFiresecCallbackService
    {
        [OperationContract(IsOneWay = true)]
        void ConfigurationChanged();

        [OperationContract(IsOneWay = false)]
        bool Progress(int stage, string comment, int percentComplete, int bytesRW);

        [OperationContract(IsOneWay = true)]
        void NewJournalRecord(JournalRecord journalRecord);
    }
}