using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Infrastructure.Common;
using Common;
using Infrastructure.Common.BalloonTrayTip;
using System.Net;
using ServerFS2.ViewModels;

namespace ServerFS2.Service
{
	public class FS2ServiceHost
	{
		static ServiceHost ServiceHost;
		static FS2Contracr FS2Contract;

		public static bool Start()
		{
			try
			{
				Stop();

				FS2Contract = new FS2Contracr();
				ServiceHost = new ServiceHost(FS2Contract);

				if (UACHelper.IsAdministrator)
				{
					CreateHttpEndpoint();
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
				var netpipeAddress = "net.pipe://127.0.0.1/FS2/";
				ServiceHost.AddServiceEndpoint("FS2Api.IFS2Contract", Common.BindingHelper.CreateNetNamedPipeBinding(), new Uri(netpipeAddress));
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
					var remoteAddress = "http://" + ipAddress + ":" + GlobalSettingsHelper.GlobalSettings.RemotePort.ToString() + "/FS2/";
					ServiceHost.AddServiceEndpoint("FS2Api.IFS2Contract", Common.BindingHelper.CreateWSHttpBinding(), new Uri(remoteAddress));
					UILogger.Log("Удаленный адрес: " + remoteAddress);
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