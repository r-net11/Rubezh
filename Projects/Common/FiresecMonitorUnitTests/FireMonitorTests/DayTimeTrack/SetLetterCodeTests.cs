using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using StrazhAPI.SKD;
using NUnit.Framework;

namespace FiresecMonitorUnitTests.FireMonitorTests
{
	[TestFixture]
	public class SetLetterCodeTests
	{
		private readonly DateTime TIME = DateTime.Now;
		private List<TimeTrackPart> _plannedTimeTrackParts;
		private TimeSpan _slideTime;

		[SetUp]
		public void SetLetterCodeTestsSetUp()
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
		public void SetLetterCodeTestsTearDown()
		{
			_slideTime = default(TimeSpan);
			_plannedTimeTrackParts = null;
		}

		[Test]
		public void SetLetterCodeTotalHours()
		{
			//Arrange
			var real = new List<TimeTrackPart>
			{
				new TimeTrackPart
				{
					EnterDateTime = TIME.Date.AddHours(9),
					ExitDateTime = TIME.Date.AddHours(18),
					IsForURVZone = true
				}
			};
			var realWithSlideTime = new List<TimeTrackPart>
			{
				new TimeTrackPart
				{
					EnterDateTime = TIME.Date.AddHours(9),
					ExitDateTime = TIME.Date.AddHours(17),
					IsForURVZone = true
				}
			};

			var dayTimeTrack = new DayTimeTrack
			{
				RealTimeTrackParts = real,
				PlannedTimeTrackParts = _plannedTimeTrackParts
			};
			var dayTimeTrackWithSlideTime = new DayTimeTrack
			{
				RealTimeTrackParts = realWithSlideTime,
				PlannedTimeTrackParts = _plannedTimeTrackParts,
				SlideTime = _slideTime
			};

			//Act
			dayTimeTrack.Calculate();
			dayTimeTrackWithSlideTime.Calculate();

			//Assert
			Assert.AreEqual(dayTimeTrack.LetterCode, TimeSpan.FromHours(9).TotalHours.ToString("F"));
			Assert.AreEqual(dayTimeTrackWithSlideTime.LetterCode, TimeSpan.FromHours(8).TotalHours.ToString("F"));
		}

		[Test]
		public void SetLetterCodeAbsencesResultAsAbsenceCode()
		{
			//Arrange
			var realLate = new List<TimeTrackPart>
			{
				new TimeTrackPart
				{
					EnterDateTime = TIME.Date.AddHours(11),
					ExitDateTime = TIME.Date.AddHours(18),
					IsForURVZone = true
				}
			};
			var realEarlyLeave = new List<TimeTrackPart>
			{
				new TimeTrackPart
				{
					EnterDateTime = TIME.Date.AddHours(9),
					ExitDateTime = TIME.Date.AddHours(16),
					IsForURVZone = true
				}
			};
			var realAbsence = new List<TimeTrackPart>
			{
				new TimeTrackPart
				{
					EnterDateTime = TIME.Date.AddHours(9),
					ExitDateTime = TIME.Date.AddHours(11),
					IsForURVZone = true
				},
				new TimeTrackPart
				{
					EnterDateTime = TIME.Date.AddHours(15),
					ExitDateTime = TIME.Date.AddHours(18),
					IsForURVZone = true
				}
			};
			var realCombined = new List<TimeTrackPart>
			{
				new TimeTrackPart
				{
					EnterDateTime = TIME.Date.AddHours(10),
					ExitDateTime = TIME.Date.AddHours(11),
					IsForURVZone = true
				},
				new TimeTrackPart
				{
					EnterDateTime = TIME.Date.AddHours(15),
					ExitDateTime = TIME.Date.AddHours(17),
					IsForURVZone = true
				}
			};

			var dayTimeTrackLate = new DayTimeTrack{ RealTimeTrackParts = realLate, PlannedTimeTrackParts = _plannedTimeTrackParts, SlideTime = _slideTime};
			var dayTimeTrackEarlyLeave = new DayTimeTrack { RealTimeTrackParts = realEarlyLeave, PlannedTimeTrackParts = _plannedTimeTrackParts, SlideTime = _slideTime };
			var dayTimeTrackAbserce = new DayTimeTrack { RealTimeTrackParts = realAbsence, PlannedTimeTrackParts = _plannedTimeTrackParts, SlideTime = _slideTime };
			var dayTimeTrackCombined = new DayTimeTrack { RealTimeTrackParts = realCombined, PlannedTimeTrackParts = _plannedTimeTrackParts, SlideTime = _slideTime };

			//Act
			dayTimeTrackLate.Calculate();
			dayTimeTrackEarlyLeave.Calculate();
			dayTimeTrackAbserce.Calculate();
			dayTimeTrackCombined.Calculate();

			//Assert
			Assert.AreEqual(dayTimeTrackLate.LetterCode, "ОП");
			Assert.AreEqual(dayTimeTrackEarlyLeave.LetterCode, "УР");
			Assert.AreEqual(dayTimeTrackAbserce.LetterCode, "НН");
			Assert.AreEqual(dayTimeTrackCombined.LetterCode, "НН");
		}

		[Test]
		public void SetLetterCodeWithDocumentResultAsMaxDocumentCode()
		{
			//Arrange
			var real = new List<TimeTrackPart>
			{
				new TimeTrackPart
				{
					EnterDateTime = TIME.Date.AddHours(9),
					ExitDateTime = TIME.Date.AddHours(12),
					IsForURVZone = true
				}
			};

			var document = new List<TimeTrackDocument>
			{
				new TimeTrackDocument("First", "Fr", 11, DocumentType.Overtime)
				{
					StartDateTime = TIME.Date.AddHours(14),
					EndDateTime = TIME.Date.AddHours(18)
				}
			};

			var documents = new List<TimeTrackDocument>
			{
				new TimeTrackDocument("First", "Fr", 11, DocumentType.Overtime)
				{
					StartDateTime = TIME.Date.AddHours(14),
					EndDateTime = TIME.Date.AddHours(18)
				},
				new TimeTrackDocument("Second", "Sec", 6, DocumentType.Absence)
				{
					StartDateTime = TIME.Date.AddHours(13),
					EndDateTime = TIME.Date.AddHours(14)
				}
			};
			var documentsEqual = new List<TimeTrackDocument>
			{
				new TimeTrackDocument("First", "Fr", 11, DocumentType.Overtime)
				{
					StartDateTime = TIME.Date.AddHours(14),
					EndDateTime = TIME.Date.AddHours(15)
				},
				new TimeTrackDocument("Second", "Sec", 6, DocumentType.Absence)
				{
					StartDateTime = TIME.Date.AddHours(13),
					EndDateTime = TIME.Date.AddHours(14)
				}
			};

			var dayTimeTrackOneDocument = new DayTimeTrack
			{
				RealTimeTrackParts = real,
				PlannedTimeTrackParts = _plannedTimeTrackParts,
				SlideTime = _slideTime,
				Documents = document,
				Date = TIME.Date
			};
			var dayTimeTrackTwoDocuments = new DayTimeTrack
			{
				RealTimeTrackParts = real,
				PlannedTimeTrackParts = _plannedTimeTrackParts,
				SlideTime = _slideTime,
				Documents = documents,
				Date = TIME.Date
			};
			var dayTimeTrackTwoDocumentsEqual = new DayTimeTrack
			{
				RealTimeTrackParts = real,
				PlannedTimeTrackParts = _plannedTimeTrackParts,
				SlideTime = _slideTime,
				Documents = documentsEqual,
				Date = TIME.Date
			};

			//Act
			dayTimeTrackOneDocument.Calculate();
			dayTimeTrackTwoDocuments.Calculate();
			dayTimeTrackTwoDocumentsEqual.Calculate();

			//Assert
			Assert.AreEqual(dayTimeTrackOneDocument.LetterCode, "Fr");
			Assert.AreEqual(dayTimeTrackOneDocument.LetterCode, "Fr");
			Assert.AreEqual(dayTimeTrackTwoDocumentsEqual.LetterCode, "Sec");
		}

		[Test]
		public void SetLetterCodeForHoliday()
		{
			//Arrange
			var real = new List<TimeTrackPart>
			{
				new TimeTrackPart
				{
					EnterDateTime = TIME.Date.AddHours(9),
					ExitDateTime = TIME.Date.AddHours(12),
					IsForURVZone = true
				}
			};

			var dayTimeTrack = new DayTimeTrack
			{
				RealTimeTrackParts = real,
				Date = TIME.Date,
				SlideTime = _slideTime
			};

			//Act
			dayTimeTrack.Calculate();

			//Assert
			Assert.AreEqual(dayTimeTrack.LetterCode, "В");
		}

		[Test]
		public void SetLetterCodeForHolidayWithDocument()
		{
			//Arrange
			var real = new List<TimeTrackPart>
			{
				new TimeTrackPart
				{
					EnterDateTime = TIME.Date.AddHours(9),
					ExitDateTime = TIME.Date.AddHours(12),
					IsForURVZone = true
				}
			};

			var document = new List<TimeTrackDocument>
			{
				new TimeTrackDocument("First", "Fr", 11, DocumentType.Overtime)
				{
					StartDateTime = TIME.Date.AddHours(14),
					EndDateTime = TIME.Date.AddHours(18)
				}
			};

			var documents = new List<TimeTrackDocument>
			{
				new TimeTrackDocument("First", "Fr", 11, DocumentType.Overtime)
				{
					StartDateTime = TIME.Date.AddHours(14),
					EndDateTime = TIME.Date.AddHours(18)
				},
				new TimeTrackDocument("Second", "Sec", 6, DocumentType.Absence)
				{
					StartDateTime = TIME.Date.AddHours(13),
					EndDateTime = TIME.Date.AddHours(14)
				}
			};
			var documentsEqual = new List<TimeTrackDocument>
			{
				new TimeTrackDocument("First", "Fr", 11, DocumentType.Overtime)
				{
					StartDateTime = TIME.Date.AddHours(14),
					EndDateTime = TIME.Date.AddHours(15)
				},
				new TimeTrackDocument("Second", "Sec", 6, DocumentType.Overtime)
				{
					StartDateTime = TIME.Date.AddHours(13),
					EndDateTime = TIME.Date.AddHours(14)
				}
			};

			var dayTimeTrackOneDocument = new DayTimeTrack
			{
				RealTimeTrackParts = real,
				SlideTime = _slideTime,
				Documents = document,
				Date = TIME.Date
			};
			var dayTimeTrackTwoDocuments = new DayTimeTrack
			{
				RealTimeTrackParts = real,
				SlideTime = _slideTime,
				Documents = documents,
				Date = TIME.Date
			};
			var dayTimeTrackTwoDocumentsEqual = new DayTimeTrack
			{
				RealTimeTrackParts = real,
				SlideTime = _slideTime,
				Documents = documentsEqual,
				Date = TIME.Date
			};

			//Act
			dayTimeTrackOneDocument.Calculate();
			dayTimeTrackTwoDocuments.Calculate();
			dayTimeTrackTwoDocumentsEqual.Calculate();

			//Assert
			Assert.AreEqual(dayTimeTrackOneDocument.LetterCode, "Fr");
			Assert.AreEqual(dayTimeTrackOneDocument.LetterCode, "Fr");
			Assert.AreEqual(dayTimeTrackTwoDocumentsEqual.LetterCode, "Sec");
		}

		[Test]
		public void SetLetterCodeForManualAddedInterval()
		{
			//Arrange
			var realWithAbsence = new List<TimeTrackPart>
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
					ExitDateTime = TIME.Date.AddHours(16),
					IsForURVZone = true,
					IsManuallyAdded = true
				}
			};
			var realAllPresence = new List<TimeTrackPart>
			{
				new TimeTrackPart
				{
					EnterDateTime = TIME.Date.AddHours(9),
					ExitDateTime = TIME.Date.AddHours(17),
					IsForURVZone = true
				},
				new TimeTrackPart
				{
					EnterDateTime = TIME.Date.AddHours(17),
					ExitDateTime = TIME.Date.AddHours(18),
					IsForURVZone = true,
					IsManuallyAdded = true
				}
			};
			var document = new List<TimeTrackDocument>
			{
				new TimeTrackDocument("First", "Fr", 11, DocumentType.Overtime)
				{
					StartDateTime = TIME.Date.AddHours(14),
					EndDateTime = TIME.Date.AddHours(18)
				}
			};

			var dayTimeTrackAllPresence = new DayTimeTrack
			{
				RealTimeTrackParts = realAllPresence,
				PlannedTimeTrackParts = _plannedTimeTrackParts,
				Date = TIME.Date,
				SlideTime = _slideTime
			};
			var dayTimeTrack = new DayTimeTrack
			{
				RealTimeTrackParts = realWithAbsence,
				PlannedTimeTrackParts = _plannedTimeTrackParts,
				Date = TIME.Date,
				SlideTime = _slideTime
			};
			var dayTimeTrackWithDocument = new DayTimeTrack
			{
				RealTimeTrackParts = realWithAbsence,
				PlannedTimeTrackParts = _plannedTimeTrackParts,
				Date = TIME.Date,
				SlideTime = _slideTime,
				Documents = document
			};
			var dayTimeTrackHoliday = new DayTimeTrack
			{
				RealTimeTrackParts = realWithAbsence,
				Date = TIME.Date,
				SlideTime = _slideTime
			};
			var dayTimeTrackHolidayWithDocument = new DayTimeTrack
			{
				RealTimeTrackParts = realWithAbsence,
				Date = TIME.Date,
				SlideTime = _slideTime,
				Documents = document
			};

			//Act
			dayTimeTrack.Calculate();
			dayTimeTrackAllPresence.Calculate();
			dayTimeTrackWithDocument.Calculate();
			dayTimeTrackHoliday.Calculate();
			dayTimeTrackHolidayWithDocument.Calculate();

			//Assert
			Assert.AreEqual(dayTimeTrack.LetterCode, "УР*");
			Assert.AreEqual(dayTimeTrackAllPresence.LetterCode, TimeSpan.FromHours(8).TotalHours.ToString("F") + "*");
			Assert.AreEqual(dayTimeTrackWithDocument.LetterCode, "Fr" + "*");
			Assert.AreEqual(dayTimeTrackHoliday.LetterCode, "В" + "*");
			Assert.AreEqual(dayTimeTrackHolidayWithDocument.LetterCode, "Fr" + "*");
		}
	}
}
