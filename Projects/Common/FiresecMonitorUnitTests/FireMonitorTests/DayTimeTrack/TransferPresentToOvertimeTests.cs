using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StrazhAPI.SKD;
using NUnit.Framework;

namespace FiresecMonitorUnitTests.FireMonitorTests
{
	[TestFixture]
	public class TransferPresentToOvertimeTests
	{
		private readonly DateTime TIME = DateTime.Now;
		private List<TimeTrackPart> _plannedTimeTrackParts;
		private TimeSpan _slideTime;


		[SetUp]
		public void TransferPresentToOvertimeTestsSetUp()
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
		public void TransferPresentToOvertimeTestsTearDown()
		{
			_slideTime = default(TimeSpan);
			_plannedTimeTrackParts = null;
		}

		[Test]
		public void AllPresence()
		{
			//Arrange
			var real = new List<TimeTrackPart>
			{
				new TimeTrackPart
				{
					EnterDateTime = TIME.Date.AddHours(9),
					ExitDateTime = TIME.Date.AddHours(10),
					IsForURVZone = true
				},
				new TimeTrackPart
				{
					EnterDateTime = TIME.Date.AddHours(10),
					ExitDateTime = TIME.Date.AddHours(12),
					IsForURVZone = true
				},
				new TimeTrackPart
				{
					EnterDateTime = TIME.Date.AddHours(12),
					ExitDateTime = TIME.Date.AddHours(17),
					IsForURVZone = true
				}
			};

			var dayTimeTrack = new DayTimeTrack
			{
				PlannedTimeTrackParts = _plannedTimeTrackParts,
				RealTimeTrackParts = real,
				Date = TIME.Date,
				SlideTime = _slideTime
			};

			//Act
			dayTimeTrack.Calculate();

			//Assert
			Assert.AreEqual(dayTimeTrack.Totals.FirstOrDefault(x => x.TimeTrackType == TimeTrackType.Overtime).TimeSpan, TimeSpan.Zero);
		}

		[Test]
		public void Transfer3HoursToOvertime()
		{
			//Arrange
			var real = new List<TimeTrackPart>
			{
				new TimeTrackPart
				{
					EnterDateTime = TIME.Date.AddHours(9),
					ExitDateTime = TIME.Date.AddHours(10),
					IsForURVZone = true
				},
				new TimeTrackPart
				{
					EnterDateTime = TIME.Date.AddHours(10),
					ExitDateTime = TIME.Date.AddHours(12),
					IsForURVZone = true
				},
				new TimeTrackPart
				{
					EnterDateTime = TIME.Date.AddHours(12),
					ExitDateTime = TIME.Date.AddHours(20),
					IsForURVZone = true
				}
			};

			var dayTimeTrack = new DayTimeTrack
			{
				PlannedTimeTrackParts = _plannedTimeTrackParts,
				RealTimeTrackParts = real,
				Date = TIME.Date,
				SlideTime = _slideTime
			};

			//Act
			dayTimeTrack.Calculate();

			//Assert
			Assert.AreEqual(dayTimeTrack.Totals.FirstOrDefault(x => x.TimeTrackType == TimeTrackType.Overtime).TimeSpan, TimeSpan.FromHours(3));
		}

		[Test]
		public void TransferToOvertimeInTheMiddle()
		{
			//Arrange
			var planned = new List<TimeTrackPart>
			{
				new TimeTrackPart
				{
					EnterDateTime = TIME.Date.AddHours(8),
					ExitDateTime = TIME.Date.AddHours(21)
				}
			};
			var real = new List<TimeTrackPart>
			{
				new TimeTrackPart
				{
					EnterDateTime = TIME.Date.AddHours(8),
					ExitDateTime = TIME.Date.AddHours(10),
					IsForURVZone = true
				},
				new TimeTrackPart
				{
					EnterDateTime = TIME.Date.AddHours(10),
					ExitDateTime = TIME.Date.AddHours(12),
					IsForURVZone = true
				},
				new TimeTrackPart
				{
					EnterDateTime = TIME.Date.AddHours(12),
					ExitDateTime = TIME.Date.AddHours(15),
					IsForURVZone = true
				},
				new TimeTrackPart
				{
					EnterDateTime = TIME.Date.AddHours(15),
					ExitDateTime = TIME.Date.AddHours(16),
					IsForURVZone = true
				},
				new TimeTrackPart
				{
					EnterDateTime = TIME.Date.AddHours(16),
					ExitDateTime = TIME.Date.AddHours(17),
					IsForURVZone = true
				},
				new TimeTrackPart
				{
					EnterDateTime = TIME.Date.AddHours(17),
					ExitDateTime = TIME.Date.AddHours(19),
					IsForURVZone = true
				},
				new TimeTrackPart
				{
					EnterDateTime = TIME.Date.AddHours(19),
					ExitDateTime = TIME.Date.AddHours(22),
					IsForURVZone = true
				}
			};

			var dayTimeTrack = new DayTimeTrack
			{
				PlannedTimeTrackParts = planned,
				RealTimeTrackParts = real,
				Date = TIME.Date,
				SlideTime = _slideTime
			};

			//Act
			dayTimeTrack.Calculate();

			//Assert
			Assert.AreEqual(dayTimeTrack.Totals.FirstOrDefault(x => x.TimeTrackType == TimeTrackType.Overtime).TimeSpan, TimeSpan.FromHours(6));
		}
	}
}
