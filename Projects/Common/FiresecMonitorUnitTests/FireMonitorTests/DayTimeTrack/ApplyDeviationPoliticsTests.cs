using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StrazhAPI.SKD;
using NUnit.Framework;

namespace FiresecMonitorUnitTests.FireMonitorTests
{
	[TestFixture]
	class ApplyDeviationPoliticsTests
	{
		private readonly DateTime TIME = DateTime.Now;
		private List<TimeTrackPart> _plannedTimeTrackParts;
		private TimeSpan _slideTime;

		[SetUp]
		public void ApplyDeviationPoliticsTestsSetUp()
		{
			_slideTime = TimeSpan.FromHours(8);
			_plannedTimeTrackParts = new List<TimeTrackPart>
			{
				new TimeTrackPart
				{
					EnterDateTime = TIME.Date.AddHours(9),
					ExitDateTime = TIME.Date.AddHours(18)
				}
			};
		}

		[TearDown]
		public void ApplyDeviationPoliticsTestsTearDown()
		{
			_slideTime = default(TimeSpan);
			_plannedTimeTrackParts = null;
		}

		[Test]
		public void NotAllowOvertimesLowerThan10MinWithAnotherOvertime()
		{
			//Arrange
			var realWithOvertime = new List<TimeTrackPart>
			{
				new TimeTrackPart
				{
					EnterDateTime = TIME.Date.AddHours(9),
					ExitDateTime = TIME.Date.AddHours(13),
					IsForURVZone = true
				},
				new TimeTrackPart
				{
					EnterDateTime = TIME.Date.AddHours(14),
					ExitDateTime = TIME.Date.AddHours(18).AddMinutes(9),
					IsForURVZone = true
				}
			};

			var dayTimeTrack = new DayTimeTrack
			{
				PlannedTimeTrackParts = _plannedTimeTrackParts,
				RealTimeTrackParts = realWithOvertime,
				NotAllowOvertimeLowerThan = 10,
				IsOnlyFirstEnter = true,
				SlideTime = _slideTime
			};

			//Act
			dayTimeTrack.Calculate();

			//Assert
			var overtime =
				dayTimeTrack.Totals.Where(x => x.TimeTrackType == TimeTrackType.Overtime)
					.Select(x => x.TimeSpan)
					.Aggregate(default(double), (s, x) => s + x.TotalMinutes);
			Assert.AreEqual(overtime, 60.0);
		}
	}
}
