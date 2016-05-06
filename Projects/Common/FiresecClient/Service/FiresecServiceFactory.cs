using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using Common;
using StrazhAPI;
using Infrastructure.Common;
using Infrastructure.Common.BalloonTrayTip;

namespace FiresecClient
{
	public class FiresecServiceFactory
	{
		public static Guid UID = Guid.NewGuid();
		ChannelFactory<IFiresecService> ChannelFactory;

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
			ChannelFactory = new ChannelFactory<IFiresecService>(binding, endpointAddress);

			foreach (OperationDescription operationDescription in ChannelFactory.Endpoint.Contract.Operations)
			{
				DataContractSerializerOperationBehavior dataContractSerializerOperationBehavior = operationDescription.Behaviors.Find<DataContractSerializerOperationBehavior>() as DataContractSerializerOperationBehavior;
				if (dataContractSerializerOperationBehavior != null)
					dataContractSerializerOperationBehavior.MaxItemsInObjectGraph = Int32.MaxValue;
			}

			ChannelFactory.Open();

			IFiresecService firesecService = ChannelFactory.CreateChannel();
			(firesecService as IContextChannel).OperationTimeout = TimeSpan.FromMinutes(100);
			return firesecService;
		}

		public void Dispose()
		{
			try
			{
				if (ChannelFactory != null)
				{
					try
					{
						ChannelFactory.Close();
					}
					catch { }
					try
					{
						ChannelFactory.Abort();
					}
					catch { }
					ChannelFactory = null;
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "FiresecServiceFactory.Dispose");
			}
		}
	}
}