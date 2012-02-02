using System.ServiceModel;

namespace FiresecAPI
{
    [ServiceContract(SessionMode = SessionMode.Required)]
    public interface IFiresecCallbackService
    {
        [OperationContract(IsOneWay = true)]
        void ConfigurationChanged();
    }
}