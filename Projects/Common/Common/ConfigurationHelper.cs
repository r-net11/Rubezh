using System;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using FiresecClient;
using Infrastructure;

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

        }

        public static string LibVlcDllsPath
        {
            get { return ConfigurationManager.AppSettings["LibVlcDllsPath"] as string; }
        }

        public static bool ShowOnlyVideo
        {
            get { return Convert.ToBoolean(ConfigurationManager.AppSettings["ShowOnlyVideo"] as string); }
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