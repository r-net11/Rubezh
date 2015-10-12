using System.Linq;
using ChinaSKDDriverAPI;
using ChinaSKDDriverNativeApi;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using FiresecAPI.Journal;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace ChinaSKDDriver
{
	public partial class Wrapper
	{
		public int GetAlarmLogItemsCount()
		{
			var logParam = new NativeWrapper.QUERY_DEVICE_LOG_PARAM();
			var result = NativeWrapper.WRAP_GetLogCount(LoginID, ref logParam);
			return result;
		}

		public List<AlarmLogItem> GetAllAlarmLogItems()
		{
			var logs = new List<AlarmLogItem>();

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
					var logItem = NativeLogToAlarmLogItem(nativeLogs.Logs[i]);
					logs.Add(logItem);
				}
			}

			NativeWrapper.WRAP_QueryStop();
			return logs;
		}

		private AlarmLogItem NativeLogToAlarmLogItem(NativeWrapper.WRAP_NET_LOG_INFO nativeLogItem)
		{
			var logItem = new AlarmLogItem();
			logItem.DateTime = Wrapper.NET_TIMEToDateTime(nativeLogItem.stuTime);
			logItem.UserName = nativeLogItem.szUserName;
			logItem.LogType = nativeLogItem.szLogType;
			logItem.LogMessage = nativeLogItem.szLogMessage;

			var alarmLogMessage = JsonConvert.DeserializeObject<AlarmLogMessage>(logItem.LogMessage);
			logItem.AlarmType = alarmLogMessage.AlarmType;
			logItem.DoorNo = alarmLogMessage.Channel;
			logItem.CardId = alarmLogMessage.CardId;
			logItem.LogNo = Convert.ToInt32(alarmLogMessage.LogNo);

			return logItem;
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