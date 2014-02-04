using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI;
using FiresecService.Processor;
using SKDDriver;

namespace FiresecService
{
	public static class SKDProcessor
	{
		public static void Create()
		{
			SKDManager.SKDConfiguration = ZipConfigurationHelper.GetSKDConfiguration();
			SKDManager.CreateDrivers();
			SKDManager.UpdateConfiguration();
			WatcherManager.Start();

			SKDProcessorManager.SKDCallbackResultEvent -= new Action<SKDCallbackResult>(OnSKDCallbackResultEvent);
			SKDProcessorManager.SKDCallbackResultEvent += new Action<SKDCallbackResult>(OnSKDCallbackResultEvent);
		}

		static void OnSKDCallbackResultEvent(SKDCallbackResult skdCallbackResult)
		{
			FiresecService.Service.FiresecService.NotifySKDObjectStateChanged(skdCallbackResult);
		}
	}
}