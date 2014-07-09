using System;
using System.Collections.Generic;
using Common;
using FiresecAPI.SKD;
using FiresecService.Processor;
using SKDDriver;
using FiresecAPI;

namespace FiresecService
{
	public static class SKDProcessor
	{
		public static void Start()
		{
//#if DEBUG
//            for (int i = 0; i < 1000; i++)
//            {
//                var journalItem = new JournalItem();
//                journalItem.SystemDateTime = DateTime.Now.AddMinutes(-i);
//                journalItem.DeviceDateTime = DateTime.Now.AddMinutes(-i);
//                journalItem.Name = FiresecAPI.Events.GlobalEventNameEnum.Проход;
//                journalItem.DescriptionText = "Description " + i;
//                FiresecService.Service.FiresecService.AddGlobalJournalItem(journalItem);
//            }
//#endif

			try
			{
				if (SKDManager.SKDConfiguration != null)
				{
					SKDManager.CreateDrivers();
					SKDManager.UpdateConfiguration();
				}
				ChinaSKDDriver.Processor.Start();
				foreach (var deviceProcessor in ChinaSKDDriver.Processor.DeviceProcessors)
				{
					deviceProcessor.Wrapper.NewJournalItem -= new Action<ChinaSKDDriverAPI.SKDJournalItem>(OnNewSKDJournalItem);
					deviceProcessor.Wrapper.NewJournalItem += new Action<ChinaSKDDriverAPI.SKDJournalItem>(OnNewSKDJournalItem);
				}

				ChinaSKDDriver.Processor.NewJournalItem -= new Action<JournalItem>(OnNewJournalItem);
				ChinaSKDDriver.Processor.NewJournalItem += new Action<JournalItem>(OnNewJournalItem);

				ChinaSKDDriver.Processor.SKDCallbackResultEvent -= new Action<SKDCallbackResult>(OnSKDCallbackResultEvent);
				ChinaSKDDriver.Processor.SKDCallbackResultEvent += new Action<SKDCallbackResult>(OnSKDCallbackResultEvent);

				ChinaSKDDriver.Processor.GKProgressCallbackEvent -= new Action<GKProgressCallback>(OnGKProgressCallbackEvent);
				ChinaSKDDriver.Processor.GKProgressCallbackEvent += new Action<GKProgressCallback>(OnGKProgressCallbackEvent);
			}
			catch (Exception e)
			{
				Logger.Error(e, "SKDProcessor.Create");
			}
		}

		public static void Stop()
		{
			ChinaSKDDriver.Processor.Stop();
		}

		static void OnNewSKDJournalItem(ChinaSKDDriverAPI.SKDJournalItem skdJournalItem)
		{
			var journalItem = new JournalItem();
			journalItem.SystemDateTime = skdJournalItem.SystemDateTime;
			journalItem.DeviceDateTime = skdJournalItem.DeviceDateTime;
			journalItem.Name = skdJournalItem.EventNameType;
			journalItem.DescriptionText = skdJournalItem.Description;
			OnNewJournalItem(journalItem);
		}

		static void OnNewJournalItem(JournalItem journalItem)
		{
			FiresecService.Service.FiresecService.AddGlobalJournalItem(journalItem);
			var journalItems = new List<JournalItem>();
			journalItems.Add(journalItem);
			FiresecService.Service.FiresecService.NotifyNewJournalItems(journalItems);
		}

		static void OnSKDCallbackResultEvent(SKDCallbackResult skdCallbackResult)
		{
			FiresecService.Service.FiresecService.NotifySKDObjectStateChanged(skdCallbackResult);
		}

		static void OnGKProgressCallbackEvent(GKProgressCallback gkProgressCallback)
		{
			FiresecService.Service.FiresecService.NotifyGKProgress(gkProgressCallback);
		}

		public static void SetNewConfig()
		{
			Stop();
			Start();
		}
	}
}