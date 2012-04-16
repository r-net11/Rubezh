using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using FiresecAPI.Models.Skud;

namespace FiresecAPI
{
	[ServiceContract(CallbackContract = typeof(IFiresecCallback), SessionMode = SessionMode.Required)]
	public interface IFiresecServiceSKUD
	{
		[OperationContract]
		IEnumerable<EmployeeCardIndex> GetEmployees();
	}
}
