using Common;
using StrazhAPI;
using System;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace FiresecClient
{
	public class FiresecServiceFactory
	{
		public static Guid UID = Guid.NewGuid();
		private ChannelFactory<IFiresecService> _channelFactory;

		public IFiresecService Create(string serverAddress)
		{
			try
			{
				return DoCreate(serverAddress);
			}
			catch (Exception e)
			{
				Logger.Error(e, "FiresecServiceFactory.Create");
			}

			return null;
		}

		private IFiresecService DoCreate(string serverAddress)
		{
			var binding = BindingHelper.CreateBindingFromAddress(serverAddress);

			var endpointAddress = new EndpointAddress(new Uri(serverAddress));

			_channelFactory = new ChannelFactory<IFiresecService>(binding, endpointAddress);

			foreach (var operationDescription in _channelFactory.Endpoint.Contract.Operations)
			{
				var dataContractSerializerOperationBehavior = operationDescription.Behaviors.Find<DataContractSerializerOperationBehavior>();

				if (dataContractSerializerOperationBehavior != null)
					dataContractSerializerOperationBehavior.MaxItemsInObjectGraph = Int32.MaxValue;
			}

			_channelFactory.Open();

			var firesecService = _channelFactory.CreateChannel();
			(firesecService as IContextChannel).OperationTimeout = TimeSpan.FromMinutes(100);
			return firesecService;
		}

		public void Dispose()
		{
			if (_channelFactory == null) return;

			try
			{
				_channelFactory.Close();
			}
			catch (CommunicationException)
			{
				if (_channelFactory != null)
					_channelFactory.Abort();
			}
			catch (TimeoutException)
			{
				if (_channelFactory != null)
					_channelFactory.Abort();
			}
			catch (Exception e)
			{
				if(_channelFactory != null)
					_channelFactory.Abort();

				Logger.Error(e, "FiresecServiceFactory.Dispose");
				throw;
			}
		}
	}
}