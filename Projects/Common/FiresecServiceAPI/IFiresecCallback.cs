using System.Collections.Generic;
using System.ServiceModel;
using FiresecAPI.Models;

namespace FiresecAPI
{
	public interface IFiresecCallback
	{
		[OperationContract(IsOneWay = true)]
		void Ping();
	}
}