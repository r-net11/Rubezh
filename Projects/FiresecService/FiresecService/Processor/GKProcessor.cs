using System;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecClient;
using FiresecService.Processor;
using GKProcessor;
using Infrastructure.Common;

namespace FiresecService
{
	public static class GKProcessor
	{
		public static void Create()
		{
			//if (GlobalSettingsHelper.GlobalSettings.IsGKAsAService)
			{
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
				var allHashesAreEqual = true;
				if (XManager.DeviceConfiguration.RootDevice.Children.Count == XManager.DeviceConfiguration.RootDevice.Children.Count)
				{
					for (int i = 0; i < XManager.DeviceConfiguration.RootDevice.Children.Count; i++)
					{
						var hash1 = GKFileInfo.CreateHash1(XManager.DeviceConfiguration, XManager.DeviceConfiguration.RootDevice.Children[i]);
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

				GKProcessorManager.AddGKMessage(FiresecAPI.Journal.JournalEventNameType.Применение_конфигурации, "", null, null);
				//if (!allHashesAreEqual)
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
			if (gkCallbackResult.JournalItems.Count > 0)
			{
				foreach(var journalItem in gkCallbackResult.JournalItems)
				{
					FiresecService.Service.FiresecService.AddGKGlobalJournalItem(journalItem);
				}
			}
			FiresecService.Service.FiresecService.NotifyGKObjectStateChanged(gkCallbackResult);
		}
	}
}