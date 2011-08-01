using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace FiresecClient
{
    public static class FiresecServiceFactory
    {
        static FiresecEventSubscriber _firesecEventSubscriber;

        public static IFiresecService Create()
        {
            NetTcpBinding binding = new NetTcpBinding();
            binding.MaxBufferPoolSize = Int32.MaxValue;
            binding.MaxConnections = 10;
            binding.OpenTimeout = TimeSpan.FromMinutes(10);
            binding.ListenBacklog = 10;
            binding.ReceiveTimeout = TimeSpan.FromMinutes(10);
            binding.MaxBufferSize = Int32.MaxValue;
            binding.MaxReceivedMessageSize = Int32.MaxValue;
            binding.MaxBufferPoolSize = Int32.MaxValue;
            binding.ReaderQuotas.MaxStringContentLength = Int32.MaxValue;
            binding.ReaderQuotas.MaxArrayLength = Int32.MaxValue;
            binding.ReaderQuotas.MaxBytesPerRead = Int32.MaxValue;
            binding.ReaderQuotas.MaxDepth = Int32.MaxValue;
            binding.ReaderQuotas.MaxNameTableCharCount = Int32.MaxValue;
            EndpointAddress endpointAddress = new EndpointAddress("net.tcp://localhost:8000/FiresecService");

            _firesecEventSubscriber = new FiresecEventSubscriber();
            DuplexChannelFactory<IFiresecService> _duplexChannelFactory = new DuplexChannelFactory<IFiresecService>(new InstanceContext(_firesecEventSubscriber), binding, endpointAddress);

            foreach (OperationDescription operationDescription in _duplexChannelFactory.Endpoint.Contract.Operations)
            {
                DataContractSerializerOperationBehavior dataContractSerializerOperationBehavior = operationDescription.Behaviors.Find<DataContractSerializerOperationBehavior>() as DataContractSerializerOperationBehavior;
                if (dataContractSerializerOperationBehavior != null)
                {
                    dataContractSerializerOperationBehavior.MaxItemsInObjectGraph = 2147483647;
                }
            }

            IFiresecService _firesecService = _duplexChannelFactory.CreateChannel();
            return _firesecService;
        }
    }
}
