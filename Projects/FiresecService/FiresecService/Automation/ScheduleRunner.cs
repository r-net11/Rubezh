using StrazhAPI.Automation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace FiresecService
{
	public static class ScheduleRunner
	{
		private static int timeValidator;
		private static DateTime startTime;

		private static int TimeDelta
		{
			get { return (int)((DateTime.Now - startTime).TotalSeconds); }
		}

		private static Thread Thread;
		private static AutoResetEvent AutoResetEvent = new AutoResetEvent(false);

		public static void Start()
		{
			timeValidator = -1;
			startTime = DateTime.Now;
			Thread = new Thread(OnRun);
			Thread.Start();
		}

		public static void Stop()
		{
			if (AutoResetEvent != null)
			{
				AutoResetEvent.Set();
				if (Thread != null)
				{
					Thread.Join(TimeSpan.FromSeconds(2));
				}
			}
		}

		public static void SetNewConfig()
		{
			Stop();
			Start();
		}

		private static void OnRun()
		{
			AutoResetEvent = new AutoResetEvent(false);
			while (true)
			{
				//Trace.WriteLine("timeValidator " + timeValidator + " TimeDelta " + TimeDelta);
				if (AutoResetEvent.WaitOne(TimeSpan.FromSeconds(1)))
				{
					return;
				}

				timeValidator++;
				foreach (var schedule in ConfigurationCashHelper.SystemConfiguration.AutomationConfiguration.AutomationSchedules)
				{
					if (timeValidator <= TimeDelta)
					{
						var dateList = new List<DateTime>();
						for (int i = 0; i < TimeDelta - timeValidator; i++)
						{
							dateList.Add(DateTime.Now - TimeSpan.FromSeconds(i));
						}
						dateList.Reverse();
						timeValidator = TimeDelta - 1;
						foreach (var date in dateList)
						{
							if (CheckSchedule(schedule, date))
							{
								Trace.WriteLine(DateTime.Now);
								RunProcedures(schedule);
							}
						}
					}
				}
			}
		}

		private static bool CheckSchedule(AutomationSchedule schedule, DateTime dateTime)
		{
			if ((schedule.DayOfWeek.ToString() != dateTime.DayOfWeek.ToString()) && (schedule.DayOfWeek != DayOfWeekType.Any))
				return false;

			if (!schedule.IsPeriodSelected)
				return (((schedule.Year == dateTime.Year) || (schedule.Year == -1)) &&
						((schedule.Month == dateTime.Month) || (schedule.Month == -1)) &&
						((schedule.Day == dateTime.Day) || (schedule.Day == -1)) &&
						((schedule.Hour == dateTime.Hour) || (schedule.Hour == -1)) &&
						((schedule.Minute == dateTime.Minute) || (schedule.Minute == -1)) &&
						((schedule.Second == dateTime.Second) || (schedule.Second == -1)));
			var scheduleDateTime = new DateTime(schedule.Year, schedule.Month, schedule.Day, schedule.Hour, schedule.Minute, schedule.Second);
			var delta = (int)((dateTime - scheduleDateTime).TotalSeconds);
			if (delta < 0)
				return false;
			var period = schedule.PeriodDay * 24 * 3600 + schedule.PeriodHour * 3600 + schedule.PeriodMinute * 60 + schedule.PeriodSecond;
			return (delta % period == 0);
		}

		private static void RunProcedures(AutomationSchedule schedule)
		{
			if (schedule.IsActive)
				foreach (var scheduleProcedure in schedule.ScheduleProcedures)
				{
					var procedure = ConfigurationCashHelper.SystemConfiguration.AutomationConfiguration.Procedures.FirstOrDefault(x => x.Uid == scheduleProcedure.ProcedureUid);
					if (procedure != null && procedure.IsActive)
						ProcedureRunner.Run(procedure, scheduleProcedure.Arguments, null);
				}
		}
	}
}