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
		void Connect(string clientId);

		[OperationContract]
        void Loaded(string clientId);

		[OperationContract]
        void Closed(string clientId);
	}
}