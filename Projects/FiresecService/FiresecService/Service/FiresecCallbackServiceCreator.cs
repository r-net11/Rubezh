using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using FiresecAPI;

namespace FiresecService
{
    public static class FiresecCallbackServiceCreator
    {
        public static IFiresecCallbackService CreateClientCallback(string serverAddress)
        {
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
            binding.ReliableSession.InactivityTimeout = TimeSpan.FromMinutes(10);
            binding.ReaderQuotas.MaxStringContentLength = Int32.MaxValue;
            binding.ReaderQuotas.MaxArrayLength = Int32.MaxValue;
            binding.ReaderQuotas.MaxBytesPerRead = Int32.MaxValue;
            binding.ReaderQuotas.MaxDepth = Int32.MaxValue;
            binding.ReaderQuotas.MaxNameTableCharCount = Int32.MaxValue;

            var endpointAddress = new EndpointAddress(new Uri(serverAddress));

            var channelFactory = new ChannelFactory<IFiresecCallbackService>(binding, endpointAddress);

            foreach (OperationDescription operationDescription in channelFactory.Endpoint.Contract.Operations)
            {
                DataContractSerializerOperationBehavior dataContractSerializerOperationBehavior = operationDescription.Behaviors.Find<DataContractSerializerOperationBehavior>() as DataContractSerializerOperationBehavior;
                if (dataContractSerializerOperationBehavior != null)
                    dataContractSerializerOperationBehavior.MaxItemsInObjectGraph = 2147483647;
            }

            channelFactory.Open();

            IFiresecCallbackService _firesecCallbackService = channelFactory.CreateChannel();

            (_firesecCallbackService as IContextChannel).OperationTimeout = TimeSpan.FromMinutes(10);

            return _firesecCallbackService;
        }
    }
}
