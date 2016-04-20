using System;
using System.Linq;
using System.Net;
using System.ServiceModel;
using Common;
using FSAgentServer.ViewModels;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.BalloonTrayTip;

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

				if (UACHelper.IsAdministrator)
				{
					//CreateHttpEndpoint();
					CreateTcpEndpoint();
				}
				CreateNetPipesEndpoint();
				ServiceHost.Open();
				return true;
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове FiresecServiceManager.Open");
				UILogger.Log("Ошибка при запуске хоста сервиса: " + e.Message);
				BalloonHelper.ShowFromAgent("Ошибка при запуске хоста сервиса " + e.Message);
				return false;
			}
		}

		private static void CreateNetPipesEndpoint()
		{
			try
			{
				var netpipeAddress = "net.pipe://127.0.0.1/FSAgent/";
				ServiceHost.AddServiceEndpoint("FSAgentAPI.IFSAgentContract", Common.BindingHelper.CreateNetNamedPipeBinding(), new Uri(netpipeAddress));
				UILogger.Log("Локальный адрес: " + netpipeAddress);
			}
			catch (Exception e)
			{
				Logger.Error(e, "FiresecServiceManager.CreateNetPipesEndpoint");
			}
		}

		private static void CreateHttpEndpoint()
		{
			try
			{
				var ipAddress = GetIPAddress();
				if (ipAddress != null)
				{
					var remoteAddress = "http://" + ipAddress + ":" + GlobalSettingsHelper.GlobalSettings.RemotePort.ToString() + "/FSAgent/";
					ServiceHost.AddServiceEndpoint("FSAgentAPI.IFSAgentContract", Common.BindingHelper.CreateWSHttpBinding(), new Uri(remoteAddress));
					UILogger.Log("Удаленный адрес: " + remoteAddress);
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "FiresecServiceManager.CreateHttpEndpoint");
			}
		}

		private static void CreateTcpEndpoint()
		{
			try
			{
				var ipAddress = GetIPAddress();
				if (ipAddress != null)
				{
					var remoteAddress = "net.tcp://" + ipAddress + ":" + (GlobalSettingsHelper.GlobalSettings.RemotePort + 1).ToString() + "/FSAgent/";
					ServiceHost.AddServiceEndpoint("FSAgentAPI.IFSAgentContract", Common.BindingHelper.CreateNetTcpBinding(), new Uri(remoteAddress));
					UILogger.Log("Удаленный адрес: " + remoteAddress);
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "FiresecServiceManager.CreateTcpEndpoint");
			}
		}

		public static void Stop()
		{
			if (ServiceHost != null && ServiceHost.State != CommunicationState.Closed && ServiceHost.State != CommunicationState.Closing)
				ServiceHost.Close();
		}

		private static string GetIPAddress()
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