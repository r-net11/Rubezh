using System.Linq;
using Common;
using FiresecService.ViewModels;
using Infrastructure.Common;
using Infrastructure.Common.BalloonTrayTip;
using System;
using System.ServiceModel;
using KeyGenerator;

namespace FiresecService.Service
{
	public static class FiresecServiceManager
	{
		private static ServiceHost ServiceHost;
		public static SafeFiresecService SafeFiresecService;

	//	public static bool Open(ILicenseManager licenseManager)
		public static bool Open()
		{
			try
			{
				Close();

				//SafeFiresecService = new SafeFiresecService(licenseManager);
				SafeFiresecService = new SafeFiresecService();
				ServiceHost = new ServiceHost(SafeFiresecService);

				if (AppServerSettingsHelper.AppServerSettings.EnableRemoteConnections && UACHelper.IsAdministrator)
				{
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
				var netpipeAddress = AppServerConnectionManager.ServerNamedPipesUri;
				ServiceHost.AddServiceEndpoint("FiresecAPI.IFiresecService", BindingHelper.CreateNetNamedPipeBinding(), new Uri(netpipeAddress));
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
				var remoteAddress = AppServerConnectionManager.ServerHttpUri;
				ServiceHost.AddServiceEndpoint("FiresecAPI.IFiresecService", BindingHelper.CreateWSHttpBinding(), new Uri(remoteAddress));
				UILogger.Log("Удаленный адрес: " + remoteAddress);
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
				ServiceHost.AddServiceEndpoint("FiresecAPI.IFiresecService", BindingHelper.CreateNetTcpBinding(), new Uri(remoteAddress));
				UILogger.Log("Удаленный адрес: " + remoteAddress);
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