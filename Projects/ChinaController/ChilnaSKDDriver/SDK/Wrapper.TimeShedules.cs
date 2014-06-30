using System.Collections.Generic;
using System.Linq;
using ChinaSKDDriverAPI;
using ChinaSKDDriverNativeApi;

namespace ChinaSKDDriver
{
	public partial class Wrapper
	{
		public List<TimeShedule> GetTimeShedules(int index)
		{
			NativeWrapper.CFG_ACCESS_TIMESCHEDULE_INFO timeSheduleInfo;
			var result = NativeWrapper.WRAP_GetTimeSchedule(LoginID, index, out timeSheduleInfo);

			var timeSheduleIntervals = new List<TimeSheduleInterval>();

			for (int i = 0; i < timeSheduleInfo.stuTime.Count(); i++)
			{
				var cfg_TIME_SECTION = timeSheduleInfo.stuTime[i];
				var timeSheduleInterval = new TimeSheduleInterval();
				timeSheduleInterval.BeginHours = cfg_TIME_SECTION.nBeginHour;
				timeSheduleInterval.BeginMinutes = cfg_TIME_SECTION.nBeginMin;
				timeSheduleInterval.BeginSeconds = cfg_TIME_SECTION.nBeginSec;
				timeSheduleInterval.EndHours = cfg_TIME_SECTION.nEndHour;
				timeSheduleInterval.EndMinutes = cfg_TIME_SECTION.nEndMin;
				timeSheduleInterval.EndSeconds = cfg_TIME_SECTION.nEndSec;
				timeSheduleIntervals.Add(timeSheduleInterval);
			}

			var timeShedules = new List<TimeShedule>();
			for (int i = 0; i < 7; i++)
			{
				var timeShedule = new TimeShedule();
				for (int j = 0; j < 4; j++)
				{
					var timeSheduleInterval = timeSheduleIntervals[i * 4 + j];
					timeShedule.TimeSheduleIntervals.Add(timeSheduleInterval);
				}
				timeShedules.Add(timeShedule);
			}

			return timeShedules;
		}

		public bool SetTimeShedules(int index, List<TimeShedule> timeShedules)
		{
			NativeWrapper.CFG_ACCESS_TIMESCHEDULE_INFO timeSheduleInfos = new NativeWrapper.CFG_ACCESS_TIMESCHEDULE_INFO();
			timeSheduleInfos.stuTime = new NativeWrapper.CFG_TIME_SECTION[7 * 4];
			timeSheduleInfos.bEnable = true;
			for (int i = 0; i < timeShedules.Count; i++)
			{
				var timeShedule = timeShedules[i];
				for (int j = 0; j < timeShedule.TimeSheduleIntervals.Count; j++)
				{
					var timeSheduleInterval = timeShedule.TimeSheduleIntervals[j];
					var nativeIndex = i * 4 + j;

					timeSheduleInfos.stuTime[nativeIndex].nBeginHour = timeSheduleInterval.BeginHours;
					timeSheduleInfos.stuTime[nativeIndex].nBeginMin = timeSheduleInterval.BeginMinutes;
					timeSheduleInfos.stuTime[nativeIndex].nBeginSec = timeSheduleInterval.BeginSeconds;
					timeSheduleInfos.stuTime[nativeIndex].nEndHour = timeSheduleInterval.EndHours;
					timeSheduleInfos.stuTime[nativeIndex].nEndMin = timeSheduleInterval.EndMinutes;
					timeSheduleInfos.stuTime[nativeIndex].nEndSec = timeSheduleInterval.EndSeconds;
				}
			}

			var result = NativeWrapper.WRAP_SetTimeSchedule(LoginID, index, ref timeSheduleInfos);
			return result;
		}
	}
}