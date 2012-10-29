using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using Common;
using FiresecService.ViewModels;

namespace FiresecService.Service
{
    public static class FiresecServiceManager
    {
        static ServiceHost _serviceHost;

        public static bool Open()
        {
            try
            {
                Close();

                _serviceHost = new ServiceHost(typeof(SafeFiresecService));

                var netPipeBinding = Common.BindingHelper.CreateNetNamedPipeBinding();
                var tcpBinding = Common.BindingHelper.CreateNetTcpBinding();

#if DEBUG
                var behavior = _serviceHost.Description.Behaviors.Find<ServiceDebugBehavior>();
                behavior.IncludeExceptionDetailInFaults = true;
#endif
                if (AppSettings.EnableRemoteConnections)
                {
                    _serviceHost.AddServiceEndpoint("FiresecAPI.IFiresecService", tcpBinding, new Uri("net.tcp://127.0.0.1:" + AppSettings.Port.ToString() + "/FiresecService/"));
                }
                _serviceHost.AddServiceEndpoint("FiresecAPI.IFiresecService", netPipeBinding, new Uri("net.pipe://127.0.0.1/FiresecService/"));
                _serviceHost.Open();
                return true;
            }
            catch (Exception e)
            {
                Logger.Error(e, "Исключение при вызове FiresecServiceManager.Open");
                UILogger.Log("Ошибка при запуске хоста сервиса", true);
                return false;
            }
        }

        public static void Close()
        {
            if (_serviceHost != null && _serviceHost.State != CommunicationState.Closed && _serviceHost.State != CommunicationState.Closing)
                _serviceHost.Close();
        }
    }
}