using FiresecAPI.SKD;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace FiresecMonitorUnitTests.FireMonitorTests
{
	[TestFixture]
	public class DayTimeTrackTests
	{
		#region GetTimeTrackType Tests

		private DateTime TIME = DateTime.Now;

		[Test]
		public void GetTimeTrackTypeNonOfficialOvertimeTimeTrackType()
		{
			//Arrange
			var dayTimeTrack = new DayTimeTrack();
			var plannedTimeInterval = new DayTimeTrack.ScheduleInterval(TIME.Date + TimeSpan.FromHours(12), TIME.Date + TimeSpan.FromHours(18));
			var realTimeTrackPart = new TimeTrackPart { EnterDateTime = TIME.Date + TimeSpan.FromHours(9), ExitDateTime = TIME.Date + TimeSpan.FromHours(10) };
			var plannedTimeTrackPart = new TimeTrackPart
			{
				EnterDateTime = plannedTimeInterval.StartTime,
				ExitDateTime = plannedTimeInterval.EndTime
			};

			var realTimeTrackPartsCollection = new List<TimeTrackPart> {realTimeTrackPart};
			var plannedTimeTrackPartCollection = new List<TimeTrackPart> {plannedTimeTrackPart};

			//Act
			TimeTrackType type = dayTimeTrack.GetTimeTrackType(realTimeTrackPart, plannedTimeTrackPartCollection, realTimeTrackPartsCollection, false, plannedTimeInterval,
				new DayTimeTrack.ScheduleInterval(realTimeTrackPart.EnterDateTime, realTimeTrackPart.ExitDateTime));

			//Assert
			Assert.IsTrue(type == TimeTrackType.Overtime);
		}

		[Test]
		public void GetTimeTrackTypePresentTimeTrackType()
		{
			//Arrange
			var dayTimeTrack = new DayTimeTrack();
			var plannedTimeInterval = new DayTimeTrack.ScheduleInterval(TIME.Date + TimeSpan.FromHours(12), TIME.Date + TimeSpan.FromHours(18));

			var realTimeTrackPart = new TimeTrackPart { EnterDateTime = TIME.Date + TimeSpan.FromHours(12), ExitDateTime = TIME.Date + TimeSpan.FromHours(13) };
			var plannedTimeTrackPart = new TimeTrackPart
			{
				EnterDateTime = plannedTimeInterval.StartTime,
				ExitDateTime = plannedTimeInterval.EndTime
			};

			var realTimeTrackPartsCollection = new List<TimeTrackPart> { realTimeTrackPart };
			var plannedTimeTrackPartCollection = new List<TimeTrackPart> { plannedTimeTrackPart };

			//Act
			TimeTrackType type = dayTimeTrack.GetTimeTrackType(realTimeTrackPart, plannedTimeTrackPartCollection, realTimeTrackPartsCollection, false, plannedTimeInterval,
				new DayTimeTrack.ScheduleInterval(realTimeTrackPart.EnterDateTime, realTimeTrackPart.ExitDateTime));

			//Assert
			Assert.IsTrue(type == TimeTrackType.Presence);
		}

		[Test]
		public void GetTimeTrackTypeNoneTimeTrackType()
		{
			//Arrange
			var dayTimeTrack = new DayTimeTrack();
			var plannedTimeInterval = new DayTimeTrack.ScheduleInterval(TIME.Date + TimeSpan.FromHours(12), TIME.Date + TimeSpan.FromHours(18));

			var realTimeTrackPart = new TimeTrackPart { EnterDateTime = TIME.Date + TimeSpan.FromHours(12), ExitDateTime = TIME.Date + TimeSpan.FromHours(13) };
			var plannedTimeTrackPart = new TimeTrackPart
			{
				EnterDateTime = plannedTimeInterval.StartTime,
				ExitDateTime = plannedTimeInterval.EndTime
			};

			var realTimeTrackPartsCollection = new List<TimeTrackPart> {realTimeTrackPart};
			var plannedTimeTrackPartCollection = new List<TimeTrackPart> { plannedTimeTrackPart };

			//Act
			TimeTrackType type = dayTimeTrack.GetTimeTrackType(realTimeTrackPart, plannedTimeTrackPartCollection, realTimeTrackPartsCollection, false, plannedTimeInterval,
				new DayTimeTrack.ScheduleInterval(TIME.Date + TimeSpan.FromHours(6), TIME.Date + TimeSpan.FromHours(7)));

			//Assert
			Assert.IsTrue(type == TimeTrackType.None);
		}

		[Test]
		public void GetTimeTrackTypePresenceInBrerakTimeTrackType()
		{
			//Arrange
			var dayTimeTrack = new DayTimeTrack();
			var plannedTimeBeforeLunch = new DayTimeTrack.ScheduleInterval(TIME.Date.Date + TimeSpan.FromHours(6), TIME.Date.Date + TimeSpan.FromHours(12));
			var plannedTimeAfterLunch = new DayTimeTrack.ScheduleInterval(TIME.Date.Date + TimeSpan.FromHours(13), TIME.Date.Date + TimeSpan.FromHours(18));

			var realTimeTrackPart = new TimeTrackPart { EnterDateTime = TIME.Date.Date + TimeSpan.FromHours(12), ExitDateTime = TIME.Date.Date + TimeSpan.FromHours(13) };
			var plannedTimeTrackPartBeforeLunch = new TimeTrackPart
			{
				EnterDateTime = plannedTimeBeforeLunch.StartTime,
				ExitDateTime = plannedTimeBeforeLunch.EndTime
			};
			var plannedTimeTrackPartAfterLunch = new TimeTrackPart
			{
				EnterDateTime = plannedTimeAfterLunch.StartTime,
				ExitDateTime = plannedTimeAfterLunch.EndTime
			};

			var realTimeTrackPartsCollection = new List<TimeTrackPart> {realTimeTrackPart};
			var plannedTimeTrackPartCollection = new List<TimeTrackPart> { plannedTimeTrackPartBeforeLunch, plannedTimeTrackPartAfterLunch};

			//Act
			TimeTrackType type = dayTimeTrack.GetTimeTrackType(realTimeTrackPart, plannedTimeTrackPartCollection,
															realTimeTrackPartsCollection, false,
															dayTimeTrack.GetPlannedScheduleInterval(plannedTimeTrackPartCollection),
															new DayTimeTrack.ScheduleInterval(realTimeTrackPart.EnterDateTime, realTimeTrackPart.ExitDateTime));

			//Assert
			Assert.IsTrue(type == TimeTrackType.PresenceInBrerak);
		}

		[Test]
		public void GetTimeTrackTypeAbsenceTimeTrackType()
		{
			//Arrange
			var dayTimeTrack = new DayTimeTrack();
			var plannedTimeInterval = new DayTimeTrack.ScheduleInterval(TIME.Date + TimeSpan.FromHours(8), TIME.Date + TimeSpan.FromHours(18));

			var realTimeTrackPart = new TimeTrackPart { EnterDateTime = TIME.Date + TimeSpan.FromHours(5), ExitDateTime = TIME.Date + TimeSpan.FromHours(7) };
			var plannedTimeTrackPart = new TimeTrackPart
			{
				EnterDateTime = plannedTimeInterval.StartTime,
				ExitDateTime = plannedTimeInterval.EndTime
			};

			var realTimeTrackPartsCollection = new List<TimeTrackPart> {realTimeTrackPart};
			var plannedTimeTrackPartCollection = new List<TimeTrackPart> { plannedTimeTrackPart };

			//Act
			TimeTrackType type = dayTimeTrack.GetTimeTrackType(realTimeTrackPart, plannedTimeTrackPartCollection, realTimeTrackPartsCollection, false, plannedTimeInterval,
				new DayTimeTrack.ScheduleInterval(TIME.Date + TimeSpan.FromHours(8), TIME.Date + TimeSpan.FromHours(10)));

			//Assert
			Assert.IsTrue(type == TimeTrackType.Absence);
		}

		[Test]
		public void GetTimeTrackTypeAbsenceInsidePlanTimeTrackType()
		{
			//Arrange
			const bool isOnlyFirstEnter = true;
			var dayTimeTrack = new DayTimeTrack();
			var plannedTimeInterval = new DayTimeTrack.ScheduleInterval(TIME.Date + TimeSpan.FromHours(9), TIME.Date + TimeSpan.FromHours(18));

			var realTimeTrackPart = new TimeTrackPart { EnterDateTime = TIME.Date + TimeSpan.FromHours(9), ExitDateTime = TIME.Date + TimeSpan.FromHours(10) };
			var realTimeTrackPart2 = new TimeTrackPart { EnterDateTime = TIME.Date + TimeSpan.FromHours(13), ExitDateTime = TIME.Date + TimeSpan.FromHours(15) };
			var realTimeTrackPart3 = new TimeTrackPart { EnterDateTime = TIME.Date + TimeSpan.FromHours(15), ExitDateTime = TIME.Date + TimeSpan.FromHours(19) };
			var plannedTimeTrackPart = new TimeTrackPart
			{
				EnterDateTime = plannedTimeInterval.StartTime,
				ExitDateTime = plannedTimeInterval.EndTime
			};

			var realTimeTrackPartsCollection = new List<TimeTrackPart> { realTimeTrackPart, realTimeTrackPart2, realTimeTrackPart3 };
			var plannedTimeTrackPartCollection = new List<TimeTrackPart> { plannedTimeTrackPart };

			//Act
			TimeTrackType type = dayTimeTrack.GetTimeTrackType(realTimeTrackPart, plannedTimeTrackPartCollection, realTimeTrackPartsCollection, isOnlyFirstEnter, plannedTimeInterval,
				new DayTimeTrack.ScheduleInterval(TIME.Date + TimeSpan.FromHours(11), TIME.Date + TimeSpan.FromHours(12)));

			//Assert
			Assert.IsTrue(type == TimeTrackType.AbsenceInsidePlan);
		}


		#endregion

		#region GetDeltaForTimeTrack Tests

		[Test]
		public void GetDeltaForAbsenceTimeTrackType()
		{
			//Arrange
			var timeTrackPart = new TimeTrackPart
			{
				TimeTrackPartType = TimeTrackType.Absence,
				EnterDateTime = TIME.Date + TimeSpan.FromHours(8),
				ExitDateTime = TIME.Date + TimeSpan.FromHours(18)
			};
			const bool isHoliday = default(bool);

			//Act
			var dayTimeTrack = new DayTimeTrack();
			var result = dayTimeTrack.GetDeltaForTimeTrack(timeTrackPart, isHoliday);

			//Assert
			Assert.AreEqual(result.Hours, 10);
		}

		[Test]
		public void GetDeltaForAbsenceHolidayTimeTrackType()
		{
			//Arrange
			var timeTrackPart = new TimeTrackPart
			{
				TimeTrackPartType = TimeTrackType.Absence,
				EnterDateTime = TIME.Date + TimeSpan.FromHours(8),
				ExitDateTime = TIME.Date + TimeSpan.FromHours(18)
			};
			const bool isHoliday = true;

			//Act
			var dayTimeTrack = new DayTimeTrack();
			var result = dayTimeTrack.GetDeltaForTimeTrack(timeTrackPart, isHoliday);

			//Assert
			Assert.AreEqual(result.Hours, 0);
		}

		[Test]
		public void GetDeltaForPresenceTimeTrackType()
		{
			//Arrange
			var timeTrackPart = new TimeTrackPart
			{
				TimeTrackPartType = TimeTrackType.Presence,
				EnterDateTime = TIME.Date + TimeSpan.FromHours(8),
				ExitDateTime = TIME.Date + TimeSpan.FromHours(18)
			};
			const bool isHoliday = default(bool);

			//Act
			var dayTimeTrack = new DayTimeTrack();
			var result = dayTimeTrack.GetDeltaForTimeTrack(timeTrackPart, isHoliday);

			//Assert
			Assert.AreEqual(result.Hours, 10);
		}

		[Test]
		public void GetDeltaForPresenceHolidayTimeTrackType()
		{
			//Arrange
			var timeTrackPart = new TimeTrackPart
			{
				TimeTrackPartType = TimeTrackType.Presence,
				EnterDateTime = TIME.Date + TimeSpan.FromHours(8),
				ExitDateTime = TIME.Date + TimeSpan.FromHours(18)
			};
			const bool isHoliday = true;

			//Act
			var dayTimeTrack = new DayTimeTrack();
			var result = dayTimeTrack.GetDeltaForTimeTrack(timeTrackPart, isHoliday);

			//Assert
			Assert.AreEqual(result.Hours, 10);
		}

		[Test]
		public void GetDeltaForAbsenceInsidePlanTimeTrackType()
		{
			//Arrange
			var timeTrackPart = new TimeTrackPart
			{
				TimeTrackPartType = TimeTrackType.AbsenceInsidePlan,
				EnterDateTime = TIME.Date + TimeSpan.FromHours(8),
				ExitDateTime = TIME.Date + TimeSpan.FromHours(18)
			};
			const bool isHoliday = default(bool);

			//Act
			var dayTimeTrack = new DayTimeTrack();
			var result = dayTimeTrack.GetDeltaForTimeTrack(timeTrackPart, isHoliday);

			//Assert
			Assert.AreEqual(result.Hours, 10);
		}

		[Test]
		public void GetDeltaForAbsenceInsidePlanHolidayTimeTrackType()
		{
			//Arrange
			var timeTrackPart = new TimeTrackPart
			{
				TimeTrackPartType = TimeTrackType.AbsenceInsidePlan,
				EnterDateTime = TIME.Date + TimeSpan.FromHours(8),
				ExitDateTime = TIME.Date + TimeSpan.FromHours(18)
			};
			const bool isHoliday = true;

			//Act
			var dayTimeTrack = new DayTimeTrack();
			var result = dayTimeTrack.GetDeltaForTimeTrack(timeTrackPart, isHoliday);

			//Assert
			Assert.AreEqual(result.Hours, 10);
		}

		[Test]
		public void GetDeltaForPresenceInBrerakTimeTrackType()
		{
			//Arrange
			var timeTrackPart = new TimeTrackPart
			{
				TimeTrackPartType = TimeTrackType.PresenceInBrerak,
				EnterDateTime = TIME.Date + TimeSpan.FromHours(8),
				ExitDateTime = TIME.Date + TimeSpan.FromHours(18)
			};
			const bool isHoliday = default(bool);

			//Act
			var dayTimeTrack = new DayTimeTrack();
			var result = dayTimeTrack.GetDeltaForTimeTrack(timeTrackPart, isHoliday);

			//Assert
			Assert.AreEqual(result.Hours, 10);
		}

		[Test]
		public void GetDeltaForPresenceInBrerakHolidayTimeTrackType()
		{
			//Arrange
			var timeTrackPart = new TimeTrackPart
			{
				TimeTrackPartType = TimeTrackType.PresenceInBrerak,
				EnterDateTime = TIME.Date + TimeSpan.FromHours(8),
				ExitDateTime = TIME.Date + TimeSpan.FromHours(18)
			};
			const bool isHoliday = true;

			//Act
			var dayTimeTrack = new DayTimeTrack();
			var result = dayTimeTrack.GetDeltaForTimeTrack(timeTrackPart, isHoliday);

			//Assert
			Assert.AreEqual(result.Hours, 10);
		}

		[Test]
		public void GetDeltaForLateTimeTrackType()
		{
			//Arrange
			var timeTrackPart = new TimeTrackPart
			{
				TimeTrackPartType = TimeTrackType.Late,
				EnterDateTime = TIME.Date + TimeSpan.FromHours(8),
				ExitDateTime = TIME.Date + TimeSpan.FromHours(18)
			};
			const bool isHoliday = default(bool);

			//Act
			var dayTimeTrack = new DayTimeTrack();
			var result = dayTimeTrack.GetDeltaForTimeTrack(timeTrackPart, isHoliday);

			//Assert
			Assert.AreEqual(result.Hours, 10);
		}

		[Test]
		public void GetDeltaForLateHolidayTimeTrackType()
		{
			//Arrange
			var timeTrackPart = new TimeTrackPart
			{
				TimeTrackPartType = TimeTrackType.Late,
				EnterDateTime = TIME.Date + TimeSpan.FromHours(8),
				ExitDateTime = TIME.Date + TimeSpan.FromHours(18)
			};
			const bool isHoliday = true;

			//Act
			var dayTimeTrack = new DayTimeTrack();
			var result = dayTimeTrack.GetDeltaForTimeTrack(timeTrackPart, isHoliday);

			//Assert
			Assert.AreEqual(result.Hours, 0);
		}

		[Test]
		public void GetDeltaForEarlyLeaveTimeTrackType()
		{
			//Arrange
			var timeTrackPart = new TimeTrackPart
			{
				TimeTrackPartType = TimeTrackType.EarlyLeave,
				EnterDateTime = TIME.Date + TimeSpan.FromHours(8),
				ExitDateTime = TIME.Date + TimeSpan.FromHours(18)
			};
			const bool isHoliday = default(bool);

			//Act
			var dayTimeTrack = new DayTimeTrack();
			var result = dayTimeTrack.GetDeltaForTimeTrack(timeTrackPart, isHoliday);

			//Assert
			Assert.AreEqual(result.Hours, 10);
		}

		[Test]
		public void GetDeltaForEarlyLeaveHolidayTimeTrackType()
		{
			//Arrange
			var timeTrackPart = new TimeTrackPart
			{
				TimeTrackPartType = TimeTrackType.EarlyLeave,
				EnterDateTime = TIME.Date + TimeSpan.FromHours(8),
				ExitDateTime = TIME.Date + TimeSpan.FromHours(18)
			};
			const bool isHoliday = true;

			//Act
			var dayTimeTrack = new DayTimeTrack();
			var result = dayTimeTrack.GetDeltaForTimeTrack(timeTrackPart, isHoliday);

			//Assert
			Assert.AreEqual(result.Hours, 0);
		}

		[Test]
		public void GetDeltaForOvertimeTimeTrackType()
		{
			//Arrange
			var timeTrackPart = new TimeTrackPart
			{
				TimeTrackPartType = TimeTrackType.Overtime,
				EnterDateTime = TIME.Date + TimeSpan.FromHours(8),
				ExitDateTime = TIME.Date + TimeSpan.FromHours(18)
			};
			const bool isHoliday = default(bool);

			//Act
			var dayTimeTrack = new DayTimeTrack();
			var result = dayTimeTrack.GetDeltaForTimeTrack(timeTrackPart, isHoliday);

			//Assert
			Assert.AreEqual(result.Hours, 10);
		}

		[Test]
		public void GetDeltaForOvertimeHolidayTimeTrackType()
		{
			//Arrange
			var timeTrackPart = new TimeTrackPart
			{
				TimeTrackPartType = TimeTrackType.Overtime,
				EnterDateTime = TIME.Date + TimeSpan.FromHours(8),
				ExitDateTime = TIME.Date + TimeSpan.FromHours(18)
			};
			const bool isHoliday = true;

			//Act
			var dayTimeTrack = new DayTimeTrack();
			var result = dayTimeTrack.GetDeltaForTimeTrack(timeTrackPart, isHoliday);

			//Assert
			Assert.AreEqual(result.Hours, 10);
		}

		[Test]
		public void GetDeltaForNightTimeTrackType()
		{
			//Arrange
			var timeTrackPart = new TimeTrackPart
			{
				TimeTrackPartType = TimeTrackType.Night,
				EnterDateTime = TIME.Date + TimeSpan.FromHours(8),
				ExitDateTime = TIME.Date + TimeSpan.FromHours(18)
			};
			const bool isHoliday = default(bool);

			//Act
			var dayTimeTrack = new DayTimeTrack();
			var result = dayTimeTrack.GetDeltaForTimeTrack(timeTrackPart, isHoliday);

			//Assert
			Assert.AreEqual(result.Hours, 10);
		}

		[Test]
		public void GetDeltaFoNightHolidayTimeTrackType()
		{
			//Arrange
			var timeTrackPart = new TimeTrackPart
			{
				TimeTrackPartType = TimeTrackType.Night,
				EnterDateTime = TIME.Date + TimeSpan.FromHours(8),
				ExitDateTime = TIME.Date + TimeSpan.FromHours(18)
			};
			const bool isHoliday = true;

			//Act
			var dayTimeTrack = new DayTimeTrack();
			var result = dayTimeTrack.GetDeltaForTimeTrack(timeTrackPart, isHoliday);

			//Assert
			Assert.AreEqual(result.Hours, 10);
		}

		#endregion

		#region GetBalance Tests

		[Test]
		public void GetBalanceForAbsenceTimeTrackType()
		{
			//Arrange
			var timeTrackPart = new TimeTrackPart
			{
				TimeTrackPartType = TimeTrackType.Absence,
				EnterDateTime = TIME.Date + TimeSpan.FromHours(8),
				ExitDateTime = TIME.Date + TimeSpan.FromHours(18)
			};

			var slideTimeSeconds = TimeSpan.FromHours(10).TotalSeconds;


			//Act
			var dayTimeTrack = new DayTimeTrack();
			var result = dayTimeTrack.GetBalance(timeTrackPart, slideTimeSeconds);

			//Assert
			Assert.AreEqual(result, TimeSpan.Zero);
		}

		[Test]
		public void GetBalanceForPresenceTimeTrackType()
		{
			//Arrange
			var timeTrackPart = new TimeTrackPart
			{
				TimeTrackPartType = TimeTrackType.Presence,
				EnterDateTime = TIME.Date + TimeSpan.FromHours(8),
				ExitDateTime = TIME.Date + TimeSpan.FromHours(15)
			};

			var slideTimeSeconds = TimeSpan.FromHours(10).TotalSeconds;

			//Act
			var dayTimeTrack = new DayTimeTrack();
			var result = dayTimeTrack.GetBalance(timeTrackPart, slideTimeSeconds);

			//Assert
			Assert.AreEqual(result, TimeSpan.FromHours(7));
		}

		[Test]
		public void GetBalanceForAbsenceInsidePlanTimeTrackType()
		{
			//Arrange
			var timeTrackPart = new TimeTrackPart
			{
				TimeTrackPartType = TimeTrackType.AbsenceInsidePlan,
				EnterDateTime = TIME.Date + TimeSpan.FromHours(8),
				ExitDateTime = TIME.Date + TimeSpan.FromHours(15)
			};

			var slideTimeSeconds = TimeSpan.FromHours(10).TotalSeconds;

			//Act
			var dayTimeTrack = new DayTimeTrack();
			var result = dayTimeTrack.GetBalance(timeTrackPart, slideTimeSeconds);

			//Assert
			Assert.AreEqual(result, TimeSpan.Zero);
		}

		[Test]
		public void GetBalanceForPresenceInBrerakTimeTrackType()
		{
			//Arrange
			var timeTrackPart = new TimeTrackPart
			{
				TimeTrackPartType = TimeTrackType.PresenceInBrerak,
				EnterDateTime = TIME.Date + TimeSpan.FromHours(8),
				ExitDateTime = TIME.Date + TimeSpan.FromHours(15)
			};

			var slideTimeSeconds = TimeSpan.FromHours(10).TotalSeconds;

			//Act
			var dayTimeTrack = new DayTimeTrack();
			var result = dayTimeTrack.GetBalance(timeTrackPart, slideTimeSeconds);

			//Assert
			Assert.AreEqual(result, TimeSpan.Zero);
		}

		[Test]
		public void GetBalanceForLateTimeTrackType()
		{
			//Arrange
			var timeTrackPart = new TimeTrackPart
			{
				TimeTrackPartType = TimeTrackType.Late,
				EnterDateTime = TIME.Date + TimeSpan.FromHours(10),
				ExitDateTime = TIME.Date + TimeSpan.FromHours(15)
			};

			var slideTimeSeconds = TimeSpan.FromHours(10).TotalSeconds;

			//Act
			var dayTimeTrack = new DayTimeTrack();
			var result = dayTimeTrack.GetBalance(timeTrackPart, slideTimeSeconds);

			//Assert
			Assert.AreEqual(result, TimeSpan.Zero);
		}

		[Test]
		public void GetBalanceForEarlyLeaveTimeTrackType()
		{
			//Arrange
			var timeTrackPart = new TimeTrackPart
			{
				TimeTrackPartType = TimeTrackType.EarlyLeave,
				EnterDateTime = TIME.Date + TimeSpan.FromHours(10),
				ExitDateTime = TIME.Date + TimeSpan.FromHours(15)
			};

			var slideTimeSeconds = TimeSpan.FromHours(10).TotalSeconds;

			//Act
			var dayTimeTrack = new DayTimeTrack();
			var result = dayTimeTrack.GetBalance(timeTrackPart, slideTimeSeconds);

			//Assert
			Assert.AreEqual(result, TimeSpan.Zero);
		}

		[Test]
		public void GetBalanceForOvertimeTimeTrackType()
		{
			//Arrange
			var timeTrackPart = new TimeTrackPart
			{
				TimeTrackPartType = TimeTrackType.Overtime,
				EnterDateTime = TIME.Date + TimeSpan.FromHours(10),
				ExitDateTime = TIME.Date + TimeSpan.FromHours(15)
			};

			var slideTimeSeconds = TimeSpan.FromHours(10).TotalSeconds;

			//Act
			var dayTimeTrack = new DayTimeTrack();
			var result = dayTimeTrack.GetBalance(timeTrackPart, slideTimeSeconds);

			//Assert
			Assert.AreEqual(result, TimeSpan.Zero);
		}

		[Test]
		public void GetBalanceForNightTimeTrackType()
		{
			//Arrange
			var timeTrackPart = new TimeTrackPart
			{
				TimeTrackPartType = TimeTrackType.Night,
				EnterDateTime = TIME.Date + TimeSpan.FromHours(10),
				ExitDateTime = TIME.Date + TimeSpan.FromHours(15)
			};

			var slideTimeSeconds = TimeSpan.FromHours(10).TotalSeconds;

			//Act
			var dayTimeTrack = new DayTimeTrack();
			var result = dayTimeTrack.GetBalance(timeTrackPart, slideTimeSeconds);

			//Assert
			Assert.AreEqual(result, TimeSpan.Zero);
		}

		[Test]
		public void GetBalanceForDocumentOvertimeTimeTrackType()
		{
			//Arrange
			var timeTrackPart = new TimeTrackPart
			{
				TimeTrackPartType = TimeTrackType.DocumentOvertime,
				EnterDateTime = TIME.Date + TimeSpan.FromHours(10),
				ExitDateTime = TIME.Date + TimeSpan.FromHours(15)
			};

			var slideTimeSeconds = TimeSpan.FromHours(10).TotalSeconds;

			//Act
			var dayTimeTrack = new DayTimeTrack();
			var result = dayTimeTrack.GetBalance(timeTrackPart, slideTimeSeconds);

			//Assert
			Assert.AreEqual(result, timeTrackPart.Delta);
		}

		[Test]
		public void GetBalanceForDocumentPresenceTimeTrackType()
		{
			//Arrange
			var timeTrackPart = new TimeTrackPart
			{
				TimeTrackPartType = TimeTrackType.DocumentPresence,
				EnterDateTime = TIME.Date + TimeSpan.FromHours(10),
				ExitDateTime = TIME.Date + TimeSpan.FromHours(15)
			};

			var slideTimeSeconds = TimeSpan.FromHours(10).TotalSeconds;

			//Act
			var dayTimeTrack = new DayTimeTrack();
			var result = dayTimeTrack.GetBalance(timeTrackPart, slideTimeSeconds);

			//Assert
			Assert.AreEqual(result, timeTrackPart.Delta);
		}

		[Test]
		public void GetBalanceForDocumentAbsenceTimeTrackType()
		{
			//Arrange
			var timeTrackPart = new TimeTrackPart
			{
				TimeTrackPartType = TimeTrackType.DocumentAbsence,
				EnterDateTime = TIME.Date + TimeSpan.FromHours(10),
				ExitDateTime = TIME.Date + TimeSpan.FromHours(15)
			};

			var slideTimeSeconds = TimeSpan.FromHours(10).TotalSeconds;

			//Act
			var dayTimeTrack = new DayTimeTrack();
			var result = dayTimeTrack.GetBalance(timeTrackPart, slideTimeSeconds);

			//Assert
			Assert.AreEqual(result, -timeTrackPart.Delta);
		}

		#endregion
	}
}
