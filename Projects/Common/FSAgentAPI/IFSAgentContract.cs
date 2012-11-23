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
		void GetStatus();

		[OperationContract]
		void GetEvents();

		[OperationContract]
		void GetStates();

		[OperationContract]
		void GetParameters();
	}
}