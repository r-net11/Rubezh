using System.Data;
using CodeReason.Reports;
using JournalModule.ViewModels;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Reports;
using Infrastructure.Common.Windows.Windows;
using RubezhAPI.Journal;
using System;

namespace JournalModule.Reports
{
	internal class JournalReport : ISingleReportProvider, IFilterableReport
	{
		private ArchiveViewModel ArchiveViewModel { get; set; }
		public JournalReport()
		{
			PdfProvider = new JournalReportPdf();
			ArchiveViewModel = new ArchiveViewModel();
		}

		#region IFilterableReport Members
		public void Filter(RelayCommand refreshCommand)
		{
			ArchiveViewModel = new ArchiveViewModel();
			ArchiveViewModel.ShowFilterCommand.Execute();
			refreshCommand.Execute();
		}
		#endregion

		#region ISingleReportProvider Members
		public ReportData GetData()
		{
			var data = new ReportData();
			if (ArchiveViewModel.Filter != null)
			{
				data.ReportDocumentValues.Add("StartDate", ArchiveViewModel.Filter.StartDate);
				data.ReportDocumentValues.Add("EndDate", ArchiveViewModel.Filter.EndDate);
			}
			else
			{
				data.ReportDocumentValues.Add("StartDate", DateTime.Now);
				data.ReportDocumentValues.Add("EndDate", DateTime.Now);
			}
			var table = new DataTable("Journal");
			table.Columns.Add("DeviceDateTime");
			table.Columns.Add("SystemDateTime");
			table.Columns.Add("Name");
			table.Columns.Add("Description");
			table.Columns.Add("ObjectName");

			//if (ArchiveViewModel.Pages != null)
			//{
			//	foreach (var page in ArchiveViewModel.Pages)
			//	{
			//		page.Create();
			//		foreach (var journalItem in page.JournalItems)
			//		{
			//			table.Rows.Add(
			//				journalItem.JournalItem.DeviceDateTime,
			//				journalItem.JournalItem.SystemDateTime,
			//				journalItem.Name,
			//				journalItem.Description,
			//				journalItem.ObjectName);
			//		}
			//	}
			//}
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
			get { return "Журнал событий"; }
		}

		public bool IsEnabled
		{
			get { return true; }
		}

		public IReportPdfProvider PdfProvider { get; private set; }

		#endregion
	}
}