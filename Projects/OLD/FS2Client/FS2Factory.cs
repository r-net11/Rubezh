using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading;
using Common;
using FS2Api;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.BalloonTrayTip;

namespace FS2Client
{
	public class FS2Factory
	{
		public static Guid UID = Guid.NewGuid();
		ChannelFactory<IFS2Contract> ChannelFactory;

		public IFS2Contract Create(string serverAddress)
		{
			try
			{
				return DoCreate(serverAddress);
			}
			catch (Exception e)
			{
				Logger.Error(e, "FS2Client.Create");
				if (serverAddress.StartsWith("net.pipe:"))
				{
					Thread.Sleep(TimeSpan.FromSeconds(5));
				}
			}
			return null;
		}

		IFS2Contract DoCreate(string serverAddress)
		{
			if (serverAddress.StartsWith("net.pipe:"))
			{
				if (!FS2LoadHelper.Load())
					BalloonHelper.ShowFromFiresec("Не удается соединиться с агентом 2");
			}

			var binding = BindingHelper.CreateBindingFromAddress(serverAddress);

			var endpointAddress = new EndpointAddress(new Uri(serverAddress));
			ChannelFactory = new ChannelFactory<IFS2Contract>(binding, endpointAddress);

			foreach (OperationDescription operationDescription in ChannelFactory.Endpoint.Contract.Operations)
			{
				DataContractSerializerOperationBehavior dataContractSerializerOperationBehavior = operationDescription.Behaviors.Find<DataContractSerializerOperationBehavior>() as DataContractSerializerOperationBehavior;
				if (dataContractSerializerOperationBehavior != null)
					dataContractSerializerOperationBehavior.MaxItemsInObjectGraph = Int32.MaxValue;
			}

			ChannelFactory.Open();

			IFS2Contract firesecService = ChannelFactory.CreateChannel();
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
				Logger.Error(e, "FS2Client.Dispose");
			}
		}
	}
}