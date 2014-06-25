using System;
using System.Collections.Generic;
using Common;
using FiresecAPI.SKD;
using FiresecService.Processor;
using SKDDriver;

namespace FiresecService
{
	public static class SKDProcessor
	{
		public static void Create()
		{
			try
			{
				var configuration = ZipConfigurationHelper.GetSKDConfiguration();
				SKDManager.SKDConfiguration = configuration;
				if (SKDManager.SKDConfiguration != null)
				{
					SKDManager.CreateDrivers();
					SKDManager.UpdateConfiguration();
				}
				ChinaSKDDriver.Processor.Run(configuration);
			}
			catch (Exception e)
			{
				Logger.Error(e, "SKDProcessor.Create");
			}
		}

		public static void OLD_Create()
		{
			try
			{
				var configuration = ZipConfigurationHelper.GetSKDConfiguration();
				if(configuration.Zones == null)
					configuration.Zones = new List<SKDZone>();
				SKDManager.SKDConfiguration = configuration;
				if (SKDManager.SKDConfiguration != null)
				{
					SKDManager.CreateDrivers();
					SKDManager.UpdateConfiguration();
					WatcherManager.Start();

					SKDProcessorManager.SKDCallbackResultEvent -= new Action<SKDCallbackResult>(OnSKDCallbackResultEvent);
					SKDProcessorManager.SKDCallbackResultEvent += new Action<SKDCallbackResult>(OnSKDCallbackResultEvent);
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "SKDProcessor.Create");
			}
		}

		static void OnSKDCallbackResultEvent(SKDCallbackResult skdCallbackResult)
		{
			FiresecService.Service.FiresecService.NotifySKDObjectStateChanged(skdCallbackResult);
		}
	}
}