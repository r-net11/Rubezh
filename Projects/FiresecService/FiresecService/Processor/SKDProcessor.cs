using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI;
using FiresecService.Processor;
using SKDDriver;
using Common;

namespace FiresecService
{
	public static class SKDProcessor
	{
		public static void Create()
		{
			try
			{
				SKDManager.SKDConfiguration = ZipConfigurationHelper.GetSKDConfiguration();
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