using StrazhDeviceSDK.API;
using StrazhDeviceSDK.NativeAPI;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace StrazhDeviceSDK
{
	public partial class Wrapper
	{
		public List<SKDJournalItem> GetOfflineSKDJournalItems(int lastReceivedRecordNumber)
		{
			var result = new List<SKDJournalItem>();
			var actualRecordNumber = GetAccessLogItemsCount();
			for (int i = lastReceivedRecordNumber + 1; i <= actualRecordNumber; i++)
			{
				NativeWrapper.NET_RECORDSET_ACCESS_CTL_CARDREC nativeAccessInfo;
				if (WRAP_Get_Native_Access_Info(i, out nativeAccessInfo))
					result.Add(NativeAccessToSKDJournalItem(nativeAccessInfo));
			}
			return result;
		}

		public List<SKDJournalItem> GetOfflineAlarmSKDJournalItems(int lastReceivedRecordNumber)
		{
			var result = new List<SKDJournalItem>();
			var alarmLogItemsCount = GetAlarmLogItemsCount();
			var alarmLogItems = GetFirstNativeAlarmLogItemsFromQuery(alarmLogItemsCount - lastReceivedRecordNumber);
			for (int i = 0; i <= alarmLogItems.Count - 1; i++)
			{
				var skdJournalItem = NativeLogToSKDJournalItem(alarmLogItems[i]);
				result.Add(skdJournalItem);
			}
			return result;
		}

		public bool WRAP_Get_Native_Access_Info(int recordNo, out NativeWrapper.NET_RECORDSET_ACCESS_CTL_CARDREC nativeAccessInfo)
		{
			var structSize = Marshal.SizeOf(typeof(NativeWrapper.NET_RECORDSET_ACCESS_CTL_CARDREC));
			var intPtr = Marshal.AllocCoTaskMem(structSize);

			var result = NativeWrapper.WRAP_Get_Access_Info(LoginID, recordNo, intPtr);
			if (result)
			{
				nativeAccessInfo = (NativeWrapper.NET_RECORDSET_ACCESS_CTL_CARDREC)(Marshal.PtrToStructure(intPtr, typeof(NativeWrapper.NET_RECORDSET_ACCESS_CTL_CARDREC)));
				Marshal.FreeCoTaskMem(intPtr);
				intPtr = IntPtr.Zero;
			}
			else
			{
				nativeAccessInfo = default(NativeWrapper.NET_RECORDSET_ACCESS_CTL_CARDREC);
			}
			return result;
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
}