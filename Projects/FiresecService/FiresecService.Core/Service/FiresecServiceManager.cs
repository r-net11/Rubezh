using Common;
using Infrastructure.Common;
using System;
using System.ServiceModel;


namespace FiresecService.Service
{
	public static class FiresecServiceManager
	{
		static ServiceHost ServiceHost;
		public static SafeFiresecService SafeFiresecService;

		public static void Open(bool isLocalEndpointEnabled = true)
		{
			try
			{
				Close();

				SafeFiresecService = new SafeFiresecService();
				ServiceHost = new ServiceHost(SafeFiresecService);

				if (!GlobalSettingsHelper.GlobalSettings.Server_EnableRemoteConnections)
				{
					Notifier.SetRemoteAddress("<Не разрешено>");
				}
				else if (isLocalEndpointEnabled && !UACHelper.IsAdministrator)
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
				Logger.Error(e, "Исключение при вызове FiresecServiceManager.Open");
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
				var address = "net.pipe://127.0.0.1/FiresecService/";
				ServiceHost.AddServiceEndpoint("RubezhAPI.IFiresecService", Common.BindingHelper.CreateNetNamedPipeBinding(), new Uri(address));
				Notifier.SetLocalAddress(address);
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
				var ipAddress = ConnectionSettingsManager.GetIPAddress();
				if (ipAddress != null)
				{
					var address = "http://" + ipAddress + ":" + GlobalSettingsHelper.GlobalSettings.RemotePort.ToString() + "/FiresecService/";
					ServiceHost.AddServiceEndpoint("RubezhAPI.IFiresecService", Common.BindingHelper.CreateWSHttpBinding(), new Uri(address));
					Notifier.SetRemoteAddress(address);
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "FiresecServiceManager.CreateHttpEndpoint");
			}
		}

		static void CreateTcpEndpoint()
		{
			try
			{
				var ipAddress = ConnectionSettingsManager.GetIPAddress();
				if (ipAddress != null)
				{
					var address = "net.tcp://" + ipAddress + ":" + GlobalSettingsHelper.GlobalSettings.RemotePort.ToString() + "/FiresecService/";
					ServiceHost.AddServiceEndpoint("RubezhAPI.IFiresecService", Common.BindingHelper.CreateNetTcpBinding(), new Uri(address));
					Notifier.SetRemoteAddress(address);
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
	}
}