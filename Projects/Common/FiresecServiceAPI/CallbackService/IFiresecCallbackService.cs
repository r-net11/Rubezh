using System.ServiceModel;

namespace FiresecAPI
{
    [ServiceContract(SessionMode = SessionMode.Required)]
    public interface IFiresecCallbackService
    {
        [OperationContract(IsOneWay = true)]
        void ConfigurationChanged();

        [OperationContract(IsOneWay = true)]
        void Progress(int stage, string comment, int percentComplete, int bytesRW);
    }
}