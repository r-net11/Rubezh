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
			var binding = Common.BindingHelper.CreateBindingFromAddress(serverAddress);

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