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

		public static bool Open(ILicenseManager licenseManager)
		{
			try
			{
				Close();

				SafeFiresecService = new SafeFiresecService(licenseManager);
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
                //Logger.Error(e, "Исключение при вызове FiresecServiceManager.Open");
                //UILogger.Log("Ошибка при запуске хоста сервиса: " + e.Message);
                //BalloonHelper.ShowFromServer("Ошибка при запуске хоста сервиса \n" + e.Message); 
                Logger.Error(e, Resources.Language.FiresecServiceManager.OpenLicenseManager_Exception);
                UILogger.Log(Resources.Language.FiresecServiceManager.OpenLicenseManagerLog_Error + e.Message);
                BalloonHelper.ShowFromServer(Resources.Language.FiresecServiceManager.OpenLicenseManagerBalloon_Error + e.Message);
				return false;
			}
		}

		private static void CreateNetPipesEndpoint()
		{
			try
			{
				var netpipeAddress = AppServerConnectionManager.ServerNamedPipesUri;
				ServiceHost.AddServiceEndpoint("FiresecAPI.IFiresecService", BindingHelper.CreateNetNamedPipeBinding(), new Uri(netpipeAddress));
				//UILogger.Log("Локальный адрес: " + netpipeAddress);
                UILogger.Log(String.Format(Resources.Language.FiresecServiceManager.CreateNetPipesEndpoint_Address, netpipeAddress));
			}
			catch (Exception e)
			{
				//Logger.Error(e, "FiresecServiceManager.CreateNetPipesEndpoint");
                Logger.Error(e, Resources.Language.FiresecServiceManager.CreateNetPipesEndpoint_Exception);
			}
		}

		private static void CreateHttpEndpoint()
		{
			try
			{
				var remoteAddress = AppServerConnectionManager.ServerHttpUri;
				ServiceHost.AddServiceEndpoint("FiresecAPI.IFiresecService", BindingHelper.CreateWSHttpBinding(), new Uri(remoteAddress));
                //UILogger.Log("Удаленный адрес: " + remoteAddress);
                UILogger.Log(String.Format(Resources.Language.FiresecServiceManager.CreateHttpEndpoint_Address, remoteAddress));

			}
			catch (Exception e)
			{
                //Logger.Error(e, "FiresecServiceManager.CreateHttpEndpoint");
                Logger.Error(e, Resources.Language.FiresecServiceManager.CreateHttpEndpoint_Exception);
			}
		}

		private static void CreateTcpEndpoint()
		{
			try
			{
				var remoteAddress = AppServerConnectionManager.ServerTcpUri;
				ServiceHost.AddServiceEndpoint("FiresecAPI.IFiresecService", BindingHelper.CreateNetTcpBinding(), new Uri(remoteAddress));
                //UILogger.Log("Удаленный адрес: " + remoteAddress);
                UILogger.Log(String.Format(Resources.Language.FiresecServiceManager.CreateTcpEndpoint_Address, remoteAddress));
			}
			catch (Exception e)
			{
				//Logger.Error(e, "FiresecServiceManager.CreateTcpEndpoint");
                Logger.Error(e, Resources.Language.FiresecServiceManager.CreateTcpEndpoint_Exception);
            }
		}

		public static void Close()
		{
			if (ServiceHost != null && ServiceHost.State != CommunicationState.Closed && ServiceHost.State != CommunicationState.Closing)
				ServiceHost.Close();
		}
	}
}