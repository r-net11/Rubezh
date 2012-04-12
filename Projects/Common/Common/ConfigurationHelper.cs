using System;
using System.Configuration;
using System.Net;
using System.Net.Sockets;

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