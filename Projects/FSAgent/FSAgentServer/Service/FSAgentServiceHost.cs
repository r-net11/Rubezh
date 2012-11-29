using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Common;
using System.Net;
using FSAgentServer.ViewModels;

namespace FSAgentServer
{
	public class FSAgentServiceHost
	{
		static ServiceHost ServiceHost;
		static FSAgentContract FsAgentContract;

		public static bool Start()
		{
			try
			{
				Stop();

				FsAgentContract = new FSAgentContract();
				ServiceHost = new ServiceHost(FsAgentContract);

				CreateHttpEndpoint();
				CreateNetPipesEndpoint();
				ServiceHost.Open();
				return true;
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове FiresecServiceManager.Open");
				return false;
			}
		}

		static void CreateNetPipesEndpoint()
		{
			try
			{
				var netpipeAddress = "net.pipe://127.0.0.1/FSAgent/";
				ServiceHost.AddServiceEndpoint("FSAgentAPI.IFSAgentContract", Common.BindingHelper.CreateNetNamedPipeBinding(), new Uri(netpipeAddress));
				UILogger.Log("Локальный адрес: " + netpipeAddress, false);
			}
			catch (Exception e)
			{
				Logger.Error(e, "FiresecServiceManager.CreateNetPipesEndpoint");
			}
		}

		static void CreateHttpEndpoint()
		{
			try
			{
				var ipAddress = GetIPAddress();
				if (ipAddress != null)
				{
					var remoteAddress = "http://" + ipAddress + ":" + AppSettings.RemotePort.ToString() + "/FSAgent/";
					ServiceHost.AddServiceEndpoint("FSAgentAPI.IFSAgentContract", Common.BindingHelper.CreateWSHttpBinding(), new Uri(remoteAddress));
					UILogger.Log("Удаленный адрес: " + remoteAddress, false);
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "FiresecServiceManager.CreateHttpEndpoint");
			}
		}

		public static void Stop()
		{
			if (ServiceHost != null && ServiceHost.State != CommunicationState.Closed && ServiceHost.State != CommunicationState.Closing)
				ServiceHost.Close();
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