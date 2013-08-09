using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using FiresecAPI.Models;
using FS2Api;
using ServerFS2.Service;
using Device = FiresecAPI.Models.Device;

namespace ServerFS2
{
	public static class ReadJournalOperationHelper
	{
		static readonly object Locker = new object();

		public static FS2JournalItemsCollection GetJournalItemsCollection(Device device)
		{
			var hasGuardJournal = device.Driver.DriverType == DriverType.Rubezh_2OP || device.Driver.DriverType == DriverType.USB_Rubezh_2OP;
			var stageCount = hasGuardJournal ? 2 : 1;
			var result = new FS2JournalItemsCollection();
			result.FireJournalItems = GetJournalItems(device, 0, 1, stageCount);
			if (hasGuardJournal)
			{
				result.SecurityJournalItems = GetJournalItems(device, 2, 2, stageCount);
			}
			return result;
		}

		static List<FS2JournalItem> GetJournalItems(Device device, int journalType, int currentStage, int stageCount)
		{
			var result = new List<FS2JournalItem>();

			CallbackManager.AddProgress(new FS2ProgressInfo("Чтение индекса последней записи", 50, currentStage, stageCount));
			var response = USBManager.Send(device, "Чтение индекса последней записи", 0x01, 0x21, journalType);
			if (response.HasError)
				return null;
			int lastIndex = BytesHelper.ExtractInt(response.Bytes, 0);

			CallbackManager.AddProgress(new FS2ProgressInfo("Чтение индекса первой записи", 50, currentStage, stageCount));
			response = USBManager.Send(device, "Чтение индекса первой записи", 0x01, 0x24, journalType);
			if (response.HasError)
				return null;
			var count = BytesHelper.ExtractShort(response.Bytes, 0);

			int firstIndex = Math.Max(lastIndex - count + 1, 0);

			for (int i = firstIndex; i <= lastIndex; i++)
			{
				CallbackManager.AddProgress(new FS2ProgressInfo("Чтение записей журнала " + (i - firstIndex).ToString() + " из " + (lastIndex - firstIndex + 1).ToString(),
					(i - firstIndex) * 100 / (lastIndex - firstIndex), currentStage, stageCount));
				response = USBManager.Send(device, "Чтение конкретной записи журнала", 0x01, 0x20, journalType, BitConverter.GetBytes(i).Reverse());
				if (!response.HasError)
				{
					try
					{
						var journalParser = new JournalParser();
						var journalItem = journalParser.Parce(null, device, response.Bytes, journalType);
						if (journalItem != null)
						{
							result.Add(journalItem);
						}
					}
					catch (Exception e)
					{
						Logger.Error(e, "ReadJournalOperationHelper.Parse");
					}
				}
			}

			result.Reverse();
			for (int i = 0; i < result.Count; i++)
			{
				result[i].No = i + 1;
			}
			return result;
		}
	}
}