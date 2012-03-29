using System;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using FiresecClient;

namespace Common
{
    public static class ConfigurationHelper
    {
        public static string ClientCallbackAddress
        {
            get
            {
                return "net.tcp://localhost:" + Port + "/FiresecCallbackService/";
            }
        }

        public static string ServiceAddress
        {
            get
            {
                return ConfigurationManager.AppSettings["ServiceAddress"] as string;
            }
        }

        public static string DefaultLogin
        {
            get { return ConfigurationManager.AppSettings["DefaultLogin"] as string; }
        }

        public static string DefaultPassword
        {
            get { return ConfigurationManager.AppSettings["DefaultPassword"] as string; }
        }

        public static string LibVlcDllsPath
        {
            get { return ConfigurationManager.AppSettings["LibVlcDllsPath"] as string; }
        }

        public static bool ShowOnlyVideo
        {
            get { return Convert.ToBoolean(ConfigurationManager.AppSettings["ShowOnlyVideo"] as string); }
        }

        public static bool AutoConnect
        {
            get { return Convert.ToBoolean(ConfigurationManager.AppSettings["AutoConnect"] as string); }
        }

        static int Port
        {
            get
            {
                var rnd = new Random();
               
                string host = "localhost";
                IPAddress addr = (IPAddress)Dns.GetHostAddresses(host)[0];
                int port = rnd.Next(9000, 9100);
                while (true)
                {
                    try
                    {
                        TcpListener tcpList = new TcpListener(addr, port);
                        tcpList.Start();
                        tcpList.Stop();
                        return port;
                    }
                    catch (SocketException e)
                    {
                        port = rnd.Next(9000, 9100);
                    }
                }
            }
        }
    }
}
