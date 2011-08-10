using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using FiresecAPI;
using System.Diagnostics;

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

            binding.Security.Message.ClientCredentialType = MessageCredentialType.UserName;

            EndpointAddress endpointAddress = new EndpointAddress("net.tcp://localhost:8000/FiresecService");

            _firesecEventSubscriber = new FiresecEventSubscriber();
            DuplexChannelFactory<IFiresecService> _duplexChannelFactory = new DuplexChannelFactory<IFiresecService>(new InstanceContext(_firesecEventSubscriber), binding, endpointAddress);

            _duplexChannelFactory.Credentials.UserName.UserName = "login";
            _duplexChannelFactory.Credentials.UserName.Password = "pass";

            foreach (OperationDescription operationDescription in _duplexChannelFactory.Endpoint.Contract.Operations)
            {
                DataContractSerializerOperationBehavior dataContractSerializerOperationBehavior = operationDescription.Behaviors.Find<DataContractSerializerOperationBehavior>() as DataContractSerializerOperationBehavior;
                if (dataContractSerializerOperationBehavior != null)
                {
                    dataContractSerializerOperationBehavior.MaxItemsInObjectGraph = 2147483647;
                }
            }

            _duplexChannelFactory.Faulted += new EventHandler(_duplexChannelFactory_Faulted);

            IFiresecService _firesecService = _duplexChannelFactory.CreateChannel();
            return _firesecService;
        }

        static void _duplexChannelFactory_Faulted(object sender, EventArgs e)
        {
            Trace.WriteLine("Channel fault");
        }
    }
}