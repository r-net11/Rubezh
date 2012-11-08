using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using Common;
using FiresecService.ViewModels;
using System.Net;

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
                    try
                    {
                        var ipAddress = AppSettings.RemoteAddress;
                        if (string.IsNullOrEmpty(ipAddress))
                        {
                            ipAddress = GetIPAddress();
                        }
                        if (ipAddress != null)
                        {
                            var remoteAddress = "net.tcp://" + ipAddress + ":" + AppSettings.RemotePort.ToString() + "/FiresecService/";
                            _serviceHost.AddServiceEndpoint("FiresecAPI.IFiresecService", tcpBinding, new Uri(remoteAddress));
                            UILogger.Log("Удаленный адрес: " + remoteAddress, false);
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e, "FiresecServiceManager.EnableRemoteConnections");
                    }
                }
                var localAddress = "net.pipe://127.0.0.1/FiresecService/";
                _serviceHost.AddServiceEndpoint("FiresecAPI.IFiresecService", netPipeBinding, new Uri(localAddress));
                UILogger.Log("Локальный адрес: " + localAddress, false);
                _serviceHost.Open();
                return true;
            }
            catch (Exception e)
            {
                Logger.Error(e, "Исключение при вызове FiresecServiceManager.Open");
                UILogger.Log("Ошибка при запуске хоста сервиса: " + e.Message, true);
                return false;
            }
        }

        public static void Close()
        {
            if (_serviceHost != null && _serviceHost.State != CommunicationState.Closed && _serviceHost.State != CommunicationState.Closing)
                _serviceHost.Close();
        }

        static string GetIPAddress()
        {
            try
            {
                var hostName = System.Net.Dns.GetHostName();
                IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(hostName);
                IPAddress[] addresses = ipEntry.AddressList;
                var ipV6Address = addresses.FirstOrDefault(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                return ipV6Address.ToString();
            }
            catch (Exception e)
            {
                Logger.Error(e, "FiresecServiceManager.GetIPAddress");
                return null;
            }
        }
    }
}