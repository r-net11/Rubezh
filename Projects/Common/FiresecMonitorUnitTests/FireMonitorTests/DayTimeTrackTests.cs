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
		[Test]
		public void GetTimeTrackTypeNonOfficialOvertimeTimeTrackType()
		{
			//Arrange
			var dayTimeTrack = new DayTimeTrack();
			var plannedTimeInterval = new DayTimeTrack.ScheduleInterval(TimeSpan.FromHours(12), TimeSpan.FromHours(18));
			var realTimeTrackPart = new TimeTrackPart { StartTime = TimeSpan.FromHours(9), EndTime = TimeSpan.FromHours(10) };
			var plannedTimeTrackPart = new TimeTrackPart
			{
				StartTime = plannedTimeInterval.StartTime,
				EndTime = plannedTimeInterval.EndTime
			};

			var realTimeTrackPartsCollection = new List<TimeTrackPart> {realTimeTrackPart};
			var plannedTimeTrackPartCollection = new List<TimeTrackPart> {plannedTimeTrackPart};

			//Act
			TimeTrackType type = dayTimeTrack.GetTimeTrackType(realTimeTrackPart, plannedTimeTrackPartCollection, realTimeTrackPartsCollection, false, plannedTimeInterval,
				new DayTimeTrack.ScheduleInterval(realTimeTrackPart.StartTime, realTimeTrackPart.EndTime));

			//Assert
			Assert.IsTrue(type == TimeTrackType.Overtime);
		}

		[Test]
		public void GetTimeTrackTypePresentTimeTrackType()
		{
			//Arrange
			var dayTimeTrack = new DayTimeTrack();
			var plannedTimeInterval = new DayTimeTrack.ScheduleInterval(TimeSpan.FromHours(12), TimeSpan.FromHours(18));

			var realTimeTrackPart = new TimeTrackPart { StartTime = TimeSpan.FromHours(12), EndTime = TimeSpan.FromHours(13) };
			var plannedTimeTrackPart = new TimeTrackPart
			{
				StartTime = plannedTimeInterval.StartTime,
				EndTime = plannedTimeInterval.EndTime
			};

			var realTimeTrackPartsCollection = new List<TimeTrackPart> { realTimeTrackPart };
			var plannedTimeTrackPartCollection = new List<TimeTrackPart> { plannedTimeTrackPart };

			//Act
			TimeTrackType type = dayTimeTrack.GetTimeTrackType(realTimeTrackPart, plannedTimeTrackPartCollection, realTimeTrackPartsCollection, false, plannedTimeInterval,
				new DayTimeTrack.ScheduleInterval(realTimeTrackPart.StartTime, realTimeTrackPart.EndTime));

			//Assert
			Assert.IsTrue(type == TimeTrackType.Presence);
		}

		[Test]
		public void GetTimeTrackTypeNoneTimeTrackType()
		{
			//Arrange
			var dayTimeTrack = new DayTimeTrack();
			var plannedTimeInterval = new DayTimeTrack.ScheduleInterval(TimeSpan.FromHours(12), TimeSpan.FromHours(18));

			var realTimeTrackPart = new TimeTrackPart { StartTime = TimeSpan.FromHours(12), EndTime = TimeSpan.FromHours(13) };
			var plannedTimeTrackPart = new TimeTrackPart
			{
				StartTime = plannedTimeInterval.StartTime,
				EndTime = plannedTimeInterval.EndTime
			};

			var realTimeTrackPartsCollection = new List<TimeTrackPart> {realTimeTrackPart};
			var plannedTimeTrackPartCollection = new List<TimeTrackPart> { plannedTimeTrackPart };

			//Act
			TimeTrackType type = dayTimeTrack.GetTimeTrackType(realTimeTrackPart, plannedTimeTrackPartCollection, realTimeTrackPartsCollection, false, plannedTimeInterval,
				new DayTimeTrack.ScheduleInterval(TimeSpan.FromHours(6), TimeSpan.FromHours(7)));

			//Assert
			Assert.IsTrue(type == TimeTrackType.None);
		}

		[Test]
		public void GetTimeTrackTypePresenceInBrerakTimeTrackType()
		{
			//Arrange
			var dayTimeTrack = new DayTimeTrack();
			var plannedTimeBeforeLunch = new DayTimeTrack.ScheduleInterval(TimeSpan.FromHours(6), TimeSpan.FromHours(12));
			var plannedTimeAfterLunch = new DayTimeTrack.ScheduleInterval(TimeSpan.FromHours(13), TimeSpan.FromHours(18));

			var realTimeTrackPart = new TimeTrackPart { StartTime = TimeSpan.FromHours(12), EndTime = TimeSpan.FromHours(13) };
			var plannedTimeTrackPartBeforeLunch = new TimeTrackPart
			{
				StartTime = plannedTimeBeforeLunch.StartTime,
				EndTime = plannedTimeBeforeLunch.EndTime
			};
			var plannedTimeTrackPartAfterLunch = new TimeTrackPart
			{
				StartTime = plannedTimeAfterLunch.StartTime,
				EndTime = plannedTimeAfterLunch.EndTime
			};

			var realTimeTrackPartsCollection = new List<TimeTrackPart> {realTimeTrackPart};
			var plannedTimeTrackPartCollection = new List<TimeTrackPart> { plannedTimeTrackPartBeforeLunch, plannedTimeTrackPartAfterLunch};

			//Act
			TimeTrackType type = dayTimeTrack.GetTimeTrackType(realTimeTrackPart, plannedTimeTrackPartCollection,
															realTimeTrackPartsCollection, false,
															dayTimeTrack.GetPlannedScheduleInterval(plannedTimeTrackPartCollection),
															new DayTimeTrack.ScheduleInterval(realTimeTrackPart.StartTime, realTimeTrackPart.EndTime));

			//Assert
			Assert.IsTrue(type == TimeTrackType.PresenceInBrerak);
		}

		[Test]
		public void GetTimeTrackTypeAbsenceTimeTrackType()
		{
			//Arrange
			var dayTimeTrack = new DayTimeTrack();
			var plannedTimeInterval = new DayTimeTrack.ScheduleInterval(TimeSpan.FromHours(8), TimeSpan.FromHours(18));

			var realTimeTrackPart = new TimeTrackPart { StartTime = TimeSpan.FromHours(5), EndTime = TimeSpan.FromHours(7) };
			var plannedTimeTrackPart = new TimeTrackPart
			{
				StartTime = plannedTimeInterval.StartTime,
				EndTime = plannedTimeInterval.EndTime
			};

			var realTimeTrackPartsCollection = new List<TimeTrackPart> {realTimeTrackPart};
			var plannedTimeTrackPartCollection = new List<TimeTrackPart> { plannedTimeTrackPart };

			//Act
			TimeTrackType type = dayTimeTrack.GetTimeTrackType(realTimeTrackPart, plannedTimeTrackPartCollection, realTimeTrackPartsCollection, false, plannedTimeInterval,
				new DayTimeTrack.ScheduleInterval(TimeSpan.FromHours(8), TimeSpan.FromHours(10)));

			//Assert
			Assert.IsTrue(type == TimeTrackType.Absence);
		}

		[Test]
		public void GetTimeTrackTypeAbsenceInsidePlanTimeTrackType()
		{
			//Arrange
			const bool isOnlyFirstEnter = true;
			var dayTimeTrack = new DayTimeTrack();
			var plannedTimeInterval = new DayTimeTrack.ScheduleInterval(TimeSpan.FromHours(9), TimeSpan.FromHours(18));

			var realTimeTrackPart = new TimeTrackPart { StartTime = TimeSpan.FromHours(9), EndTime = TimeSpan.FromHours(10) };
			var realTimeTrackPart2 = new TimeTrackPart { StartTime = TimeSpan.FromHours(13), EndTime = TimeSpan.FromHours(15) };
			var realTimeTrackPart3 = new TimeTrackPart { StartTime = TimeSpan.FromHours(15), EndTime = TimeSpan.FromHours(19) };
			var plannedTimeTrackPart = new TimeTrackPart
			{
				StartTime = plannedTimeInterval.StartTime,
				EndTime = plannedTimeInterval.EndTime
			};

			var realTimeTrackPartsCollection = new List<TimeTrackPart> { realTimeTrackPart, realTimeTrackPart2, realTimeTrackPart3 };
			var plannedTimeTrackPartCollection = new List<TimeTrackPart> { plannedTimeTrackPart };

			//Act
			TimeTrackType type = dayTimeTrack.GetTimeTrackType(realTimeTrackPart, plannedTimeTrackPartCollection, realTimeTrackPartsCollection, isOnlyFirstEnter, plannedTimeInterval,
				new DayTimeTrack.ScheduleInterval(TimeSpan.FromHours(11), TimeSpan.FromHours(12)));

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
				StartTime = TimeSpan.FromHours(8),
				EndTime = TimeSpan.FromHours(18)
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
				StartTime = TimeSpan.FromHours(8),
				EndTime = TimeSpan.FromHours(18)
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
				StartTime = TimeSpan.FromHours(8),
				EndTime = TimeSpan.FromHours(18)
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
				StartTime = TimeSpan.FromHours(8),
				EndTime = TimeSpan.FromHours(18)
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
				StartTime = TimeSpan.FromHours(8),
				EndTime = TimeSpan.FromHours(18)
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
				StartTime = TimeSpan.FromHours(8),
				EndTime = TimeSpan.FromHours(18)
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
				StartTime = TimeSpan.FromHours(8),
				EndTime = TimeSpan.FromHours(18)
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
				StartTime = TimeSpan.FromHours(8),
				EndTime = TimeSpan.FromHours(18)
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
				StartTime = TimeSpan.FromHours(8),
				EndTime = TimeSpan.FromHours(18)
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
				StartTime = TimeSpan.FromHours(8),
				EndTime = TimeSpan.FromHours(18)
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
				StartTime = TimeSpan.FromHours(8),
				EndTime = TimeSpan.FromHours(18)
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
				StartTime = TimeSpan.FromHours(8),
				EndTime = TimeSpan.FromHours(18)
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
				StartTime = TimeSpan.FromHours(8),
				EndTime = TimeSpan.FromHours(18)
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
				StartTime = TimeSpan.FromHours(8),
				EndTime = TimeSpan.FromHours(18)
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
				StartTime = TimeSpan.FromHours(8),
				EndTime = TimeSpan.FromHours(18)
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
				StartTime = TimeSpan.FromHours(8),
				EndTime = TimeSpan.FromHours(18)
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
				StartTime = TimeSpan.FromHours(8),
				EndTime = TimeSpan.FromHours(18)
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
				StartTime = TimeSpan.FromHours(8),
				EndTime = TimeSpan.FromHours(15)
			};

			var slideTimeSeconds = TimeSpan.FromHours(10).TotalSeconds;

			//Act
			var dayTimeTrack = new DayTimeTrack();
			var result = dayTimeTrack.GetBalance(timeTrackPart, slideTimeSeconds);

			//Assert
			Assert.AreEqual(result, TimeSpan.Zero);
		}

		[Test]
		public void GetBalanceForAbsenceInsidePlanTimeTrackType()
		{
			//Arrange
			var timeTrackPart = new TimeTrackPart
			{
				TimeTrackPartType = TimeTrackType.AbsenceInsidePlan,
				StartTime = TimeSpan.FromHours(8),
				EndTime = TimeSpan.FromHours(15)
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
				StartTime = TimeSpan.FromHours(8),
				EndTime = TimeSpan.FromHours(15)
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
				StartTime = TimeSpan.FromHours(10),
				EndTime = TimeSpan.FromHours(15)
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
				StartTime = TimeSpan.FromHours(10),
				EndTime = TimeSpan.FromHours(15)
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
				StartTime = TimeSpan.FromHours(10),
				EndTime = TimeSpan.FromHours(15)
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
				StartTime = TimeSpan.FromHours(10),
				EndTime = TimeSpan.FromHours(15)
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
				StartTime = TimeSpan.FromHours(10),
				EndTime = TimeSpan.FromHours(15)
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
				StartTime = TimeSpan.FromHours(10),
				EndTime = TimeSpan.FromHours(15)
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
				StartTime = TimeSpan.FromHours(10),
				EndTime = TimeSpan.FromHours(15)
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
