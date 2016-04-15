using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.SKD;
using NUnit.Framework;

namespace FiresecMonitorUnitTests.FireMonitorTests
{
	[TestFixture]
	class GetDocumentTimeTrackType
	{
		private readonly DateTime TIME = DateTime.Now;
		private List<TimeTrackPart> _plannedTimeTrackParts;
		private TimeSpan _slideTime;

		[SetUp]
		public void GetDocumentTimeTrackTypeSetUp()
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
		public void GetDocumentTimeTrackTypeTearDown()
		{
			_slideTime = default(TimeSpan);
			_plannedTimeTrackParts = null;
		}

		[Test]
		public void GetOvertimeDocument()
		{
			var dayTimeTrack = new DayTimeTrack
			{
				PlannedTimeTrackParts = _plannedTimeTrackParts,
				Documents = new List<TimeTrackDocument>
				{
					new TimeTrackDocument("OverTimeDocument", "11", 12, DocumentType.Overtime)
					{
						StartDateTime = TIME.Date,
						EndDateTime = TIME.Date.AddHours(23),
						IsOutside = true
					}
				},
				Date = TIME.Date
			};

			dayTimeTrack.Calculate();

			var overtime =
				dayTimeTrack.CombinedTimeTrackParts.Where(x => x.TimeTrackPartType == TimeTrackType.DocumentOvertime)
					.Select(x => x.Delta)
					.Aggregate(default(TimeSpan), (span, timeSpan) => span + timeSpan);

			Assert.AreEqual(overtime, TimeSpan.FromHours(23));
		}

		[Test]
		public void GetPresenceDocumentInWorkingDay()
		{
			var dayTimeTrack = new DayTimeTrack
			{
				PlannedTimeTrackParts = _plannedTimeTrackParts,
				Documents = new List<TimeTrackDocument>
				{
					new TimeTrackDocument("PresenceDocument", "11", 12, DocumentType.Presence)
					{
						StartDateTime = TIME.Date,
						EndDateTime = TIME.Date.AddHours(23)
					}
				},
				Date = TIME.Date
			};

			dayTimeTrack.Calculate();

			var presence =
				dayTimeTrack.CombinedTimeTrackParts.Where(x => x.TimeTrackPartType == TimeTrackType.DocumentPresence)
					.Select(x => x.Delta)
					.Aggregate(default(TimeSpan), (span, timeSpan) => span + timeSpan);

			Assert.AreEqual(presence, TimeSpan.FromHours(9)); //Документ присутствие действует только в рамках рабочего графика
		}

		[Test]
		public void GetPresenceDocumentInDayOff()
		{
			var dayTimeTrack = new DayTimeTrack
			{
				Documents = new List<TimeTrackDocument>
				{
					new TimeTrackDocument("PresenceDocument", "11", 12, DocumentType.Presence)
					{
						StartDateTime = TIME.Date,
						EndDateTime = TIME.Date.AddHours(23)
					}
				},
				Date = TIME.Date
			};

			dayTimeTrack.Calculate();

			var presence =
				dayTimeTrack.CombinedTimeTrackParts.Where(x => x.TimeTrackPartType == TimeTrackType.DocumentPresence)
					.Select(x => x.Delta)
					.Aggregate(default(TimeSpan), (span, timeSpan) => span + timeSpan);

			Assert.AreEqual(presence, TimeSpan.Zero); //Документ присутствие не действует в выходные дни
		}

		[Test]
		public void GetAbsenceDocumentOutside()
		{
			var dayTimeTrack = new DayTimeTrack
			{
				PlannedTimeTrackParts = _plannedTimeTrackParts,
				Documents = new List<TimeTrackDocument>
				{
					new TimeTrackDocument("AbsenceDocument", "11", 12, DocumentType.Absence)
					{
						StartDateTime = TIME.Date,
						EndDateTime = TIME.Date.AddHours(23),
						IsOutside = true
					}
				},
				Date = TIME.Date
			};

			dayTimeTrack.Calculate();

			var absence =
				dayTimeTrack.CombinedTimeTrackParts.Where(x => x.TimeTrackPartType == TimeTrackType.DocumentAbsence)
					.Select(x => x.Delta)
					.Aggregate(default(TimeSpan), (span, timeSpan) => span + timeSpan);

			Assert.AreEqual(absence, TimeSpan.FromHours(23)); //Документ отсутствия действует за рамками графика, при выставленно флаге IsOutside
		}

		[Test]
		public void GetAbsenceDocumentNotOutside()
		{
			var dayTimeTrack = new DayTimeTrack
			{
				PlannedTimeTrackParts = _plannedTimeTrackParts,
				Documents = new List<TimeTrackDocument>
				{
					new TimeTrackDocument("AbsenceDocument", "11", 12, DocumentType.Absence)
					{
						StartDateTime = TIME.Date,
						EndDateTime = TIME.Date.AddHours(23)
					}
				},
				Date = TIME.Date
			};

			dayTimeTrack.Calculate();

			var absence =
				dayTimeTrack.CombinedTimeTrackParts.Where(x => x.TimeTrackPartType == TimeTrackType.DocumentAbsence)
					.Select(x => x.Delta)
					.Aggregate(default(TimeSpan), (span, timeSpan) => span + timeSpan);

			Assert.AreEqual(absence, TimeSpan.FromHours(9)); //Документ присутствие действует только в рамках рабочего графика
		}

		[Test]
		public void GetAbsenceDocumentInDayOff()
		{
			var dayTimeTrack = new DayTimeTrack
			{
				Documents = new List<TimeTrackDocument>
				{
					new TimeTrackDocument("AbsenceDocument", "11", 12, DocumentType.Absence)
					{
						StartDateTime = TIME.Date,
						EndDateTime = TIME.Date.AddHours(23)
					}
				},
				Date = TIME.Date
			};

			dayTimeTrack.Calculate();

			var absence =
				dayTimeTrack.CombinedTimeTrackParts.Where(x => x.TimeTrackPartType == TimeTrackType.DocumentAbsence)
					.Select(x => x.Delta)
					.Aggregate(default(TimeSpan), (span, timeSpan) => span + timeSpan);

			Assert.AreEqual(absence, TimeSpan.Zero); //Документ присутствие не действует в выходные дни
		}

		[Test]
		public void GetAbsenceReasonableDocumentOutside()
		{
			var dayTimeTrack = new DayTimeTrack
			{
				PlannedTimeTrackParts = _plannedTimeTrackParts,
				Documents = new List<TimeTrackDocument>
				{
					new TimeTrackDocument("AbsenceReasonableDocument", "11", 12, DocumentType.AbsenceReasonable)
					{
						StartDateTime = TIME.Date,
						EndDateTime = TIME.Date.AddHours(23),
						IsOutside = true
					}
				},
				Date = TIME.Date
			};

			dayTimeTrack.Calculate();

			var absenceReasonable =
				dayTimeTrack.CombinedTimeTrackParts.Where(x => x.TimeTrackPartType == TimeTrackType.DocumentAbsenceReasonable)
					.Select(x => x.Delta)
					.Aggregate(default(TimeSpan), (span, timeSpan) => span + timeSpan);

			Assert.AreEqual(absenceReasonable, TimeSpan.FromHours(23)); //Документ отсутствия по уважительной причине действует за рамками графика, при выставленно флаге IsOutside
		}

		[Test]
		public void GetAbsenceReasonableDocumentNotOutside()
		{
			var dayTimeTrack = new DayTimeTrack
			{
				PlannedTimeTrackParts = _plannedTimeTrackParts,
				Documents = new List<TimeTrackDocument>
				{
					new TimeTrackDocument("AbsenceReasonableDocument", "11", 12, DocumentType.AbsenceReasonable)
					{
						StartDateTime = TIME.Date,
						EndDateTime = TIME.Date.AddHours(23)
					}
				},
				Date = TIME.Date
			};

			dayTimeTrack.Calculate();

			var absenceReasonable =
				dayTimeTrack.CombinedTimeTrackParts.Where(x => x.TimeTrackPartType == TimeTrackType.DocumentAbsenceReasonable)
					.Select(x => x.Delta)
					.Aggregate(default(TimeSpan), (span, timeSpan) => span + timeSpan);

			Assert.AreEqual(absenceReasonable, TimeSpan.FromHours(9)); //Документ отсутствия по уважительной причине действует только в рамках графика, при отключенном флаге IsOutside
		}

		[Test]
		public void GetAbsenceReasonableDocumentInDayOff()
		{
			var dayTimeTrack = new DayTimeTrack
			{
				Documents = new List<TimeTrackDocument>
				{
					new TimeTrackDocument("AbsenceReasonableDocument", "11", 12, DocumentType.AbsenceReasonable)
					{
						StartDateTime = TIME.Date,
						EndDateTime = TIME.Date.AddHours(23)
					}
				},
				Date = TIME.Date
			};

			dayTimeTrack.Calculate();

			var absenceReasonable =
				dayTimeTrack.CombinedTimeTrackParts.Where(x => x.TimeTrackPartType == TimeTrackType.DocumentAbsenceReasonable)
					.Select(x => x.Delta)
					.Aggregate(default(TimeSpan), (span, timeSpan) => span + timeSpan);

			Assert.AreEqual(absenceReasonable, TimeSpan.Zero); //Документ отсутствия по уважительной причин не действует в выходные дни
		}
	}
}
