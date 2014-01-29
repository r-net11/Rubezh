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

				GKProcessorManager.GKProgressCallbackEvent -= new Action<GKProgressCallback>(OnGKProgressCallbackEvent);
				GKProcessorManager.GKProgressCallbackEvent += new Action<GKProgressCallback>(OnGKProgressCallbackEvent);
				GKProcessorManager.GKCallbackResultEvent -= new Action<GKCallbackResult>(OnGKCallbackResultEvent);
				GKProcessorManager.GKCallbackResultEvent += new Action<GKCallbackResult>(OnGKCallbackResultEvent);
			}
		}

		public static void Start()
		{
			if (GlobalSettingsHelper.GlobalSettings.IsGKAsAService)
			{
				GKProcessorManager.MustMonitor = true;
				GKProcessorManager.Start();
				GKLicenseProcessor.Start();
			}
		}

		public static void Stop()
		{
			if (GlobalSettingsHelper.GlobalSettings.IsGKAsAService)
			{
				GKProcessorManager.Stop();
			}
		}

		public static void SetNewConfig()
		{
			if (GlobalSettingsHelper.GlobalSettings.IsGKAsAService)
			{
				var deviceConfiguration = ZipConfigurationHelper.GetDeviceConfiguration();
				UpdateConfigurationHelper.Update(deviceConfiguration);

				var allHashesAreEqual = true;
				if (deviceConfiguration.RootDevice.Children.Count == XManager.DeviceConfiguration.RootDevice.Children.Count)
				{
					for (int i = 0; i < deviceConfiguration.RootDevice.Children.Count; i++)
					{
						var hash1 = GKFileInfo.CreateHash1(deviceConfiguration, deviceConfiguration.RootDevice.Children[i]);
						var hash2 = GKFileInfo.CreateHash1(XManager.DeviceConfiguration, XManager.DeviceConfiguration.RootDevice.Children[i]);
						if (!GKFileInfo.CompareHashes(hash1, hash2))
						{
							allHashesAreEqual = false;
						}
					}
				}
				else
				{
					allHashesAreEqual = false;
				}

				GKProcessorManager.AddGKMessage(EventName.Применение_конфигурации, "", null, null);
				if (!allHashesAreEqual)
				{
					Stop();
					Create();
					Start();
				}
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