using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Common;
using System.Net;

namespace FSAgent
{
	public class ServerHost
	{
        static ServiceHost _serviceHost;
        static FSAgentContract _fsAgentContract;

        public static bool Open()
        {
            try
            {
                Close();

                _fsAgentContract = new FSAgentContract();
                _fsAgentContract.Run();
                _serviceHost = new ServiceHost(_fsAgentContract);

                var netpipeAddress = "net.pipe://127.0.0.1/FSAgent/";
                _serviceHost.AddServiceEndpoint("FSAgentAPI.IFSAgentContract", Common.BindingHelper.CreateNetNamedPipeBinding(), new Uri(netpipeAddress));
                _serviceHost.Open();
                return true;
            }
            catch (Exception e)
            {
                Logger.Error(e, "Исключение при вызове ServerHost.Open");
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
                Logger.Error(e, "ServerHost.GetIPAddress");
                return null;
            }
        }
	}
}