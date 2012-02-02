using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using FiresecAPI;

namespace FiresecClient
{
    public static class FiresecServiceFactory
    {
        static FiresecEventSubscriber _firesecEventSubscriber;
        static DuplexChannelFactory<IFiresecService> _duplexChannelFactory;

        public static IFiresecService Create(string serverAddress)
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

            _firesecEventSubscriber = new FiresecEventSubscriber();
            _duplexChannelFactory = new DuplexChannelFactory<IFiresecService>(new InstanceContext(_firesecEventSubscriber), binding, endpointAddress);

            foreach (OperationDescription operationDescription in _duplexChannelFactory.Endpoint.Contract.Operations)
            {
                DataContractSerializerOperationBehavior dataContractSerializerOperationBehavior = operationDescription.Behaviors.Find<DataContractSerializerOperationBehavior>() as DataContractSerializerOperationBehavior;
                if (dataContractSerializerOperationBehavior != null)
                    dataContractSerializerOperationBehavior.MaxItemsInObjectGraph = 2147483647;
            }

            _duplexChannelFactory.Open();

            IFiresecService _firesecService = _duplexChannelFactory.CreateChannel();

            (_firesecService as IContextChannel).OperationTimeout = TimeSpan.FromMinutes(10);

            return _firesecService;
        }

        public static void Dispose()
        {
            if ((_duplexChannelFactory != null) && (_duplexChannelFactory.State == CommunicationState.Created))
                _duplexChannelFactory.Close();
        }
    }
}