using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ChinaSKDDriverAPI;
using ChinaSKDDriverNativeApi;

namespace ChinaSKDDriver
{
	public static partial class Wrapper
	{
		public static int AddHoliday(int loginID, Holiday holiday)
		{
			SDKImport.NET_RECORDSET_HOLIDAY stuHoliday = new SDKImport.NET_RECORDSET_HOLIDAY();
			stuHoliday.nDoorNum = holiday.DoorsCount;
			stuHoliday.sznDoors = new int[32];
			stuHoliday.sznDoors[0] = 1;
			stuHoliday.sznDoors[1] = 2;

			stuHoliday.stuStartTime.dwYear = holiday.StartDateTime.Year;
			stuHoliday.stuStartTime.dwMonth = holiday.StartDateTime.Month;
			stuHoliday.stuStartTime.dwDay = holiday.StartDateTime.Day;
			stuHoliday.stuStartTime.dwHour = holiday.StartDateTime.Hour;
			stuHoliday.stuStartTime.dwMinute = holiday.StartDateTime.Minute;
			stuHoliday.stuStartTime.dwSecond = holiday.StartDateTime.Second;

			stuHoliday.stuEndTime.dwYear = holiday.EndDateTime.Year;
			stuHoliday.stuEndTime.dwMonth = holiday.EndDateTime.Month;
			stuHoliday.stuEndTime.dwDay = holiday.EndDateTime.Day;
			stuHoliday.stuEndTime.dwHour = holiday.EndDateTime.Hour;
			stuHoliday.stuEndTime.dwMinute = holiday.EndDateTime.Minute;
			stuHoliday.stuEndTime.dwSecond = holiday.EndDateTime.Second;
			stuHoliday.bEnable = holiday.IsEnabled;
			var result = SDKImport.WRAP_Insert_Holiday(loginID, ref stuHoliday);
			return result;
		}

		public static Holiday GetHolidayInfo(int loginID, int recordNo)
		{
			int structSize = Marshal.SizeOf(typeof(SDKImport.NET_RECORDSET_HOLIDAY));
			IntPtr intPtr = Marshal.AllocCoTaskMem(structSize);

			var result = SDKImport.WRAP_GetHolidayInfo(loginID, recordNo, intPtr);

			SDKImport.NET_RECORDSET_HOLIDAY sdkHoliday = (SDKImport.NET_RECORDSET_HOLIDAY)(Marshal.PtrToStructure(intPtr, typeof(SDKImport.NET_RECORDSET_HOLIDAY)));
			Marshal.FreeCoTaskMem(intPtr);
			intPtr = IntPtr.Zero;

			var holiday = new Holiday();
			holiday.RecordNo = sdkHoliday.nRecNo;
			holiday.DoorsCount = sdkHoliday.nDoorNum;
			holiday.StartDateTime = NET_TIMEToDateTime(sdkHoliday.stuStartTime);
			holiday.EndDateTime = NET_TIMEToDateTime(sdkHoliday.stuEndTime);
			holiday.IsEnabled = sdkHoliday.bEnable;

			return holiday;
		}

		public static List<Holiday> GetAllHolidays(int loginID)
		{
			return new List<Holiday>();
		}
	}
}