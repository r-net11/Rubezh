using System;
using System.ServiceModel;
using System.Threading;
using Common;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;

namespace FiresecClient
{
    public static class CallbackAddressHelper
    {
        public static string GetFreeClientCallbackAddress()
        {
            var freePort = FindNextAvailablePort(9000);
            return "net.tcp://localhost:" + freePort + "/FiresecCallbackService/";
        }
        
        private const string PortReleaseGuid = "8875BD8E-4D5B-11DE-B2F4-691756D89593";

        static int FindNextAvailablePort(int startPort)
        {
            int port = startPort;
            bool isAvailable = true;

            var mutex = new Mutex(false,
                string.Concat("Global/", PortReleaseGuid));
            mutex.WaitOne();
            try
            {
                IPGlobalProperties ipGlobalProperties =
                    IPGlobalProperties.GetIPGlobalProperties();
                IPEndPoint[] endPoints =
                    ipGlobalProperties.GetActiveTcpListeners();

                do
                {
                    if (!isAvailable)
                    {
                        port++;
                        isAvailable = true;
                    }

                    foreach (IPEndPoint endPoint in endPoints)
                    {
                        if (endPoint.Port != port) continue;
                        isAvailable = false;
                        break;
                    }

                } while (!isAvailable && port < IPEndPoint.MaxPort);

                if (!isAvailable)
                    throw new Exception("Нет свободных портов для ответного сервера");

                return port;
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }
    }
}