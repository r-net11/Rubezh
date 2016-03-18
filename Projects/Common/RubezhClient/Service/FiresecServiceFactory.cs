using Common;
using Infrastructure.Common;
using Infrastructure.Common.BalloonTrayTip;
using RubezhAPI;
using System;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace RubezhClient
{
	public class FiresecServiceFactory
	{
		public Guid UID = Guid.NewGuid();
		ChannelFactory<IFiresecService> _channelFactory;
		string _serverAddress;

		public FiresecServiceFactory(string serverAddress)
		{
			_serverAddress = serverAddress;
			var binding = BindingHelper.CreateBindingFromAddress(serverAddress);

			var endpointAddress = new EndpointAddress(new Uri(serverAddress));
			_channelFactory = new ChannelFactory<IFiresecService>(binding, endpointAddress);

			foreach (OperationDescription operationDescription in _channelFactory.Endpoint.Contract.Operations)
			{
				DataContractSerializerOperationBehavior dataContractSerializerOperationBehavior = operationDescription.Behaviors.Find<DataContractSerializerOperationBehavior>() as DataContractSerializerOperationBehavior;
				if (dataContractSerializerOperationBehavior != null)
					dataContractSerializerOperationBehavior.MaxItemsInObjectGraph = Int32.MaxValue;
			}

			_channelFactory.Open();
		}

		public IFiresecService Create(TimeSpan operationTimeout)
		{
			try
			{
				return DoCreate(operationTimeout);
			}
			catch (Exception e)
			{
				Logger.Error(e, "FiresecServiceFactory.Create");
			}
			return null;
		}

		private IFiresecService DoCreate(TimeSpan operationTimeout)
		{
			if (_serverAddress.StartsWith("net.pipe:"))
			{
				if (!ServerLoadHelper.Load())
					BalloonHelper.ShowFromAdm("Не удается соединиться с сервером");
			}

			IFiresecService firesecService = _channelFactory.CreateChannel();
			(firesecService as IContextChannel).OperationTimeout = operationTimeout;
			return firesecService;
		}

		public void Dispose()
		{
			try
			{
				if (_channelFactory != null)
				{
					try
					{
						_channelFactory.Close();
					}
					catch { }
					try
					{
						_channelFactory.Abort();
					}
					catch { }
					_channelFactory = null;
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "FiresecServiceFactory.Dispose");
			}
		}
	}
}