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
        void SetStatus(string clientId, string status);

		[OperationContract]
		List<MulticlientData> GetMulticlientData();

		[OperationContract]
		void Activate(string oldClientId, string newClientId, WindowSize windowSize);
	}
}