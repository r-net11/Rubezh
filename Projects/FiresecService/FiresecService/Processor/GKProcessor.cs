using System;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecAPI.Journal;
using FiresecClient;
using GKProcessor;
using Infrastructure.Common;
using FiresecService.Processor;

namespace FiresecService
{
	public static class GKProcessor
	{
		public static void Create()
		{
			GKProcessorManager.GKProgressCallbackEvent -= new Action<GKProgressCallback>(OnGKProgressCallbackEvent);
			GKProcessorManager.GKProgressCallbackEvent += new Action<GKProgressCallback>(OnGKProgressCallbackEvent);
			GKProcessorManager.GKCallbackResultEvent -= new Action<GKCallbackResult>(OnGKCallbackResultEvent);
			GKProcessorManager.GKCallbackResultEvent += new Action<GKCallbackResult>(OnGKCallbackResultEvent);
		}

		public static void Start()
		{
			GKProcessorManager.MustMonitor = true;
			GKProcessorManager.Start();
			GKLicenseProcessor.Start();
		}

		public static void Stop()
		{
			GKProcessorManager.Stop();
		}

		public static void SetNewConfig()
		{
			var allHashesAreEqual = true;
			if (GKManager.DeviceConfiguration.RootDevice.Children.Count == GKManager.DeviceConfiguration.RootDevice.Children.Count)
			{
				for (int i = 0; i < GKManager.DeviceConfiguration.RootDevice.Children.Count; i++)
				{
					var hash1 = GKFileInfo.CreateHash1(GKManager.DeviceConfiguration, GKManager.DeviceConfiguration.RootDevice.Children[i]);
					var hash2 = GKFileInfo.CreateHash1(GKManager.DeviceConfiguration, GKManager.DeviceConfiguration.RootDevice.Children[i]);
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

			GKProcessorManager.AddGKMessage(JournalEventNameType.Применение_конфигурации, JournalEventDescriptionType.NULL, "", null, null);
			//if (!allHashesAreEqual)
			{
				Stop();
				Create();
				Start();
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
				foreach (var journalItem in gkCallbackResult.JournalItems)
				{
					FiresecService.Service.FiresecService.AddGKJournalItem(journalItem);
				}
			}
			FiresecService.Service.FiresecService.NotifyGKObjectStateChanged(gkCallbackResult);

			AutomationProcessorRunner.RunOnStateChanged();
		}
	}
}