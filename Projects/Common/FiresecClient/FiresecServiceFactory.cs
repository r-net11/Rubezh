using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using FiresecAPI;

namespace FiresecClient
{
	public class FiresecServiceFactory
	{
		static FiresecEventSubscriber _firesecEventSubscriber;
		DuplexChannelFactory<IFiresecService> _duplexChannelFactory;
		string _serverAddress;

		static FiresecServiceFactory()
		{
			_firesecEventSubscriber = new FiresecEventSubscriber();
		}

		public IFiresecService Create(string serverAddress)
		{
			_serverAddress = serverAddress;
			var binding = new NetTcpBinding()
			{
				MaxBufferPoolSize = Int32.MaxValue,
				MaxConnections = 10,
				OpenTimeout = TimeSpan.FromMinutes(10),
				ListenBacklog = 10,
				ReceiveTimeout = TimeSpan.FromMinutes(10),
				MaxBufferSize = Int32.MaxValue,
				MaxReceivedMessageSize = Int32.MaxValue
			};
			binding.ReliableSession.InactivityTimeout = TimeSpan.MaxValue;
			binding.ReaderQuotas.MaxStringContentLength = Int32.MaxValue;
			binding.ReaderQuotas.MaxArrayLength = Int32.MaxValue;
			binding.ReaderQuotas.MaxBytesPerRead = Int32.MaxValue;
			binding.ReaderQuotas.MaxDepth = Int32.MaxValue;
			binding.ReaderQuotas.MaxNameTableCharCount = Int32.MaxValue;

			var endpointAddress = new EndpointAddress(new Uri(serverAddress));
			_duplexChannelFactory = new DuplexChannelFactory<IFiresecService>(new InstanceContext(_firesecEventSubscriber), binding, endpointAddress);

			foreach (OperationDescription operationDescription in _duplexChannelFactory.Endpoint.Contract.Operations)
			{
				DataContractSerializerOperationBehavior dataContractSerializerOperationBehavior = operationDescription.Behaviors.Find<DataContractSerializerOperationBehavior>() as DataContractSerializerOperationBehavior;
				if (dataContractSerializerOperationBehavior != null)
					dataContractSerializerOperationBehavior.MaxItemsInObjectGraph = Int32.MaxValue;
			}

			_duplexChannelFactory.Open();
			IFiresecService _firesecService = _duplexChannelFactory.CreateChannel();
			(_firesecService as IContextChannel).OperationTimeout = TimeSpan.FromMinutes(100);
			return _firesecService;
		}

		public void Dispose()
		{
			try
			{
				if (_duplexChannelFactory != null)
				{
					_duplexChannelFactory.Abort();
					_duplexChannelFactory.Close();
				}
			}
			catch { ;}
		}
	}
}