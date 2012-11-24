using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace FSAgentAPI
{
	[ServiceContract]
	public interface IFSAgentContract
	{
		[OperationContract]
		string GetStatus();

		[OperationContract]
        List<ChangeResult> GetChangeResult();
	}
}