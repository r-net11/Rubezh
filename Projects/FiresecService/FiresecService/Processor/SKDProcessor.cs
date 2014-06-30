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
				foreach (var deviceProcessor in ChinaSKDDriver.Processor.DeviceProcessors)
				{
					deviceProcessor.Wrapper.NewJournalItem -= new Action<ChinaSKDDriverAPI.SKDJournalItem>(Wrapper_NewJournalItem);
					deviceProcessor.Wrapper.NewJournalItem += new Action<ChinaSKDDriverAPI.SKDJournalItem>(Wrapper_NewJournalItem);

					ChinaSKDDriver.Processor.SKDCallbackResultEvent -= new Action<SKDCallbackResult>(OnSKDCallbackResultEvent);
					ChinaSKDDriver.Processor.SKDCallbackResultEvent += new Action<SKDCallbackResult>(OnSKDCallbackResultEvent);
				}
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

		static void Wrapper_NewJournalItem(ChinaSKDDriverAPI.SKDJournalItem skdJournalItem)
		{
			var skdCallbackResult = new SKDCallbackResult();
			var journalItem = new JournalItem();
			journalItem.SystemDateTime = skdJournalItem.SystemDateTime;
			journalItem.DeviceDateTime = skdJournalItem.DeviceDateTime;
			journalItem.NameText = skdJournalItem.Name;
			journalItem.DescriptionText = skdJournalItem.Description;
			skdCallbackResult.JournalItems.Add(journalItem);

			var journalItems = new List<JournalItem>();
			journalItems.Add(journalItem);
			FiresecService.Service.FiresecService.NotifyNewJournalItems(journalItems);
		}

		static void OnSKDCallbackResultEvent(SKDCallbackResult skdCallbackResult)
		{
			FiresecService.Service.FiresecService.NotifySKDObjectStateChanged(skdCallbackResult);
		}

		public static void SetNewConfig()
		{
			Stop();
			Create();
		}
	}
}