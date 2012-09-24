using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using Common;
using FiresecAPI;

namespace FiresecClient
{
	public class FiresecServiceFactory
	{
		static FiresecCallback _firesecCallback;
		DuplexChannelFactory<IFiresecService> _duplexChannelFactory;
		string _serverAddress;

		static FiresecServiceFactory()
		{
			_firesecCallback = new FiresecCallback();
		}

		public IFiresecService Create(string serverAddress)
		{
			_serverAddress = serverAddress;
			Binding binding = BindingHelper.CreateBindingFromAddress(_serverAddress);

			var endpointAddress = new EndpointAddress(new Uri(serverAddress));
			_duplexChannelFactory = new DuplexChannelFactory<IFiresecService>(new InstanceContext(_firesecCallback), binding, endpointAddress);

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
					//_duplexChannelFactory.Abort();
					_duplexChannelFactory.Close();
				}
			}
			catch { ;}
		}
	}
}