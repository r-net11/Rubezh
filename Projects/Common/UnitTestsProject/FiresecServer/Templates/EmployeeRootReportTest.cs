using FiresecService.Report.Model;
using FiresecService.Report.Templates;
using NUnit.Framework;
using StrazhDAL.DataAccess;
using System;
using System.Collections.Generic;

namespace UnitTestsProject.FiresecServer.Templates
{
	[TestFixture]
	public class EmployeeRootReportTest
	{
		private List<PassJournal> passJournals;
		private EmployeeInfo employee;
		private Guid employeeGuid;

		[SetUp]
		public void SetUp()
		{
			employeeGuid = new Guid();
			var datetimeNow = DateTime.Now;
			passJournals = InitPassJournals(datetimeNow);

			employee = new EmployeeInfo
			{
				UID = employeeGuid,
			};
		}

		private List<PassJournal> InitPassJournals(DateTime datetimeNow)
		{
			return new List<PassJournal>
			{
				new PassJournal
				{
					UID = new Guid(),
					EmployeeUID = employeeGuid,
					ZoneUID = new Guid(),
					EnterTime = datetimeNow,
					ExitTime = null
				},
				new PassJournal
				{
					UID = new Guid(),
					EmployeeUID = employeeGuid,
					ZoneUID = new Guid(),
					EnterTime = datetimeNow,
					ExitTime = datetimeNow.AddHours(2)
				},
				new PassJournal
				{
					UID = new Guid(),
					EmployeeUID = employeeGuid,
					ZoneUID = new Guid(),
					EnterTime = datetimeNow.AddHours(2),
					ExitTime = datetimeNow
				}
			};
		}

		[Test]
		public void GetTimeTrackParts_Result_Count()
		{
			var c = new EmployeeRootReport().GetTimeTrackParts(new EmployeeRootReport().GetDayPassJournals(passJournals, employee));
			Assert.AreEqual(c.Count, passJournals.Count);
		}
	}
}