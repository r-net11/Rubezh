using System;
using System.Collections.Generic;
using System.Threading;
using FiresecAPI.Automation;
using FiresecAPI.Models;
using FiresecService.Processor;

namespace FiresecService
{
	public static class AutomationProcessor
	{
		public static SystemConfiguration SystemConfiguration { get; private set; }

		public static void Start()
		{
			SystemConfiguration = ZipConfigurationHelper.GetSystemConfiguration();
			timeValidator = 0;
			startTime = DateTime.Now;
			var thread = new Thread(OnRun);
			thread.Start();
		}

		static int timeValidator;
		static DateTime startTime;
		static int TimeDelta
		{
			get { return  (int)((DateTime.Now - startTime).TotalSeconds) % 100000; }
		}   

		static void OnRun()
		{
			while (true)
			{
				var shedules = SystemConfiguration.AutomationSchedules;
				timeValidator++;
				Thread.Sleep(TimeSpan.FromSeconds(1));
				foreach (var schedule in shedules)
				{
					if (CheckSchedule(schedule, DateTime.Now))
						RunProcedures(schedule);
					timeValidator = timeValidator % 100000;
					if (timeValidator < TimeDelta - 1)
					{
						var dateList = new List<DateTime>();
						for (int i = 0; i < TimeDelta - 1 - timeValidator; i++)
						{
							dateList.Add(DateTime.Now - TimeSpan.FromSeconds(i));
						}
						timeValidator = TimeDelta;
						foreach (var date in dateList)
						{
							if (CheckSchedule(schedule, date))
								RunProcedures(schedule);
						}
					}
				}
			}
		}

		static bool CheckSchedule(AutomationSchedule schedule, DateTime dateTime)
		{
			return (((schedule.Year == dateTime.Year) || (schedule.Year == -1)) &&
					((schedule.Month == dateTime.Month) || (schedule.Month == -1)) &&
					((schedule.Day == dateTime.Day) || (schedule.Day == -1)) &&
					((schedule.Hour == dateTime.Hour) || (schedule.Hour == -1)) &&
					((schedule.Minute == dateTime.Minute) || (schedule.Minute == -1)) &&
					((schedule.Second == dateTime.Second) || (schedule.Second == -1)) &&
					((schedule.DayOfWeek.ToString() == dateTime.DayOfWeek.ToString()) || (schedule.DayOfWeek == DayOfWeekType.Any)));
		}

		static void RunProcedures(AutomationSchedule schedule)
		{
			foreach (var procedure in SystemConfiguration.AutomationConfiguration.Procedures)
			{
				if (schedule.ProceduresUids.Contains(procedure.Uid))
					procedure.Start();
			}
		}
	}
}