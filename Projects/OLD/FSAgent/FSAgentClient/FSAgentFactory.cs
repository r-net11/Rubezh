using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading;
using Common;
using FSAgentAPI;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.BalloonTrayTip;

namespace FSAgentClient
{
	public class FSAgentFactory
	{
		public static Guid UID = Guid.NewGuid();
		ChannelFactory<IFSAgentContract> ChannelFactory;

		public IFSAgentContract Create(string serverAddress)
		{
			try
			{
				return DoCreate(serverAddress);
			}
			catch (Exception e)
			{
				Logger.Error(e, "FSAgentClient.Create");
				if (serverAddress.StartsWith("net.pipe:"))
				{
					Thread.Sleep(TimeSpan.FromSeconds(5));
				}
			}
			return null;
		}

		IFSAgentContract DoCreate(string serverAddress)
		{
			if (serverAddress.StartsWith("net.pipe:"))
			{
				if (!FSAgentLoadHelper.Load())
					BalloonHelper.ShowFromFiresec("Не удается соединиться с агентом");
			}

			var binding = BindingHelper.CreateBindingFromAddress(serverAddress);

			var endpointAddress = new EndpointAddress(new Uri(serverAddress));
			ChannelFactory = new ChannelFactory<IFSAgentContract>(binding, endpointAddress);

			foreach (OperationDescription operationDescription in ChannelFactory.Endpoint.Contract.Operations)
			{
				DataContractSerializerOperationBehavior dataContractSerializerOperationBehavior = operationDescription.Behaviors.Find<DataContractSerializerOperationBehavior>() as DataContractSerializerOperationBehavior;
				if (dataContractSerializerOperationBehavior != null)
					dataContractSerializerOperationBehavior.MaxItemsInObjectGraph = Int32.MaxValue;
			}

			ChannelFactory.Open();

			IFSAgentContract firesecService = ChannelFactory.CreateChannel();
			(firesecService as IContextChannel).OperationTimeout = TimeSpan.FromMinutes(10);
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
				Logger.Error(e, "FSAgentClient.Dispose");
			}
		}
	}
}