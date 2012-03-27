using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Net.NetworkInformation;
using FiresecClient;
using System.Net;
using System.Net.Sockets;

namespace FireAdministrator.ViewModels
{
    public class ConnectHelper
    {
        public static bool DefaultConnect()
        {
            var userName = ConnectHelper.DefaultLogin;
            var password = ConnectHelper.DefaultPassword;
            if (userName != null && password != null)
            {
                string clientCallbackAddress = ConnectHelper.ClientCallbackAddress;
                string serverAddress = ConnectHelper.ServiceAddress;
                string message = FiresecManager.Connect(clientCallbackAddress, serverAddress, userName, password);
                if (message == null)
                {
                    return true;
                }
            }
            return false;
        }

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

        static int Port
        {
            get
            {
                Random rnd = new Random();
               
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
