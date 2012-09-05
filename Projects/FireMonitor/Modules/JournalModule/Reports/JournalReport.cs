using System.Data;
using CodeReason.Reports;
using FiresecAPI.Models;
using Infrastructure.Common;
using Infrastructure.Common.Reports;
using Infrastructure.Common.Windows;
using JournalModule.ViewModels;

namespace JournalModule.Reports
{
	internal class JournalReport : ISingleReportProvider, IFilterableReport
	{
		private ReportArchiveFilter ReportArchiveFilter { get; set; }
		public JournalReport()
		{
			ReportArchiveFilter = new ReportArchiveFilter();
		}

		#region IFilterableReport Members

		public void Filter(RelayCommand refreshCommand)
		{
			var archiveFilterViewModel = new ArchiveFilterViewModel(ReportArchiveFilter.ArchiveFilter);
			if (DialogService.ShowModalWindow(archiveFilterViewModel))
			{
				ReportArchiveFilter = new ReportArchiveFilter(archiveFilterViewModel);
				refreshCommand.Execute();
			}
		}

		#endregion

		#region ISingleReportProvider Members

		public ReportData GetData()
		{
			ReportArchiveFilter.LoadArchive();
			var data = new ReportData();
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

		#endregion

		#region IReportProvider Members

		public string Template
		{
			get { return "Reports/JournalReport.xaml"; }
		}

		public string Title
		{
			get { return "Журнал событий"; }
		}

		public bool IsEnabled
		{
			get { return true; }
		}

		#endregion
	}
}
