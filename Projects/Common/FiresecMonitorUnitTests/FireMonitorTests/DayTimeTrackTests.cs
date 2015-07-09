using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using FiresecAPI.SKD;
using Infrastructure.Common;
using NUnit.Framework;

namespace FiresecMonitorUnitTests.FireMonitorTests
{
	[TestFixture]
	public class DayTimeTrackTests
	{
		#region Base Tests
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
		public void GetTimeTrackTypePresentTimeTrackType() //TODO: incorrect working with incorrect input data
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
		public void GetTimeTrackTypeNoneTimeTrackType() //TODO: incorrect working with incorrect input data
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
		public void GetTimeTrackTypePresenceInBrerakTimeTrackType() //TODO: incorrect working with incorrect input data
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
		public void GetTimeTrackTypeAbsenceTimeTrackType() //TODO: incorrect working with incorrect input data
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
		public void GetTimeTrackTypeAbsenceInsidePlanTimeTrackType() //TODO: incorrect working with incorrect input data
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
	}
}
