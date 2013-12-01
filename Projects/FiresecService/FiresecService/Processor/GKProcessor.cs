using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFiresecAPI;
using FiresecService.Processor;
using FiresecClient;
using GKProcessor;
using Infrastructure.Common;

namespace FiresecService
{
	public static class GKProcessor
	{
		public static void Create()
		{
			if (GlobalSettingsHelper.GlobalSettings.IsGKAsAService)
			{
				XManager.DeviceConfiguration = ZipConfigurationHelper.GetDeviceConfiguration();
				GKDriversCreator.Create();
				XManager.UpdateConfiguration();
				XManager.CreateStates();
				DescriptorsManager.Create();
				foreach (var gkDatabase in DescriptorsManager.GkDatabases)
				{
					foreach (var delay in gkDatabase.Delays)
					{
						delay.DelayState = new XDelayState();
					}
				}
			}
		}

		public static void Start()
		{
			if (GlobalSettingsHelper.GlobalSettings.IsGKAsAService)
			{
				WatcherManager.Start();
			}
		}

		public static void Stop()
		{
			if (GlobalSettingsHelper.GlobalSettings.IsGKAsAService)
			{
				WatcherManager.Stop();
			}
		}
	}
}