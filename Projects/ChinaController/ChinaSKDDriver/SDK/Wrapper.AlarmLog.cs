using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using StrazhAPI.Journal;
using StrazhDeviceSDK.API;
using StrazhDeviceSDK.NativeAPI;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace StrazhDeviceSDK
{
	public partial class Wrapper
	{
		/// <summary>
		/// Получает количество тревожных событий в оффлайн журнале
		/// </summary>
		/// <returns>Количество тревожных событий в оффлайн журнале</returns>
		public int GetAlarmLogItemsCount()
		{
			var logParam = new NativeWrapper.QUERY_DEVICE_LOG_PARAM();
			var result = NativeWrapper.WRAP_GetLogCount(LoginID, ref logParam);
			return result;
		}

		/// <summary>
		/// Получает все тревожные события из оффлайн журнала
		/// </summary>
		/// <returns>Список тревожных событий</returns>
		public List<NativeWrapper.WRAP_NET_LOG_INFO> GetFirstNativeAlarmLogItemsFromQuery(int logItemsCount)
		{
			var logs = new List<NativeWrapper.WRAP_NET_LOG_INFO>();
			var isBreak = false;

			NativeWrapper.WRAP_QueryStart(LoginID);

			while (true)
			{
				var structSize = Marshal.SizeOf(typeof(NativeWrapper.WRAP_Dev_QueryLogList_Result));
				var intPtr = Marshal.AllocCoTaskMem(structSize);

				var result = NativeWrapper.WRAP_QueryNext(intPtr);
				if (result == 0)
					break;

				var nativeLogs = (NativeWrapper.WRAP_Dev_QueryLogList_Result)(Marshal.PtrToStructure(intPtr, typeof(NativeWrapper.WRAP_Dev_QueryLogList_Result)));
				Marshal.FreeCoTaskMem(intPtr);
				intPtr = IntPtr.Zero;
				for (var i = 0; i < result; i++)
				{
					isBreak = logs.Count >= logItemsCount;
					if (isBreak)
						break;
					logs.Add(nativeLogs.Logs[i]);
				}
				if (isBreak)
					break;
			}
			NativeWrapper.WRAP_QueryStop();
			return logs;
		}

		/// <summary>
		/// Получает все тревожные события из оффлайн журнала, возраст которых старше указанной даты
		/// </summary>
		/// <param name="dateTime">Дата</param>
		/// <returns>Список тревожных событий</returns>
		public List<AlarmLogItem> GetAlarmLogItemsOlderThan(DateTime dateTime)
		{
			var resultAlarms = new List<AlarmLogItem>();

			NativeWrapper.WRAP_QueryStart(LoginID);

			var continueSearch = true;
			while (continueSearch)
			{
				var structSize = Marshal.SizeOf(typeof(NativeWrapper.WRAP_Dev_QueryLogList_Result));
				var intPtr = Marshal.AllocCoTaskMem(structSize);

				var result = NativeWrapper.WRAP_QueryNext(intPtr);
				if (result == 0)
					break;

				var nativeLogs = (NativeWrapper.WRAP_Dev_QueryLogList_Result)(Marshal.PtrToStructure(intPtr, typeof(NativeWrapper.WRAP_Dev_QueryLogList_Result)));
				Marshal.FreeCoTaskMem(intPtr);
				intPtr = IntPtr.Zero;

				for (var i = 0; i < result; i++)
				{
					var logItem = NativeLogToAlarmLogItem(nativeLogs.Logs[i]);
					if (logItem.DateTime > dateTime)
						resultAlarms.Add(logItem);
					else
					{
						continueSearch = false;
						break;
					}
				}
			}

			NativeWrapper.WRAP_QueryStop();
			return resultAlarms;
		}

		private static AlarmLogItem NativeLogToAlarmLogItem(NativeWrapper.WRAP_NET_LOG_INFO nativeLogItem)
		{
			var logItem = new AlarmLogItem();
			logItem.DateTime = Wrapper.NET_TIMEToDateTime(nativeLogItem.stuTime);
			logItem.UserName = nativeLogItem.szUserName;
			logItem.LogType = nativeLogItem.szLogType;
			logItem.LogMessage = nativeLogItem.szLogMessage;

			var alarmLogMessage = JsonConvert.DeserializeObject<AlarmLogMessage>(logItem.LogMessage);
			logItem.AlarmType = alarmLogMessage.AlarmType;
			logItem.Channel = alarmLogMessage.Channel;
			logItem.CardId = alarmLogMessage.CardId;
			logItem.LogNo = Convert.ToInt32(alarmLogMessage.LogNo);

			return logItem;
		}

		public SKDJournalItem AlarmLogItemToJournalItem(AlarmLogItem alarmLogItem)
		{
			var journalItem = new SKDJournalItem();

			journalItem.JournalItemType = JournalItemType.Offline;
			journalItem.LoginID = LoginID;
			journalItem.SystemDateTime = DateTime.Now;
			journalItem.DeviceDateTime = alarmLogItem.DateTime;
			switch (alarmLogItem.AlarmType)
			{
				case AlarmType.BreakIn:
					journalItem.JournalEventNameType = JournalEventNameType.Взлом;
					journalItem.DoorNo = alarmLogItem.Channel;
					break;
				case AlarmType.Duress:
					journalItem.JournalEventNameType = JournalEventNameType.Принуждение;
					journalItem.DoorNo = alarmLogItem.Channel;
					break;
				case AlarmType.AlarmLocal:
					journalItem.JournalEventNameType = JournalEventNameType.Местная_тревога_начало;
					journalItem.DoorNo = alarmLogItem.Channel;
					break;
				case AlarmType.DoorNotClose:
					{
						journalItem.JournalEventNameType = JournalEventNameType.Дверь_не_закрыта_начало;
						journalItem.DoorNo = alarmLogItem.Channel;
						break;
					}
				case AlarmType.ReaderChassisIntruded:
					journalItem.JournalEventNameType = JournalEventNameType.Вскрытие_контроллера_начало;
					journalItem.szReaderID = (alarmLogItem.Channel + 1).ToString();
					break;
				case AlarmType.Repeatenter:
					journalItem.JournalEventNameType = JournalEventNameType.Повторный_проход;
					journalItem.DoorNo = alarmLogItem.Channel;
					break;
			}
			journalItem.CardNo = alarmLogItem.CardId;

			return journalItem;
		}

		public SKDJournalItem NativeLogToSKDJournalItem(NativeWrapper.WRAP_NET_LOG_INFO nativeLog)
		{
			var journalItem = new SKDJournalItem();

			journalItem.JournalItemType = JournalItemType.Offline;
			journalItem.LoginID = LoginID;
			journalItem.SystemDateTime = DateTime.Now;
			journalItem.DeviceDateTime = NET_TIMEToDateTime(nativeLog.stuTime);
			var alarmLogMessage = JsonConvert.DeserializeObject<AlarmLogMessage>(nativeLog.szLogMessage);
			switch (alarmLogMessage.AlarmType)
			{
				case AlarmType.BreakIn:
					journalItem.JournalEventNameType = JournalEventNameType.Взлом;
					break;
				case AlarmType.Duress:
					journalItem.JournalEventNameType = JournalEventNameType.Принуждение;
					break;
				case AlarmType.AlarmLocal:
					journalItem.JournalEventNameType = JournalEventNameType.Местная_тревога_начало;
					break;
				case AlarmType.DoorNotClose:
					{
						journalItem.JournalEventNameType = JournalEventNameType.Дверь_не_закрыта_начало;
						break;
					}
				case AlarmType.ReaderChassisIntruded:
					journalItem.JournalEventNameType = JournalEventNameType.Вскрытие_контроллера_начало;
					journalItem.szReaderID = (alarmLogMessage.Channel + 1).ToString();
					break;
				case AlarmType.Repeatenter:
					journalItem.JournalEventNameType = JournalEventNameType.Повторный_проход;
					break;
			}
			journalItem.DoorNo = alarmLogMessage.Channel;
			journalItem.CardNo = alarmLogMessage.CardId;

			return journalItem;
		}
	}

	public class AlarmLogMessage
	{
		public int LogNo { get; set; }
		[JsonConverter(typeof(StringEnumConverter))]
		public AlarmType AlarmType { get; set; }
		public int Channel { get; set; }
		public string CardId { get; set; }
	}

	public enum AlarmType
	{
		BreakIn = 0,
		Duress = 1,
		AlarmLocal = 2,
		DoorNotClose = 3,
		ReaderChassisIntruded = 4,
		Repeatenter = 5
	}
}