using System;
using System.Data;
using CodeReason.Reports;
using FiresecAPI.Models;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using JournalModule.ViewModels;
using ReportsModule.Models;

namespace ReportsModule.ReportProviders
{
	internal class JournalReport : BaseReport
	{
		private ReportArchiveFilter ReportArchiveFilter { get; set; }

		public JournalReport()
			: base(ReportType.ReportJournal)
		{
			ReportArchiveFilter = new ReportArchiveFilter();
		}

		public override ReportData GetData()
		{
			ReportArchiveFilter.LoadArchive();
			var data = new ReportData();
			data.ReportDocumentValues.Add("PrintDate", DateTime.Now);
			data.ReportDocumentValues.Add("StartDate", ReportArchiveFilter.StartDate);
			data.ReportDocumentValues.Add("EndDate", ReportArchiveFilter.EndDate);

			DataTable table = new DataTable("Journal");
			table.Columns.Add("DeviceTime");
			table.Columns.Add("SystemTime");
			table.Columns.Add("ZoneName");
			table.Columns.Add("Description");
			table.Columns.Add("DeviceName");
			table.Columns.Add("PanelName");
			table.Columns.Add("User");
			foreach (var journalRecord in ReportArchiveFilter.JournalRecords)
				table.Rows.Add(journalRecord.DeviceTime, journalRecord.SystemTime, journalRecord.ZoneName, journalRecord.Description, journalRecord.DeviceName, journalRecord.PanelName, journalRecord.User);
			data.DataTables.Add(table);
			return data;
		}

		public override bool IsFilterable
		{
			get { return true; }
		}
		public override void Filter(RelayCommand refreshCommand)
		{
			var archiveFilterViewModel = new ArchiveFilterViewModel(ReportArchiveFilter.ArchiveFilter);
			if (DialogService.ShowModalWindow(archiveFilterViewModel))
			{
				ReportArchiveFilter = new ReportArchiveFilter(archiveFilterViewModel);
				refreshCommand.Execute();
			}
		}
	}
}
