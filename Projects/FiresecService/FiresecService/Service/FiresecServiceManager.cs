using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using Common;
using FiresecService.ViewModels;
using System.Net;

namespace FiresecService.Service
{
	public static class FiresecServiceManager
	{
		static ServiceHost _serviceHost;
		static FiresecService _firesecService;

		public static bool Open()
		{
			try
			{
				Close();

				_firesecService = new FiresecService();
				_serviceHost = new ServiceHost(_firesecService);

				if (AppSettings.EnableRemoteConnections)
				{
					try
					{
						var ipAddress = GetIPAddress();
						if (ipAddress != null)
						{
							var remoteAddress = "http://" + ipAddress + ":" + AppSettings.RemotePort.ToString() + "/FiresecService/";
							_serviceHost.AddServiceEndpoint("FiresecAPI.IFiresecService", Common.BindingHelper.CreateWSHttpBinding(), new Uri(remoteAddress));
							UILogger.Log("Удаленный адрес: " + remoteAddress, false);
						}
					}
					catch (Exception e)
					{
						Logger.Error(e, "FiresecServiceManager.EnableRemoteConnections");
					}
				}
				try
				{
					var localAddress = "http://127.0.0.1:" + AppSettings.LocalPort.ToString() + "/FiresecService/";
					_serviceHost.AddServiceEndpoint("FiresecAPI.IFiresecService", Common.BindingHelper.CreateWSHttpBinding(), new Uri(localAddress));
					UILogger.Log("Локальный адрес: " + localAddress, false);
					_serviceHost.Open();
				}
				catch (Exception e)
				{
					Logger.Error(e, "FiresecServiceManager.EnableRemoteConnections");
				}
				return true;
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове FiresecServiceManager.Open");
				UILogger.Log("Ошибка при запуске хоста сервиса: " + e.Message, true);
				return false;
			}
		}

		public static void Close()
		{
			if (_serviceHost != null && _serviceHost.State != CommunicationState.Closed && _serviceHost.State != CommunicationState.Closing)
				_serviceHost.Close();
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
				Logger.Error(e, "FiresecServiceManager.GetIPAddress");
				return null;
			}
		}
	}
}