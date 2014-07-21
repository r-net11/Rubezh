using System;
using System.Collections.Generic;
using Common;
using FiresecAPI;
using FiresecAPI.Journal;
using FiresecAPI.SKD;

namespace FiresecService
{
	public static class SKDProcessor
	{
		static SKDProcessor()
		{
#if DEBUG
			try
			{
				System.IO.File.Copy(@"..\..\..\ChinaController\CPPWrapper\Bin\CPPWrapper.dll", @"CPPWrapper.dll", true);
			}
			catch { }
#endif
		}

		public static void Start()
		{
//#if DEBUG
//            for (int i = 0; i < 1000; i++)
//            {
//                var journalItem = new JournalItem();
//                journalItem.SystemDateTime = DateTime.Now.AddMinutes(-i);
//                journalItem.DeviceDateTime = DateTime.Now.AddMinutes(-i);
//                journalItem.JournalEventNameType = JournalEventNameType.Проход;
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
					deviceProcessor.NewJournalItem -= new Action<JournalItem>(OnNewJournalItem);
					deviceProcessor.NewJournalItem += new Action<JournalItem>(OnNewJournalItem);
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

		static void OnNewJournalItem(JournalItem journalItem)
		{
			journalItem.StateClass = EventDescriptionAttributeHelper.ToStateClass(journalItem.JournalEventNameType);
			FiresecService.Service.FiresecService.AddJournalItem(journalItem);
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