using System;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace FiresecService
{
    public static class FiresecServiceManager
    {
        static ServiceHost host;

        public static void Open()
        {
            host = new ServiceHost(typeof(FiresecService));

            NetTcpBinding binding = new NetTcpBinding();
            binding.MaxBufferPoolSize = Int32.MaxValue;
            binding.MaxConnections = 1000;
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

            binding.ReliableSession.InactivityTimeout = TimeSpan.MaxValue;
            host.AddServiceEndpoint("FiresecAPI.IFiresecService", binding, "net.tcp://localhost:8000/FiresecService");

            ServiceMetadataBehavior behavior = host.Description.Behaviors.Find<ServiceMetadataBehavior>();
            if (behavior == null)
            {
                behavior = new ServiceMetadataBehavior();
                behavior.HttpGetUrl = new Uri("http://localhost:8001/FiresecService");
                behavior.HttpGetEnabled = true;
                host.Description.Behaviors.Add(behavior);
            }

            host.Open();
        }

        public static void Close()
        {
            if (host != null)
                host.Close();
        }
    }
}
