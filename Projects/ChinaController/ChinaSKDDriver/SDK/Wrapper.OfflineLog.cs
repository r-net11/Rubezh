using System.Threading.Tasks;
using StrazhDeviceSDK.API;
using System;
using System.Collections.Generic;

namespace StrazhDeviceSDK
{
	public partial class Wrapper
	{
		public IEnumerable<SKDJournalItem> GetOfflineLogItems(DateTime dateTime)
		{
			var readLogs = new Task<List<SKDJournalItem>[]>(() =>
			{
				var subTaskResults = new List<SKDJournalItem>[2];

				// Читаем оффлайн лог проходов
				new Task(() =>
				{
					var journalItems = new List<SKDJournalItem>();
					GetAccessLogItemsOlderThan(dateTime).ForEach(item => journalItems.Add(AccessLogItemToJournalItem(item)));
					subTaskResults[0] = journalItems;
				}, TaskCreationOptions.AttachedToParent).Start();

				// Читаем оффлайн лог тревог
				new Task(() =>
				{
					var journalItems = new List<SKDJournalItem>();
					GetAlarmLogItemsOlderThan(dateTime).ForEach(item => journalItems.Add(AlarmLogItemToJournalItem(item)));
					subTaskResults[1] = journalItems;
				}, TaskCreationOptions.AttachedToParent).Start();

				return subTaskResults;
			});
			
			readLogs.Start();

			var readLogsResult = readLogs.Result;
			var result = new List<SKDJournalItem>();
			readLogsResult[0].ForEach(result.Add);
			readLogsResult[1].ForEach(result.Add);
			result.Sort(new SKDJournalItemComparer());
			return result;
		}
	}
	public class SKDJournalItemComparer : IComparer<SKDJournalItem>
	{
		public int Compare(SKDJournalItem x, SKDJournalItem y)
		{
			if (!x.DeviceDateTime.HasValue && !y.DeviceDateTime.HasValue)
				return 0;
			if (!x.DeviceDateTime.HasValue)
				return -1;
			if (!y.DeviceDateTime.HasValue)
				return 1;
			return x.DeviceDateTime.Value.CompareTo(y.DeviceDateTime.Value);
		}
	}
}