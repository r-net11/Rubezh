using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ChinaSKDDriverAPI;
using ChinaSKDDriverNativeApi;

namespace ChinaSKDDriver
{
	public partial class Wrapper
	{
		public int AddHoliday(Holiday holiday)
		{
			NativeWrapper.NET_RECORDSET_HOLIDAY stuHoliday = new NativeWrapper.NET_RECORDSET_HOLIDAY();
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
			var result = NativeWrapper.WRAP_Insert_Holiday(LoginID, ref stuHoliday);
			return result;
		}

		public bool RemoveHoliday(int index)
		{
			var result = NativeWrapper.WRAP_RemoveHoliday(LoginID, index);
			return result;
		}

		public bool RemoveAllHolidays()
		{
			var result = NativeWrapper.WRAP_RemoveAllHolidays(LoginID);
			return result;
		}

		public Holiday GetHolidayInfo(int recordNo)
		{
			int structSize = Marshal.SizeOf(typeof(NativeWrapper.NET_RECORDSET_HOLIDAY));
			IntPtr intPtr = Marshal.AllocCoTaskMem(structSize);

			var result = NativeWrapper.WRAP_GetHolidayInfo(LoginID, recordNo, intPtr);

			NativeWrapper.NET_RECORDSET_HOLIDAY sdkHoliday = (NativeWrapper.NET_RECORDSET_HOLIDAY)(Marshal.PtrToStructure(intPtr, typeof(NativeWrapper.NET_RECORDSET_HOLIDAY)));
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

		public int GetHolidaysCount()
		{
			var holidaysCount = NativeWrapper.WRAP_Get_HolidaysCount(LoginID);
			return holidaysCount;
		}

		public List<Holiday> GetAllHolidays()
		{
			return new List<Holiday>();
		}
	}
}