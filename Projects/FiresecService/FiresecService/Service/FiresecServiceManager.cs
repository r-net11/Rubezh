using System.Linq;
using Common;
using Infrastructure.Common;
using System;
using System.ServiceModel;
using KeyGenerator;
using StrazhService;

namespace FiresecService.Service
{
	public static class FiresecServiceManager
	{
		private static ServiceHost _serviceHost;
		public static SafeFiresecService SafeFiresecService;

		public static bool Open(ILicenseManager licenseManager)
		{
			try
			{
				Close();

				SafeFiresecService = new SafeFiresecService(licenseManager);
				_serviceHost = new ServiceHost(SafeFiresecService);

				if (AppServerSettingsHelper.AppServerSettings.EnableRemoteConnections && UACHelper.IsAdministrator)
				{
					CreateTcpEndpoint();
				}
				CreateNetPipesEndpoint();
				_serviceHost.Open();
				return true;
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове FiresecServiceManager.Open");
				Notifier.Log("Ошибка при запуске хоста сервиса: " + e.Message);
				Notifier.BalloonShowFromServer("Ошибка при запуске хоста сервиса \n" + e.Message);
				return false;
			}
		}

		private static void CreateNetPipesEndpoint()
		{
			try
			{
				var netpipeAddress = AppServerConnectionManager.ServerNamedPipesUri;
				_serviceHost.AddServiceEndpoint("StrazhAPI.IFiresecService", BindingHelper.CreateNetNamedPipeBinding(), new Uri(netpipeAddress));
				Notifier.Log("Локальный адрес: " + netpipeAddress);
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
				var remoteAddress = AppServerConnectionManager.ServerHttpUri;
				_serviceHost.AddServiceEndpoint("StrazhAPI.IFiresecService", BindingHelper.CreateWSHttpBinding(), new Uri(remoteAddress));
				Notifier.Log("Удаленный адрес: " + remoteAddress);
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
				var remoteAddress = AppServerConnectionManager.ServerTcpUri;
				_serviceHost.AddServiceEndpoint("StrazhAPI.IFiresecService", BindingHelper.CreateNetTcpBinding(), new Uri(remoteAddress));
				Notifier.Log("Удаленный адрес: " + remoteAddress);
			}
			catch (Exception e)
			{
				Logger.Error(e, "FiresecServiceManager.CreateTcpEndpoint");
			}
		}

		public static void Close()
		{
			if (_serviceHost != null && _serviceHost.State != CommunicationState.Closed && _serviceHost.State != CommunicationState.Closing)
				_serviceHost.Close();
		}
	}
}