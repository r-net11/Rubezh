using System;
using System.ServiceModel;
using Common;
using FiresecService.Models;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.BalloonTrayTip;

namespace FiresecService.Service
{
	public static class FiresecServiceManager
	{
		static ServiceHost ServiceHost;
		public static SafeFiresecService SafeFiresecService;

		public static void Open()
		{
			try
			{
				Close();

				SafeFiresecService = new SafeFiresecService();
				ServiceHost = new ServiceHost(SafeFiresecService);

				if (!GlobalSettingsHelper.GlobalSettings.Server_EnableRemoteConnections)
				{
					MainViewModel.SetRemoteAddress("<Не разрешено>");
				}
				else if (!UACHelper.IsAdministrator)
				{
					MainViewModel.SetRemoteAddress("<Нет прав администратора>");
				}
				else
				{
					CreateTcpEndpoint();
				}
				CreateNetPipesEndpoint();
				ServiceHost.Open();
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове FiresecServiceManager.Open");
				BalloonHelper.ShowFromServer("Ошибка при запуске хоста сервиса \n" + e.Message);
				UILogger.Log("Ошибка при запуске хоста сервиса: " + e.Message);
				MainViewModel.SetLocalAddress("<Ошибка>");
				MainViewModel.SetRemoteAddress("<Ошибка>");
			}
		}

		static void CreateNetPipesEndpoint()
		{
			try
			{
				var address = "net.pipe://127.0.0.1/FiresecService/";
				ServiceHost.AddServiceEndpoint("RubezhAPI.IFiresecService", Common.BindingHelper.CreateNetNamedPipeBinding(), new Uri(address));
				MainViewModel.SetLocalAddress(address);
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
					MainViewModel.SetRemoteAddress(address);
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
					MainViewModel.SetRemoteAddress(address);
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