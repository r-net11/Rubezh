using System.Collections.Generic;
using System.ServiceModel;
using FiresecAPI.Models.Skud;

namespace FiresecAPI
{
    [ServiceContract(CallbackContract = typeof(IFiresecCallback), SessionMode = SessionMode.Required)]
    public interface IFiresecServiceSKUD
    {
        [OperationContract]
        IEnumerable<EmployeeCard> GetEmployees();
    }
}