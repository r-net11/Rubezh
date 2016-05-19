using Common;
using Infrastructure.Common;
using System;
using System.ServiceModel;


namespace RubezhService.Service
{
	public static class RubezhServiceManager
	{
		static ServiceHost ServiceHost;
		public static SafeRubezhService SafeRubezhService;

		public static void Open(bool isLocalEndpointEnabled = true)
		{
			try
			{
				Close();

				SafeRubezhService = new SafeRubezhService();
				ServiceHost = new ServiceHost(SafeRubezhService);

				if (!GlobalSettingsHelper.GlobalSettings.Server_EnableRemoteConnections)
				{
					Notifier.SetRemoteAddress("<Не разрешено>");
				}
				else if (!UACHelper.IsAdministrator)
				{
					Notifier.SetRemoteAddress("<Нет прав администратора>");
				}
				else
				{
					CreateTcpEndpoint();
				}
				if (isLocalEndpointEnabled)
					CreateNetPipesEndpoint();
				else
					Notifier.SetLocalAddress("<Не разрешено>");
				ServiceHost.Open();
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове RubezhServiceManager.Open");
				Notifier.BalloonShowFromServer("Ошибка при запуске хоста сервиса \n" + e.Message);
				Notifier.UILog("Ошибка при запуске хоста сервиса: " + e.Message, true);
				Notifier.SetLocalAddress("<Ошибка>");
				Notifier.SetRemoteAddress("<Ошибка>");
			}
		}

		static void CreateNetPipesEndpoint()
		{
			try
			{
				var address = "net.pipe://127.0.0.1/RubezhService/";
				ServiceHost.AddServiceEndpoint("RubezhAPI.IRubezhService", Common.BindingHelper.CreateNetNamedPipeBinding(), new Uri(address));
				Notifier.SetLocalAddress(address);
			}
			catch (Exception e)
			{
				Logger.Error(e, "RubezhServiceManager.CreateNetPipesEndpoint");
			}
		}

		static void CreateHttpEndpoint()
		{
			try
			{
				var ipAddress = ConnectionSettingsManager.GetIPAddress();
				if (ipAddress != null)
				{
					var address = "http://" + ipAddress + ":" + GlobalSettingsHelper.GlobalSettings.RemotePort.ToString() + "/RubezhService/";
					ServiceHost.AddServiceEndpoint("RubezhAPI.IRubezhService", Common.BindingHelper.CreateWSHttpBinding(), new Uri(address));
					Notifier.SetRemoteAddress(address);
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "RubezhServiceManager.CreateHttpEndpoint");
			}
		}

		static void CreateTcpEndpoint()
		{
			try
			{
				var ipAddress = ConnectionSettingsManager.GetIPAddress();
				if (ipAddress != null)
				{
					var address = "net.tcp://" + ipAddress + ":" + GlobalSettingsHelper.GlobalSettings.RemotePort.ToString() + "/RubezhService/";
					ServiceHost.AddServiceEndpoint("RubezhAPI.IRubezhService", Common.BindingHelper.CreateNetTcpBinding(), new Uri(address));
					Notifier.SetRemoteAddress(address);
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "RubezhServiceManager.CreateTcpEndpoint");
			}
		}

		public static void Close()
		{
			if (ServiceHost != null && ServiceHost.State != CommunicationState.Closed && ServiceHost.State != CommunicationState.Closing)
				ServiceHost.Close();
		}
	}
}