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
            if (host != null)
                return;

            host = new ServiceHost(typeof(FiresecService));

            NetTcpBinding binding = new NetTcpBinding()
            {
                MaxReceivedMessageSize = Int32.MaxValue,
                MaxBufferPoolSize = Int32.MaxValue,
                MaxBufferSize = Int32.MaxValue,
                MaxConnections = 1000,
                OpenTimeout = TimeSpan.FromMinutes(10),
                ReceiveTimeout = TimeSpan.FromMinutes(10),
                ListenBacklog = 10,
            };
            binding.ReaderQuotas.MaxStringContentLength = Int32.MaxValue;
            binding.ReaderQuotas.MaxArrayLength = Int32.MaxValue;
            binding.ReaderQuotas.MaxBytesPerRead = Int32.MaxValue;
            binding.ReaderQuotas.MaxDepth = Int32.MaxValue;
            binding.ReaderQuotas.MaxNameTableCharCount = Int32.MaxValue;

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