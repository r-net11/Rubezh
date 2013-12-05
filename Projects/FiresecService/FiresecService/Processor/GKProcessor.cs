using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFiresecAPI;
using FiresecService.Processor;
using FiresecClient;
using GKProcessor;
using Infrastructure.Common;
using FiresecAPI;

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
				DescriptorsManager.CreateDynamicObjectsInXManager();
				GKProcessorManager.GKProgressCallbackEvent += new Action<GKProgressCallback>(OnGKProgressCallbackEvent);
				GKProcessorManager.GKCallbackResultEvent += new Action<GKCallbackResult>(OnGKCallbackResultEvent);
			}
		}

		public static void Start()
		{
			if (GlobalSettingsHelper.GlobalSettings.IsGKAsAService)
			{
				GKProcessorManager.Start();
			}
		}

		public static void Stop()
		{
			if (GlobalSettingsHelper.GlobalSettings.IsGKAsAService)
			{
				GKProcessorManager.Stop();
			}
		}

		static void OnGKProgressCallbackEvent(GKProgressCallback gkProgressCallback)
		{
			FiresecService.Service.FiresecService.NotifyGKProgress(gkProgressCallback);
		}

		static void OnGKCallbackResultEvent(GKCallbackResult gkCallbackResult)
		{
			FiresecService.Service.FiresecService.NotifyGKObjectStateChanged(gkCallbackResult);
		}
	}
}