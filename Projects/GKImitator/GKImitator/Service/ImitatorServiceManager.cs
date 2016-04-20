using System;
using System.ServiceModel;
using Common;
using Infrastructure.Common.Windows.BalloonTrayTip;

namespace GKImitator
{
	public static class ImitatorServiceManager
	{
		static ServiceHost ServiceHost;
		public static ImitatorService ImitatorService;
		
		public static void Open()
		{
			try
			{
				Close();
				ImitatorService = new ImitatorService();
				ServiceHost = new ServiceHost(ImitatorService);
				CreateNetPipesEndpoint();
				ServiceHost.Open();
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове ImitatorServiceManager.Open");
				BalloonHelper.ShowFromServer("Ошибка при запуске хоста имитатора \n" + e.Message);
			}
		}

		static void CreateNetPipesEndpoint()
		{
			try
			{
				var address = "net.pipe://127.0.0.1/GKImitator/";
				ServiceHost.AddServiceEndpoint("RubezhAPI.IImitatorService", BindingHelper.CreateNetNamedPipeBinding(), new Uri(address));
			}
			catch (Exception e)
			{
				Logger.Error(e, "ImitatorServiceManager.CreateNetPipesEndpoint");
			}
		}
		public static void Close()
		{
			if (ServiceHost != null && ServiceHost.State != CommunicationState.Closed && ServiceHost.State != CommunicationState.Closing)
				ServiceHost.Close();
		}
	}
}
