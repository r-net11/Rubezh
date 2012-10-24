using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using Common;
using FiresecAPI;
using Infrastructure.Common;
using System.Threading;
using Infrastructure.Common.Windows;

namespace FiresecClient
{
	public class FiresecServiceFactory
	{
		static FiresecCallback FiresecCallback;
		DuplexChannelFactory<IFiresecService> DuplexChannelFactory;
		string ServerAddress;

		static FiresecServiceFactory()
		{
			FiresecCallback = new FiresecCallback();
		}

		public IFiresecService Create(string serverAddress)
		{
			ServerAddress = serverAddress;

			for (int i = 0; i < 3; i++)
			{
				try
				{
					return TryCreate();
				}
				catch(Exception e)
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
			DuplexChannelFactory = new DuplexChannelFactory<IFiresecService>(new InstanceContext(FiresecCallback), binding, endpointAddress);

			foreach (OperationDescription operationDescription in DuplexChannelFactory.Endpoint.Contract.Operations)
			{
				DataContractSerializerOperationBehavior dataContractSerializerOperationBehavior = operationDescription.Behaviors.Find<DataContractSerializerOperationBehavior>() as DataContractSerializerOperationBehavior;
				if (dataContractSerializerOperationBehavior != null)
					dataContractSerializerOperationBehavior.MaxItemsInObjectGraph = Int32.MaxValue;
			}

				DuplexChannelFactory.Open();

			IFiresecService _firesecService = DuplexChannelFactory.CreateChannel();
			(_firesecService as IContextChannel).OperationTimeout = TimeSpan.FromMinutes(100);
			return _firesecService;
		}

		public void Dispose()
		{
			try
			{
				if (DuplexChannelFactory != null)
				{
					//_duplexChannelFactory.Abort();
					DuplexChannelFactory.Close();
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "FiresecServiceFactory.Dispose");
			}
		}
	}
}