using System;
using System.Runtime.InteropServices;
using ChinaSKDDriverAPI;
using ChinaSKDDriverNativeApi;
using FiresecAPI.SKD;
using System.Collections.Generic;

namespace ChinaSKDDriver
{
	public partial class Wrapper
	{
		public List<LogItem> GetAllLogs()
		{
			var logs = new List<LogItem>();

			NativeWrapper.WRAP_QueryStart(LoginID);

			while (true)
			{
				int structSize = Marshal.SizeOf(typeof(NativeWrapper.WRAP_Dev_QueryLogList_Result));
				IntPtr intPtr = Marshal.AllocCoTaskMem(structSize);

				var result = NativeWrapper.WRAP_QueryNext(intPtr);
				if (result == 0)
					break;

				NativeWrapper.WRAP_Dev_QueryLogList_Result nativeLogs = (NativeWrapper.WRAP_Dev_QueryLogList_Result)(Marshal.PtrToStructure(intPtr, typeof(NativeWrapper.WRAP_Dev_QueryLogList_Result)));
				Marshal.FreeCoTaskMem(intPtr);
				intPtr = IntPtr.Zero;

				for (int i = 0; i < result; i++)
				{
					var logItem = NativeLogToLogItem(nativeLogs.Logs[i]);
					logs.Add(logItem);
				}
			}

			NativeWrapper.WRAP_QueryStop();
			return logs;
		}

		LogItem NativeLogToLogItem(ChinaSKDDriverNativeApi.NativeWrapper.WRAP_NET_LOG_INFO nativeLogItem)
		{
			var logItem = new LogItem();
			logItem.DateTime = Wrapper.NET_TIMEToDateTime(nativeLogItem.stuTime);
			logItem.UserName = nativeLogItem.szUserName;
			logItem.LogType = nativeLogItem.szLogType;
			logItem.LogMessage = nativeLogItem.szLogMessage;

			var index1 = nativeLogItem.szLogMessage.IndexOf("\"CardId\" : ");
			var index2 = nativeLogItem.szLogMessage.IndexOf(",");
			var value = nativeLogItem.szLogMessage.Substring(index1 + 11, index2 - index1 - 11);
			nativeLogItem.szLogMessage = nativeLogItem.szLogMessage.Remove(0, index2 + 2);
			logItem.CardId = value;

			index1 = nativeLogItem.szLogMessage.IndexOf("\"DoorNo\" : ");
			index2 = nativeLogItem.szLogMessage.IndexOf(",");
			value = nativeLogItem.szLogMessage.Substring(index1 + 11, index2 - index1 - 11);
			nativeLogItem.szLogMessage = nativeLogItem.szLogMessage.Remove(0, index2 + 2);
			logItem.DoorNo = value;

			index1 = nativeLogItem.szLogMessage.IndexOf("\"Type\" : ");
			index2 = nativeLogItem.szLogMessage.IndexOf("\n");
			value = nativeLogItem.szLogMessage.Substring(index1 + 9, index2 - index1 - 9);
			nativeLogItem.szLogMessage = nativeLogItem.szLogMessage.Remove(0, index2 + 2);
			logItem.Type = value;

			return logItem;
		}
	}
}