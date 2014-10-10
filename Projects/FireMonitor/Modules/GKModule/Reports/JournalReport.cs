using System.Data;
using CodeReason.Reports;
using GKModule.ViewModels;
using Infrastructure.Common;
using Infrastructure.Common.Reports;
using Infrastructure.Common.Windows;

namespace GKModule.Reports
{
	internal class JournalReport : ISingleReportProvider, IFilterableReport
	{
		private ReportArchiveFilter ReportArchiveFilter { get; set; }
		public JournalReport()
		{
			PdfProvider = new JournalReportPdf();
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

			var table = new DataTable("Journal");
			table.Columns.Add("DeviceDateTime");
			table.Columns.Add("SystemDateTime");
			table.Columns.Add("Name");
			table.Columns.Add("Description");
			table.Columns.Add("ObjectName");
			foreach (var journalItem in ReportArchiveFilter.JournalItems)
			{
				var journalItemViewModel = new JournalItemViewModel(journalItem);
				var objectName = "";
				if (journalItemViewModel.Device != null)
				{
					objectName = journalItemViewModel.Device.PresentationName;
				}
				if (journalItemViewModel.Zone != null)
				{
					objectName = journalItemViewModel.Zone.PresentationName;
				}
				if (journalItemViewModel.Direction != null)
				{
					objectName = journalItemViewModel.Direction.PresentationName;
				}
				table.Rows.Add(
					journalItem.DeviceDateTime,
					journalItem.SystemDateTime,
					journalItem.Name,
					journalItem.Description,
					objectName);
			}
			data.DataTables.Add(table);
			PdfProvider.ReportData = data;
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
			get { return "Журнал событий ГК"; }
		}

		public bool IsEnabled
		{
			get { return true; }
		}

		public IReportPdfProvider PdfProvider { get; private set; }

		#endregion
	}
}