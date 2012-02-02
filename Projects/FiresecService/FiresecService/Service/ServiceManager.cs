using System;
using System.Configuration;
using System.ServiceModel;
using Common;

namespace FiresecService
{
    public static class FiresecServiceManager
    {
        static ServiceHost _serviceHost;

        public static void Open()
        {
            Close();

            _serviceHost = new ServiceHost(typeof(SafeFiresecService));

            var binding = new NetTcpBinding()
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

            string serverName = ConfigurationManager.AppSettings["ServiceAddress"] as string;
            string machineName = MachineNameHelper.GetMachineName();
            serverName = serverName.Replace("localhost", machineName);
            _serviceHost.AddServiceEndpoint("FiresecAPI.IFiresecService", binding, new Uri(serverName));

            _serviceHost.Open();
        }

        public static void Close()
        {
            if (_serviceHost != null && _serviceHost.State != CommunicationState.Closed && _serviceHost.State != CommunicationState.Closing)
                _serviceHost.Close();
        }
    }
}