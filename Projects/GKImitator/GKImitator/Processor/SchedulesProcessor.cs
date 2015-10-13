using GKProcessor;
using RubezhDAL.DataClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GKImitator.Processor
{
	public static class SchedulesProcessor
	{
		public static void EditShedule(List<byte> bytes)
		{
			var imitatorSchedule = new ImitatorSchedule();
			imitatorSchedule.No = bytes[0];
			imitatorSchedule.Name = BytesHelper.BytesToStringDescription(bytes, 1);
			imitatorSchedule.HolidayScheduleNo = bytes[33];
			imitatorSchedule.PartsCount = BytesHelper.SubstructShort(bytes, 34);
			imitatorSchedule.TotalSeconds = BytesHelper.SubstructInt(bytes, 36);
			imitatorSchedule.WorkHolidayScheduleNo = bytes[40];
			for (int i = 0; i < imitatorSchedule.PartsCount / 2; i++)
			{
				var imitatorSheduleInterval = new ImitatorSheduleInterval();
				imitatorSheduleInterval.StartSeconds = BytesHelper.SubstructInt(bytes, 48 + i * 8);
				imitatorSheduleInterval.EndSeconds = BytesHelper.SubstructInt(bytes, 52 + i * 8);
				imitatorSchedule.ImitatorSheduleIntervals.Add(imitatorSheduleInterval);
			}

			using (var dbService = new DbService())
			{
				dbService.ImitatorScheduleTranslator.AddOrEdit(imitatorSchedule);
			}
		}
	}
}