using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using ChinaSKDDriverAPI;
using ChinaSKDDriverNativeApi;

namespace ChinaSKDDriver
{
	public static partial class Wrapper
	{
		public static List<TimeShedule> GetTimeShedules(int loginID)
		{
			int structSize = Marshal.SizeOf(typeof(SDKImport.CFG_ACCESS_TIMESCHEDULE_INFO));
			IntPtr intPtr = Marshal.AllocCoTaskMem(structSize);

			var result = SDKImport.WRAP_GetAccessTimeSchedule(loginID, intPtr);

			SDKImport.CFG_ACCESS_TIMESCHEDULE_INFO timeSheduleInfo = (SDKImport.CFG_ACCESS_TIMESCHEDULE_INFO)(Marshal.PtrToStructure(intPtr, typeof(SDKImport.CFG_ACCESS_TIMESCHEDULE_INFO)));
			Marshal.FreeCoTaskMem(intPtr);
			intPtr = IntPtr.Zero;

			var timeShedules = new List<TimeShedule>();

			for (int i = 0; i < timeSheduleInfo.stuTime.Count(); i++)
			{
				var cfg_TIME_SECTION = timeSheduleInfo.stuTime[i];
				var timeShedule = new TimeShedule();
				timeShedule.Mask = cfg_TIME_SECTION.dwRecordMask;
				timeShedule.BeginHours = cfg_TIME_SECTION.nBeginHour;
				timeShedule.BeginMinutes = cfg_TIME_SECTION.nBeginMin;
				timeShedule.BeginSeconds = cfg_TIME_SECTION.nBeginSec;
				timeShedule.EndHours = cfg_TIME_SECTION.nEndHour;
				timeShedule.EndMinutes = cfg_TIME_SECTION.nEndMin;
				timeShedule.EndSeconds = cfg_TIME_SECTION.nEndSec;
				timeShedules.Add(timeShedule);
			}
			return timeShedules;
		}

		public static bool SetTimeShedules(int loginID, List<TimeShedule> timeShedules)
		{
			SDKImport.CFG_ACCESS_TIMESCHEDULE_INFO timeSheduleInfos = new SDKImport.CFG_ACCESS_TIMESCHEDULE_INFO();
			timeSheduleInfos.stuTime = new SDKImport.CFG_TIME_SECTION[7 * 4];
			timeSheduleInfos.bEnable = true;
			for (int i = 0; i < timeShedules.Count; i++)
			{
				timeSheduleInfos.stuTime[i].nBeginHour = timeShedules[i].BeginHours;
				timeSheduleInfos.stuTime[i].nBeginMin = timeShedules[i].BeginMinutes;
				timeSheduleInfos.stuTime[i].nBeginSec = timeShedules[i].BeginSeconds;
				timeSheduleInfos.stuTime[i].nEndHour = timeShedules[i].EndHours;
				timeSheduleInfos.stuTime[i].nEndMin = timeShedules[i].EndMinutes;
				timeSheduleInfos.stuTime[i].nEndSec = timeShedules[i].EndSeconds;
			}

			var result = SDKImport.WRAP_SetAccessTimeSchedule(loginID, timeSheduleInfos);

			return result;
		}
	}
}