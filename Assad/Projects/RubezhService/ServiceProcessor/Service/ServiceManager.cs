using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;
using ServiseProcessor.Service;
using System.Diagnostics;

namespace ServiseProcessor
{
    public static class ServiceManager
    {
        static ServiceHost host;

        public static void Open()
        {
            host = new ServiceHost(typeof(FiresecService));

            NetTcpBinding binding = new NetTcpBinding();
            binding.MaxBufferSize = Int32.MaxValue;
            binding.MaxReceivedMessageSize = Int32.MaxValue;
            binding.MaxBufferPoolSize = Int32.MaxValue;
            binding.ReceiveTimeout = TimeSpan.MaxValue;
            binding.MaxConnections = 1000;
            binding.ReliableSession.InactivityTimeout = TimeSpan.MaxValue;
            host.AddServiceEndpoint("ServiceApi.IFiresecService", binding, "net.tcp://localhost:8000/StateService");

            ServiceMetadataBehavior behavior = host.Description.Behaviors.Find<ServiceMetadataBehavior>();
            if (behavior == null)
            {
                behavior = new ServiceMetadataBehavior();
                behavior.HttpGetUrl = new Uri("http://localhost:8001/StateService");
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
