using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace ServiseProcessor
{
    public static class ServiceManager
    {
        static ServiceHost host;

        public static void Open()
        {
            host = new ServiceHost(typeof(StateService));

            NetTcpBinding binding = new NetTcpBinding();
            binding.MaxBufferSize = Int32.MaxValue;
            binding.MaxReceivedMessageSize = Int32.MaxValue;
            binding.MaxBufferPoolSize = Int32.MaxValue;
            host.AddServiceEndpoint("ServiceApi.IStateService", binding, "net.tcp://localhost:8000/StateService");

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
