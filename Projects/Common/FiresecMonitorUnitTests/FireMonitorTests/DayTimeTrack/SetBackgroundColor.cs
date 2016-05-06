using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using StrazhAPI.SKD;
using NUnit.Framework;

namespace FiresecMonitorUnitTests.FireMonitorTests
{
	[TestFixture]
	public class SetBackgroundColor
	{
		private readonly DateTime TIME = DateTime.Now;
		private List<TimeTrackPart> _plannedTimeTrackParts;
		private List<TimeTrackPart> _realTimeTrackPartsPresence;
		private List<TimeTrackPart> _realTimeTrackPartsAbsence;
		private List<TimeTrackPart> _realTimeTrackPartsAbsenceWithOvertime;
		private List<TimeTrackPart> _realTimeTrackPartsOvertime;
		private TimeSpan _slideTime;

		[SetUp]
		public void SetBackgroundColorSetUp()
		{
			_slideTime = TimeSpan.FromHours(8);
			_realTimeTrackPartsPresence = new List<TimeTrackPart>
			{
				new TimeTrackPart
				{
					EnterDateTime = TIME.Date.AddHours(9),
					ExitDateTime = TIME.Date.AddHours(17),
					IsForURVZone = true
				}
			};
			_realTimeTrackPartsAbsence = new List<TimeTrackPart>
			{
				new TimeTrackPart
				{
					EnterDateTime = TIME.Date.AddHours(9),
					ExitDateTime = TIME.Date.AddHours(12),
					IsForURVZone = true
				}
			};
			_realTimeTrackPartsAbsenceWithOvertime = new List<TimeTrackPart>
			{
				new TimeTrackPart
				{
					EnterDateTime = TIME.Date.AddHours(9),
					ExitDateTime = TIME.Date.AddHours(12),
					IsForURVZone = true
				},
				new TimeTrackPart
				{
					EnterDateTime = TIME.Date.AddHours(14),
					ExitDateTime = TIME.Date.AddHours(21),
					IsForURVZone = true
				}
			};
			_realTimeTrackPartsOvertime = new List<TimeTrackPart>
			{
				new TimeTrackPart
				{
					EnterDateTime = TIME.Date.AddHours(9),
					ExitDateTime = TIME.Date.AddHours(20),
					IsForURVZone = true
				}
			};
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
		public void SetBackgroundColorTearDown()
		{
			_slideTime = default(TimeSpan);
			_plannedTimeTrackParts = null;
			_realTimeTrackPartsPresence = null;
			_realTimeTrackPartsAbsence = null;
			_realTimeTrackPartsAbsenceWithOvertime = null;
			_realTimeTrackPartsOvertime = null;
		}

		[Test]
		public void SetBackgroundColorWhite()
		{
			//Arrange
			var dayTimeTrack = new DayTimeTrack
			{
				PlannedTimeTrackParts = _plannedTimeTrackParts,
				RealTimeTrackParts = _realTimeTrackPartsPresence,
				SlideTime = _slideTime,
				Date = TIME.Date
			};

			//Act
			dayTimeTrack.Calculate();

			//Assert
			Assert.AreEqual(dayTimeTrack.BackgroundColor, Colors.White);
		}

		[Test]
		public void SetBackgroundColorPink()
		{
			//Arrange
			var dayTimeTrack = new DayTimeTrack
			{
				PlannedTimeTrackParts = _plannedTimeTrackParts,
				RealTimeTrackParts = _realTimeTrackPartsAbsenceWithOvertime,
				SlideTime = _slideTime,
				Date = TIME.Date
			};

			//Act
			dayTimeTrack.Calculate();

			//Assert
			Assert.AreEqual(dayTimeTrack.BackgroundColor, Colors.Pink);
		}

		[Test]
		public void SetBackgroundColorLightGreen()
		{
			//Arrange
			var dayTimeTrack = new DayTimeTrack
			{
				PlannedTimeTrackParts = _plannedTimeTrackParts,
				RealTimeTrackParts = _realTimeTrackPartsOvertime,
				SlideTime = _slideTime,
				Date = TIME.Date
			};

			//Act
			dayTimeTrack.Calculate();

			//Assert
			Assert.AreEqual(dayTimeTrack.BackgroundColor, Colors.LightGreen);
		}

		[Test]
		public void SetBackgroundColorLightCoral()
		{
			//Arrange
			var dayTimeTrack = new DayTimeTrack
			{
				PlannedTimeTrackParts = _plannedTimeTrackParts,
				RealTimeTrackParts = _realTimeTrackPartsAbsence,
				SlideTime = _slideTime,
				Date = TIME.Date
			};

			//Act
			dayTimeTrack.Calculate();

			//Assert
			Assert.AreEqual(dayTimeTrack.BackgroundColor, Colors.LightCoral);
		}

		[Test]
		public void SetBackgroundColorDarkRed()
		{
			//Arrange
			var document = new List<TimeTrackDocument>
			{
				new TimeTrackDocument("Absence", "Abs", 5, DocumentType.Absence)
				{
					StartDateTime = TIME.Date.AddHours(9),
					EndDateTime = TIME.Date.AddHours(18)
				}
			};
			var dayTimeTrack = new DayTimeTrack
			{
				PlannedTimeTrackParts = _plannedTimeTrackParts,
				Documents = document,
				SlideTime = _slideTime,
				Date = TIME.Date
			};

			//Act
			dayTimeTrack.Calculate();

			//Assert
			Assert.AreEqual(dayTimeTrack.BackgroundColor, Colors.DarkRed);
		}

		[Test]
		public void SetBackgroundColorLightGray()
		{
			//Arrange
			var dayTimeTrack = new DayTimeTrack
			{
				SlideTime = _slideTime,
				Date = TIME.Date
			};

			//Act
			dayTimeTrack.Calculate();

			//Assert
			Assert.AreEqual(dayTimeTrack.BackgroundColor, Colors.LightGray);
		}

		[Test]
		public void SetBackgroundColorDarkGreen()
		{
			//Arrange
			var document = new List<TimeTrackDocument>
			{
				new TimeTrackDocument("Overtime", "Ove", 5, DocumentType.Overtime)
				{
					StartDateTime = TIME.Date.AddHours(9),
					EndDateTime = TIME.Date.AddHours(18)
				}
			};
			var dayTimeTrack = new DayTimeTrack
			{
				RealTimeTrackParts = _realTimeTrackPartsAbsence,
				Documents = document,
				SlideTime = _slideTime,
				Date = TIME.Date
			};

			//Act
			dayTimeTrack.Calculate();

			//Assert
			Assert.AreEqual(dayTimeTrack.BackgroundColor, Colors.DarkGreen);
		}

		[Test]
		public void SetBackgroundColorDarkRedForHoliday()
		{
			//Arrange
			var document = new List<TimeTrackDocument>
			{
				new TimeTrackDocument("Absence", "Abs", 5, DocumentType.Absence)
				{
					StartDateTime = TIME.Date.AddHours(9),
					EndDateTime = TIME.Date.AddHours(18),
					IsOutside = true
				}
			};
			var dayTimeTrack = new DayTimeTrack
			{
				Documents = document,
				SlideTime = _slideTime,
				Date = TIME.Date
			};

			//Act
			dayTimeTrack.Calculate();

			//Assert
			Assert.AreEqual(dayTimeTrack.BackgroundColor, Colors.DarkRed);
		}
	}
}
