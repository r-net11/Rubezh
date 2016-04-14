using System.Threading.Tasks;
using ChinaSKDDriverAPI;
using System;
using System.Collections.Generic;
using Common;

namespace ChinaSKDDriver
{
	public partial class Wrapper
	{
		public IEnumerable<SKDJournalItem> GetOfflineLogItems(DateTime dateTime)
		{
			var readLogs = new Task<List<SKDJournalItem>[]>(() =>
			{
				Logger.Info(string.Format("Контроллер '{0}'. Запущена задача чтения оффлайн-логов (после {1})", _deviceUid, dateTime));
				var subTaskResults = new List<SKDJournalItem>[2];

				// Читаем оффлайн лог проходов
				new Task(() =>
				{
					Logger.Info(string.Format("Контроллер '{0}'. Запущена задача чтения оффлайн-лога проходов (после {1})", _deviceUid, dateTime));
					var journalItems = new List<SKDJournalItem>();
					GetAccessLogItemsOlderThan(dateTime).ForEach(item => journalItems.Add(AccessLogItemToJournalItem(item)));
					Logger.Info(string.Format("Контроллер '{0}'. Прочитано {1} шт. оффлайн-проходов (после {2})", _deviceUid, journalItems.Count, dateTime));
					subTaskResults[0] = journalItems;
					Logger.Info(string.Format("Контроллер '{0}'. Завершена задача чтения оффлайн-лога проходов (после {1})", _deviceUid, dateTime));
				}, TaskCreationOptions.AttachedToParent).Start();

				// Читаем оффлайн лог тревог
				new Task(() =>
				{
					Logger.Info(string.Format("Контроллер '{0}'. Запущена задача чтения оффлайн-лога тревог (после {1})", _deviceUid, dateTime));
					var journalItems = new List<SKDJournalItem>();
					GetAlarmLogItemsOlderThan(dateTime).ForEach(item => journalItems.Add(AlarmLogItemToJournalItem(item)));
					Logger.Info(string.Format("Контроллер '{0}'. Прочитано {1} шт. оффлайн-тревог (после {2})", _deviceUid, journalItems.Count, dateTime));
					subTaskResults[1] = journalItems;
					Logger.Info(string.Format("Контроллер '{0}'. Завершена задача чтения оффлайн-лога тревог (после {1})", _deviceUid, dateTime));
				}, TaskCreationOptions.AttachedToParent).Start();

				Logger.Info(string.Format("Контроллер '{0}'. Завершена задача чтения оффлайн-логов (после {1})", _deviceUid, dateTime));
				return subTaskResults;
			});

			Logger.Info(string.Format("Контроллер '{0}'. Запускаем задачу чтения оффлайн-логов (после {1}) и ожидаем получение результата...", _deviceUid, dateTime));
			readLogs.Start();

			var readLogsResult = readLogs.Result;
			Logger.Info(string.Format("Контроллер '{0}'. Результат чтения оффлайн-логов (после {1}) получен", _deviceUid, dateTime));

			var result = new List<SKDJournalItem>();
			readLogsResult[0].ForEach(result.Add);
			readLogsResult[1].ForEach(result.Add);

			if (result.Count > 1)
			{
				Logger.Info(string.Format("Контроллер '{0}'. Сортируем оффлайн-события по дате регистарции на контроллере (после {1})", _deviceUid, dateTime));
				result.Sort(new SKDJournalItemComparer());
			}

			Logger.Info(string.Format("Контроллер '{0}'. Прочитанные оффлайн-события (после {1}) готовы к дальнейшей обработке...", _deviceUid, dateTime));
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