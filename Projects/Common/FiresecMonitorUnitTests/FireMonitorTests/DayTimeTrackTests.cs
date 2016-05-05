using System.Linq;
using StrazhAPI.SKD;
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
		public void GetTimeTrackTypeNonOfficialOvertimeTimeTrackTypeTakeInCalculations()
		{
			//Arrange
			var dayTimeTrack = new DayTimeTrack();
			var plannedTimeInterval = new DayTimeTrack.ScheduleInterval(TIME.Date + TimeSpan.FromHours(12), TIME.Date + TimeSpan.FromHours(18));
			var realTimeTrackPart = new TimeTrackPart { EnterDateTime = TIME.Date + TimeSpan.FromHours(9), ExitDateTime = TIME.Date + TimeSpan.FromHours(10), IsForURVZone = true};
			var plannedTimeTrackPart = new TimeTrackPart
			{
				EnterDateTime = plannedTimeInterval.StartTime,
				ExitDateTime = plannedTimeInterval.EndTime
			};

			var realTimeTrackPartsCollection = new List<TimeTrackPart> {realTimeTrackPart};
			var plannedTimeTrackPartCollection = new List<TimeTrackPart> {plannedTimeTrackPart};

			//Act
			TimeTrackType type = dayTimeTrack.GetTimeTrackType(realTimeTrackPart, plannedTimeTrackPartCollection, realTimeTrackPartsCollection, plannedTimeInterval,
				new DayTimeTrack.ScheduleInterval(realTimeTrackPart.EnterDateTime, realTimeTrackPart.ExitDateTime));

			//Assert
			Assert.IsTrue(type == TimeTrackType.Overtime);
		}

		[Test]
		public void GetTimeTrackTypeNonOfficialOvertimeTimeTrackTypeNotTakeInCalculations()
		{
			//Arrange
			var dayTimeTrack = new DayTimeTrack();
			var plannedTimeInterval = new DayTimeTrack.ScheduleInterval(TIME.Date + TimeSpan.FromHours(12), TIME.Date + TimeSpan.FromHours(18));
			var realTimeTrackPart = new TimeTrackPart { EnterDateTime = TIME.Date + TimeSpan.FromHours(9), ExitDateTime = TIME.Date + TimeSpan.FromHours(10), IsForURVZone = true, NotTakeInCalculations = true};
			var plannedTimeTrackPart = new TimeTrackPart
			{
				EnterDateTime = plannedTimeInterval.StartTime,
				ExitDateTime = plannedTimeInterval.EndTime
			};

			var realTimeTrackPartsCollection = new List<TimeTrackPart> { realTimeTrackPart };
			var plannedTimeTrackPartCollection = new List<TimeTrackPart> { plannedTimeTrackPart };

			//Act
			TimeTrackType type = dayTimeTrack.GetTimeTrackType(realTimeTrackPart, plannedTimeTrackPartCollection, realTimeTrackPartsCollection, plannedTimeInterval,
				new DayTimeTrack.ScheduleInterval(realTimeTrackPart.EnterDateTime, realTimeTrackPart.ExitDateTime));

			//Assert
			Assert.IsTrue(type == TimeTrackType.None);
		}

		[Test]
		public void GetTimeTrackTypePresentTimeTrackType()
		{
			//Arrange
			var dayTimeTrack = new DayTimeTrack();
			var plannedTimeInterval = new DayTimeTrack.ScheduleInterval(TIME.Date + TimeSpan.FromHours(12), TIME.Date + TimeSpan.FromHours(18));

			var realTimeTrackPart = new TimeTrackPart { EnterDateTime = TIME.Date + TimeSpan.FromHours(12), ExitDateTime = TIME.Date + TimeSpan.FromHours(13), IsForURVZone = true};
			var plannedTimeTrackPart = new TimeTrackPart
			{
				EnterDateTime = plannedTimeInterval.StartTime,
				ExitDateTime = plannedTimeInterval.EndTime
			};

			var realTimeTrackPartsCollection = new List<TimeTrackPart> { realTimeTrackPart };
			var plannedTimeTrackPartCollection = new List<TimeTrackPart> { plannedTimeTrackPart };

			//Act
			TimeTrackType type = dayTimeTrack.GetTimeTrackType(realTimeTrackPart, plannedTimeTrackPartCollection, realTimeTrackPartsCollection, plannedTimeInterval,
				new DayTimeTrack.ScheduleInterval(realTimeTrackPart.EnterDateTime, realTimeTrackPart.ExitDateTime));

			//Assert
			Assert.IsTrue(type == TimeTrackType.Presence);
		}

		[Test]
		public void GetTimeTrackTypePresentTimeTrackTypeNotTakeInCalculation()
		{
			//Arrange
			var dayTimeTrack = new DayTimeTrack();
			var plannedTimeInterval = new DayTimeTrack.ScheduleInterval(TIME.Date + TimeSpan.FromHours(12), TIME.Date + TimeSpan.FromHours(18));

			var realTimeTrackPart = new TimeTrackPart { EnterDateTime = TIME.Date + TimeSpan.FromHours(12), ExitDateTime = TIME.Date + TimeSpan.FromHours(13), IsForURVZone = true, NotTakeInCalculations = true };
			var plannedTimeTrackPart = new TimeTrackPart
			{
				EnterDateTime = plannedTimeInterval.StartTime,
				ExitDateTime = plannedTimeInterval.EndTime
			};

			var realTimeTrackPartsCollection = new List<TimeTrackPart> { realTimeTrackPart };
			var plannedTimeTrackPartCollection = new List<TimeTrackPart> { plannedTimeTrackPart };

			//Act
			TimeTrackType type = dayTimeTrack.GetTimeTrackType(realTimeTrackPart, plannedTimeTrackPartCollection, realTimeTrackPartsCollection, plannedTimeInterval,
				new DayTimeTrack.ScheduleInterval(realTimeTrackPart.EnterDateTime, realTimeTrackPart.ExitDateTime));

			//Assert
			Assert.IsTrue(type == TimeTrackType.Absence);
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
			TimeTrackType type = dayTimeTrack.GetTimeTrackType(realTimeTrackPart, plannedTimeTrackPartCollection, realTimeTrackPartsCollection, plannedTimeInterval,
				new DayTimeTrack.ScheduleInterval(TIME.Date + TimeSpan.FromHours(6), TIME.Date + TimeSpan.FromHours(7)));

			//Assert
			Assert.IsTrue(type == TimeTrackType.None);
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
			TimeTrackType type = dayTimeTrack.GetTimeTrackType(realTimeTrackPart, plannedTimeTrackPartCollection, realTimeTrackPartsCollection, plannedTimeInterval,
				new DayTimeTrack.ScheduleInterval(TIME.Date + TimeSpan.FromHours(8), TIME.Date + TimeSpan.FromHours(10)));

			//Assert
			Assert.IsTrue(type == TimeTrackType.Absence);
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
			Assert.AreEqual(result, TimeSpan.FromHours(10));
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
			Assert.AreEqual(result, TimeSpan.FromHours(5));
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

		#region IsOnlyFirstEnter Tests

		[Test]
		public void IsOnlyFirstEnterEarlyLeave()
		{
			//Arrange
			var realTimeTrackParts = new List<TimeTrackPart>
			{
				new TimeTrackPart
				{
					EnterDateTime = TIME.Date.Date + TimeSpan.FromHours(9),
					ExitDateTime = TIME.Date.Date + TimeSpan.FromHours(11),
					IsForURVZone = true
				},
				new TimeTrackPart
				{
					EnterDateTime = TIME.Date.Date + TimeSpan.FromHours(12),
					ExitDateTime = TIME.Date.Date + TimeSpan.FromHours(14),
					IsForURVZone = true
				}
			};

			var plannedTimeTrackParts = new List<TimeTrackPart>
			{
				new TimeTrackPart
				{
					EnterDateTime = TIME.Date.Date + TimeSpan.FromHours(9),
					ExitDateTime = TIME.Date.Date + TimeSpan.FromHours(18)
				}
			};

			var dayTimeTrack = new DayTimeTrack
			{
				IsOnlyFirstEnter = true,
				RealTimeTrackParts = realTimeTrackParts,
				PlannedTimeTrackParts = plannedTimeTrackParts
			};
			//Act
			dayTimeTrack.Calculate();
			//Assert
			var firstOrDefault = dayTimeTrack.CombinedTimeTrackParts.FirstOrDefault(x => x.EnterDateTime.TimeOfDay == TimeSpan.FromHours(14));
			if (firstOrDefault != null)
				Assert.AreEqual(firstOrDefault.TimeTrackPartType, TimeTrackType.EarlyLeave);
		}

		[Test]
		public void IsOnlyFirstEnterPresence()
		{
			//Arrange
			var realTimeTrackParts = new List<TimeTrackPart>
			{
				new TimeTrackPart
				{
					EnterDateTime = TIME.Date.Date + TimeSpan.FromHours(9),
					ExitDateTime = TIME.Date.Date + TimeSpan.FromHours(11),
					IsForURVZone = true
				},
				new TimeTrackPart
				{
					EnterDateTime = TIME.Date.Date + TimeSpan.FromHours(12),
					ExitDateTime = TIME.Date.Date + TimeSpan.FromHours(14),
					IsForURVZone = true
				},
				new TimeTrackPart
				{
					EnterDateTime = TIME.Date.Date + TimeSpan.FromHours(16),
					ExitDateTime = TIME.Date.Date + TimeSpan.FromHours(18),
					IsForURVZone = true
				}
			};

			var plannedTimeTrackParts = new List<TimeTrackPart>
			{
				new TimeTrackPart
				{
					EnterDateTime = TIME.Date.Date + TimeSpan.FromHours(9),
					ExitDateTime = TIME.Date.Date + TimeSpan.FromHours(18)
				}
			};

			var dayTimeTrack = new DayTimeTrack
			{
				IsOnlyFirstEnter = true,
				RealTimeTrackParts = realTimeTrackParts,
				PlannedTimeTrackParts = plannedTimeTrackParts
			};
			//Act
			dayTimeTrack.Calculate();
			//Assert
			var firstOrDefault = dayTimeTrack.CombinedTimeTrackParts.FirstOrDefault(x => x.EnterDateTime.TimeOfDay == TimeSpan.FromHours(11));
			if (firstOrDefault != null)
				Assert.AreEqual(firstOrDefault.TimeTrackPartType, TimeTrackType.Presence);
		}

		[Test]
		public void IsOnlyFirstEnterOvertime()
		{
			//Arrange
			var realTimeTrackParts = new List<TimeTrackPart>
			{
				new TimeTrackPart
				{
					EnterDateTime = TIME.Date.Date + TimeSpan.FromHours(9),
					ExitDateTime = TIME.Date.Date + TimeSpan.FromHours(11),
					IsForURVZone = true
				},
				new TimeTrackPart
				{
					EnterDateTime = TIME.Date.Date + TimeSpan.FromHours(12),
					ExitDateTime = TIME.Date.Date + TimeSpan.FromHours(14),
					IsForURVZone = true
				},
				new TimeTrackPart
				{
					EnterDateTime = TIME.Date.Date + TimeSpan.FromHours(16),
					ExitDateTime = TIME.Date.Date + TimeSpan.FromHours(18),
					IsForURVZone = true
				}
			};

			var plannedTimeTrackParts = new List<TimeTrackPart>
			{
				new TimeTrackPart
				{
					EnterDateTime = TIME.Date.Date + TimeSpan.FromHours(9),
					ExitDateTime = TIME.Date.Date + TimeSpan.FromHours(18)
				}
			};

			var dayTimeTrack = new DayTimeTrack
			{
				IsOnlyFirstEnter = true,
				RealTimeTrackParts = realTimeTrackParts,
				PlannedTimeTrackParts = plannedTimeTrackParts
			};
			//Act
			dayTimeTrack.Calculate();
			//Assert
			var firstOrDefault = dayTimeTrack.CombinedTimeTrackParts.FirstOrDefault(x => x.EnterDateTime.TimeOfDay == TimeSpan.FromHours(17));
			if (firstOrDefault != null)
				Assert.AreEqual(firstOrDefault.TimeTrackPartType, TimeTrackType.Overtime);
		}

		#endregion

		#region GetNightTimeTests

		[Test]
		public void CactulateNightTimeInNonURVZones()
		{
			//Arrange
			var dayTimeTrack = new DayTimeTrack();
			var nightSettings = new NightSettings
			{
				IsNightSettingsEnabled = true,
				NightStartTime = TimeSpan.FromHours(20),
				NightEndTime = TimeSpan.FromHours(23)
			};
			var realTimeTrackParts = new List<TimeTrackPart>
			{
				new TimeTrackPart
				{
					EnterDateTime = TIME.AddDays(-1).AddHours(20),
					ExitDateTime = TIME.AddDays(-1).AddHours(23),
					IsForURVZone = false,
					NotTakeInCalculations = true
				}
			};
			//Act
			var nightTime = dayTimeTrack.GetNightTime(nightSettings, realTimeTrackParts);
			//Assert
			Assert.AreEqual(nightTime, TimeSpan.Zero);
		}

		#endregion

		[Test]
		public void CalculateIsCrossNightDeltaWithSameDateTimes()
		{
			//Arrange
			var dayTimeTrack = new DayTimeTrack();
			var plannedTimeTrackParts = new List<TimeTrackPart>
			{
				new TimeTrackPart
				{
					EnterDateTime = TIME.Date.Date + new TimeSpan(0, 0, 0),
					ExitDateTime = TIME.Date.Date + new TimeSpan(8, 0, 0)
				},
				new TimeTrackPart
				{
					EnterDateTime = TIME.Date.Date + new TimeSpan(18, 0, 0),
					ExitDateTime = TIME.Date.Date + new TimeSpan(23, 59, 59)
				}
			};

			var realTimeTrackParts = new List<TimeTrackPart>
			{
				new TimeTrackPart
				{
					EnterDateTime = TIME.Date.Date + new TimeSpan(18, 0, 0),
					ExitDateTime = TIME.Date.Date + new TimeSpan(23, 59, 59),
					IsForURVZone = true
				}
			};

			dayTimeTrack.PlannedTimeTrackParts = plannedTimeTrackParts;
			dayTimeTrack.RealTimeTrackParts = realTimeTrackParts;
			dayTimeTrack.RealTimeTrackPartsForCalculates = realTimeTrackParts;

			//Act
			dayTimeTrack.Calculate();
			var result = dayTimeTrack.CombinedTimeTrackParts
						.Where(x => x.EnterDateTime.TimeOfDay == x.ExitDateTime.GetValueOrDefault().TimeOfDay && x.Delta != new TimeSpan())
						.Select(x => x.Delta)
						.ToList();

			//Assert
			Assert.That(result, Is.EqualTo(new List<TimeSpan>()));
		}

		#region AdjustmentCombinedTimeTracks
		[Test]
		public void CalculateEarlyLeaveTimeForScheduleWithNorm()
		{
			//Arrange
			var dayTimeTrack = new DayTimeTrack {SlideTime = TimeSpan.FromHours(8)};
			var plannedTimeTrackPart = new List<TimeTrackPart>
			{
				new TimeTrackPart
				{
					EnterDateTime = TIME.Date + TimeSpan.FromHours(9),
					ExitDateTime = TIME.Date + TimeSpan.FromHours(18)
				}
			};
			var realTimeTrackPart = new List<TimeTrackPart>
			{
				new TimeTrackPart
				{

					EnterDateTime = TIME.Date + TimeSpan.FromHours(8),
					ExitDateTime = TIME.Date + TimeSpan.FromHours(10),
					IsForURVZone = true
				}
			};
			dayTimeTrack.RealTimeTrackParts = realTimeTrackPart;
			dayTimeTrack.PlannedTimeTrackParts = plannedTimeTrackPart;
			//Act
			dayTimeTrack.Calculate();
			//Assert
			var totalEarlyLeave = dayTimeTrack.Totals.FirstOrDefault(x => x.TimeTrackType == TimeTrackType.EarlyLeave);
			if (totalEarlyLeave != null)
				Assert.AreEqual(totalEarlyLeave.TimeSpan, TimeSpan.FromHours(7));

		}
		#endregion
	}
}
