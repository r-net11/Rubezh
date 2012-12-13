using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace MuliclientAPI
{
	[ServiceContract(CallbackContract=typeof(IMuliclientCallback))]
	public interface IMuliclient
	{
		[OperationContract]
		void Connect(int clientId);

		[OperationContract]
		void Loaded(int clientId);

		[OperationContract]
		void Closed(int clientId);
	}
}