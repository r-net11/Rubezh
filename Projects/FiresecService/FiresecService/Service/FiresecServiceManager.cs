using System;
using System.Linq;
using System.Net;
using System.ServiceModel;
using Common;
using FiresecService.ViewModels;
using Infrastructure.Common;
using Infrastructure.Common.BalloonTrayTip;

namespace FiresecService.Service
{
	public static class FiresecServiceManager
	{
		static ServiceHost ServiceHost;
		static FiresecService FiresecService;

		public static bool Open()
		{
			try
			{
				Close();

				FiresecService = new FiresecService();
				ServiceHost = new ServiceHost(FiresecService);

				if (GlobalSettingsHelper.GlobalSettings.Server_EnableRemoteConnections && UACHelper.IsAdministrator)
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
				BalloonHelper.ShowFromServer("Ошибка при запуске хоста сервиса \n" + e.Message);
				return false;
			}
		}

		private static void CreateNetPipesEndpoint()
		{
			try
			{
				var netpipeAddress = "net.pipe://127.0.0.1/FiresecService/";
				ServiceHost.AddServiceEndpoint("FiresecAPI.IFiresecService", Common.BindingHelper.CreateNetNamedPipeBinding(), new Uri(netpipeAddress));
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
					var remoteAddress = "http://" + ipAddress + ":" + GlobalSettingsHelper.GlobalSettings.RemotePort.ToString() + "/FiresecService/";
					ServiceHost.AddServiceEndpoint("FiresecAPI.IFiresecService", Common.BindingHelper.CreateWSHttpBinding(), new Uri(remoteAddress));
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
					var remoteAddress = "net.tcp://" + ipAddress + ":" + GlobalSettingsHelper.GlobalSettings.RemotePort.ToString() + "/FiresecService/";
					ServiceHost.AddServiceEndpoint("FiresecAPI.IFiresecService", Common.BindingHelper.CreateNetTcpBinding(), new Uri(remoteAddress));
					UILogger.Log("Удаленный адрес: " + remoteAddress);
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "FiresecServiceManager.CreateTcpEndpoint");
			}
		}

		public static void Close()
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
				Logger.Error(e, "FiresecServiceManager.GetIPAddress");
				return null;
			}
		}
	}
}