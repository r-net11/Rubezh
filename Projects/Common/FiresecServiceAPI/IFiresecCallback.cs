using System.ServiceModel;
using FiresecAPI.Models;
using System.Collections.Generic;

namespace FiresecAPI
{
	public interface IFiresecCallback
	{
		[OperationContract(IsOneWay = true)]
        void Ping();
	}
}