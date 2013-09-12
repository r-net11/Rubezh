using System.Data;
using CodeReason.Reports;
using FiresecAPI;
using GKModule.ViewModels;
using Infrastructure.Common;
using Infrastructure.Common.Reports;
using Infrastructure.Common.Windows;
using Controls.Converters;
using iTextSharp.text.pdf;
using Common.PDF;
using iTextSharp.text;

namespace GKModule.Reports
{
	internal class JournalReport : ISingleReportProvider, IFilterableReport
	{
		private DataTable _table;

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

			_table = new DataTable("Journal");
			_table.Columns.Add("SystemDateTime");
			_table.Columns.Add("DeviceDateTime");
			_table.Columns.Add("Name");
			_table.Columns.Add("YesNo");
			_table.Columns.Add("Description");
			_table.Columns.Add("DeviceName");
			_table.Columns.Add("ZoneName");
			_table.Columns.Add("DirectionName");
			_table.Columns.Add("StateClass");
			foreach (var journalItem in ReportArchiveFilter.JournalItems)
			{
				var journalItemViewModel = new JournalItemViewModel(journalItem);
				var deviceName = "";
				if (journalItemViewModel.DeviceState != null)
				{
					deviceName = journalItemViewModel.DeviceState.Device.PresentationAddressAndDriver;
				}
				var zoneName = "";
				if (journalItemViewModel.ZoneState != null)
				{
					zoneName = journalItemViewModel.ZoneState.Zone.PresentationName;
				}
				var directionName = "";
				if (journalItemViewModel.DirectionState != null)
				{
					directionName = journalItemViewModel.DirectionState.Direction.PresentationName;
				}
				_table.Rows.Add(
					journalItem.SystemDateTime,
					journalItem.DeviceDateTime,
					journalItem.Name,
					journalItem.YesNo.ToDescription(),
					journalItem.Description,
					deviceName,
					zoneName,
					directionName,
					journalItem.StateClass.ToDescription());
			}
			data.DataTables.Add(_table);
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

		public bool CanPdfPrint
		{
			get { return true; }
		}
		public void PdfPrint(iTextSharp.text.Document document)
		{
			var table = PDFHelper.CreateTable(document, 8);
			table.HeaderRows = 2;
			table.SetWidths(new float[] { 1f, 1f, 0.5f, 2f, 1f, 1.5f, 1.5f, 1f, });
			var cell = PDFHelper.GetCell(string.Format("Журнал событий с {0:dd.MM.yyyy HH:mm:ss} по {1:dd.MM.yyyy HH:mm:ss}", ReportArchiveFilter.StartDate, ReportArchiveFilter.EndDate), PDFStyle.HeaderFont, PDFStyle.HeaderBackground);
			cell.Colspan = 8;
			cell.HorizontalAlignment = Element.ALIGN_CENTER;
			table.AddCell(cell);
			var headers = new string[] 
			{
				"Дата",
				"Название",
				"Д/Н",
				"Описание",
				"Устройство",
				"Зона",
				"Направление",
				"Состояние",
			};
			foreach (var heder in headers)
			{
				cell = PDFHelper.GetCell(heder, PDFStyle.TextFont, PDFStyle.HeaderBackground);
				cell.HorizontalAlignment = Element.ALIGN_CENTER;
				cell.VerticalAlignment = Element.ALIGN_MIDDLE;
				table.AddCell(cell);
			};

			PDFHelper.PrintTable(table, _table);
			document.Add(table);
		}
		#endregion
	}
}