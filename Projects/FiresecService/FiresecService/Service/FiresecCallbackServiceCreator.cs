using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using FiresecAPI;

namespace FiresecService.Service
{
	public static class FiresecCallbackServiceCreator
	{
		public static IFiresecCallbackService CreateClientCallback(string serverAddress)
		{
			var binding = new NetTcpBinding()
			{
				MaxReceivedMessageSize = Int32.MaxValue,
				MaxBufferPoolSize = Int32.MaxValue,
				MaxBufferSize = Int32.MaxValue,
				MaxConnections = 1000,
				OpenTimeout = TimeSpan.FromMinutes(10),
				ReceiveTimeout = TimeSpan.FromMinutes(10),
				SendTimeout = TimeSpan.FromMinutes(10),
				ListenBacklog = 10
			};
			binding.ReaderQuotas.MaxStringContentLength = Int32.MaxValue;
			binding.ReaderQuotas.MaxArrayLength = Int32.MaxValue;
			binding.ReaderQuotas.MaxBytesPerRead = Int32.MaxValue;
			binding.ReaderQuotas.MaxDepth = Int32.MaxValue;
			binding.ReaderQuotas.MaxNameTableCharCount = Int32.MaxValue;
			binding.ReliableSession.InactivityTimeout = TimeSpan.MaxValue;

			var endpointAddress = new EndpointAddress(new Uri(serverAddress));
			var channelFactory = new ChannelFactory<IFiresecCallbackService>(binding, endpointAddress);

			foreach (OperationDescription operationDescription in channelFactory.Endpoint.Contract.Operations)
			{
				DataContractSerializerOperationBehavior dataContractSerializerOperationBehavior = operationDescription.Behaviors.Find<DataContractSerializerOperationBehavior>() as DataContractSerializerOperationBehavior;
				if (dataContractSerializerOperationBehavior != null)
					dataContractSerializerOperationBehavior.MaxItemsInObjectGraph = Int32.MaxValue;
			}

			channelFactory.Open();
			IFiresecCallbackService _firesecCallbackService = channelFactory.CreateChannel();
			(_firesecCallbackService as IContextChannel).OperationTimeout = TimeSpan.FromMinutes(100);
			return _firesecCallbackService;
		}
	}
}