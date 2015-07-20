using GKProcessor;
using SKDDriver.DataClasses;
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
			imitatorSchedule.No = bytes[1];
			imitatorSchedule.Name = BytesHelper.BytesToStringDescription(bytes, 2);
			imitatorSchedule.HolidayScheduleNo = bytes[34];
			imitatorSchedule.PartsCount = BytesHelper.SubstructShort(bytes, 36);
			imitatorSchedule.TotalSeconds = BytesHelper.SubstructInt(bytes, 38);
			imitatorSchedule.WorkHolidayScheduleNo = bytes[42];
			for (int i = 0; i < 100; i++)
			{
				var imitatorSheduleInterval = new ImitatorSheduleInterval();
				imitatorSheduleInterval.StartSeconds = BytesHelper.SubstructInt(bytes, 49 + i * 4);
				imitatorSheduleInterval.EndSeconds = BytesHelper.SubstructInt(bytes, 53 + i * 4);
				if (imitatorSheduleInterval.StartSeconds != 0)
				{
					imitatorSchedule.ImitatorSheduleIntervals.Add(imitatorSheduleInterval);
				}
				else
				{
					break;
				}
			}

			using (var dbService = new DbService())
			{
				dbService.ImitatorScheduleTranslator.AddOrEdit(imitatorSchedule);
			}
		}
	}
}