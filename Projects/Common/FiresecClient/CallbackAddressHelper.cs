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
            //freePort = 9245;
            return "net.tcp://localhost:" + freePort + "/FiresecCallbackService/";
        }

        //static int Port
        //{
        //    get
        //    {
        //        var rnd = new Random();

        //        string host = "localhost";
        //        IPAddress addr = (IPAddress)Dns.GetHostAddresses(host)[0];
        //        int port = rnd.Next(9000, 9100);
        //        while (true)
        //        {
        //            try
        //            {
        //                TcpListener tcpList = new TcpListener(addr, port);
        //                tcpList.Start();
        //                tcpList.Stop();
        //                return port;
        //            }
        //            catch (SocketException e)
        //            {
        //                port = rnd.Next(9000, 9100);
        //            }
        //        }
        //    }
        //}

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