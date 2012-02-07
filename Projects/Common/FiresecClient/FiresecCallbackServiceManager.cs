using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Common;
using System.Threading;

namespace FiresecClient
{
    public static class FiresecCallbackServiceManager
    {
        static ServiceHost _serviceHost;
        static string _clientCallbackAddress;

        public static void Open(string clientCallbackAddress)
        {
            _clientCallbackAddress = clientCallbackAddress;
            Thread thread = new Thread(OnOpen);
            thread.Start();
        }

        public static void OnOpen()
        {
            Close();

            _serviceHost = new ServiceHost(typeof(FiresecCallbackService));

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

            string machineName = MachineNameHelper.GetMachineName();
            _clientCallbackAddress = _clientCallbackAddress.Replace("localhost", machineName);
            _serviceHost.AddServiceEndpoint("FiresecAPI.IFiresecCallbackService", binding, new Uri(_clientCallbackAddress));

            _serviceHost.Open();
        }

        public static void Close()
        {
            if (_serviceHost != null && _serviceHost.State != CommunicationState.Closed && _serviceHost.State != CommunicationState.Closing)
                _serviceHost.Close();
        }
    }
}
