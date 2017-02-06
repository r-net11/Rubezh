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
				var nativeAccessInfo = WRAP_Get_Native_Access_Info(i);
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
				skdJournalItem.No = lastReceivedRecordNumber + 1 + i;
				result.Add(skdJournalItem);
			}
			return result;
		}

		public NativeWrapper.NET_RECORDSET_ACCESS_CTL_CARDREC WRAP_Get_Native_Access_Info(int recordNo)
		{
			var structSize = Marshal.SizeOf(typeof(NativeWrapper.NET_RECORDSET_ACCESS_CTL_CARDREC));
			var intPtr = Marshal.AllocCoTaskMem(structSize);

			var result = NativeWrapper.WRAP_Get_Access_Info(LoginID, recordNo, intPtr);
			if (!result)
				return default(NativeWrapper.NET_RECORDSET_ACCESS_CTL_CARDREC);

			var nativeAccess = (NativeWrapper.NET_RECORDSET_ACCESS_CTL_CARDREC)(Marshal.PtrToStructure(intPtr, typeof(NativeWrapper.NET_RECORDSET_ACCESS_CTL_CARDREC)));
			Marshal.FreeCoTaskMem(intPtr);
			intPtr = IntPtr.Zero;
			return nativeAccess;
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