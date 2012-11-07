using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Threading;
using Common;
using FiresecAPI;
using Infrastructure.Common;
using Infrastructure.Common.Windows;

namespace FiresecClient
{
	public class FiresecServiceFactory
	{
		ChannelFactory<IFiresecService> ChannelFactory;
		string ServerAddress;

		public IFiresecService Create(string serverAddress)
		{
			ServerAddress = serverAddress;

			for (int i = 0; i < 3; i++)
			{
				try
				{
					return TryCreate();
				}
				catch (Exception e)
				{
					Logger.Error(e, "FiresecServiceFactory.Create");
					if (serverAddress.StartsWith("net.pipe:"))
					{
						ServerLoadHelper.Reload();
						Thread.Sleep(TimeSpan.FromSeconds(5));
					}
				}
			}
			MessageBoxService.Show("Невозможно соединиться с сервером");
			return null;
		}

		public IFiresecService TryCreate()
		{
			Binding binding = BindingHelper.CreateBindingFromAddress(ServerAddress);

			var endpointAddress = new EndpointAddress(new Uri(ServerAddress));
			ChannelFactory = new ChannelFactory<IFiresecService>(binding, endpointAddress);

			foreach (OperationDescription operationDescription in ChannelFactory.Endpoint.Contract.Operations)
			{
				DataContractSerializerOperationBehavior dataContractSerializerOperationBehavior = operationDescription.Behaviors.Find<DataContractSerializerOperationBehavior>() as DataContractSerializerOperationBehavior;
				if (dataContractSerializerOperationBehavior != null)
					dataContractSerializerOperationBehavior.MaxItemsInObjectGraph = Int32.MaxValue;
			}

			ChannelFactory.Open();

			IFiresecService _firesecService = ChannelFactory.CreateChannel();
			(_firesecService as IContextChannel).OperationTimeout = TimeSpan.FromMinutes(100);
			return _firesecService;
		}

		public void Dispose()
		{
			try
			{
				if (ChannelFactory != null)
				{
					if (ChannelFactory.State != CommunicationState.Opened)
					{
						ChannelFactory.Close();
					}
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "FiresecServiceFactory.Dispose");
			}
		}
	}
}