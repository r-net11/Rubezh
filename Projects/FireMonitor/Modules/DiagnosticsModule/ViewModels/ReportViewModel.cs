using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Reports;
using Infrastructure.Common;
using System.IO;
using Common.PDF;

namespace DiagnosticsModule.ViewModels
{
	public class ReportViewModel : BaseViewModel
	{
		private readonly IReportProvider _reportProvider;

		public ReportViewModel(IReportProvider reportProvider)
		{
			_reportProvider = reportProvider;
			IsActive = false;
		}

		public string Title
		{
			get { return _reportProvider.Title; }
		}
		public bool IsFilterable
		{
			get { return _reportProvider is IFilterableReport; }
		}
		public bool IsEnabled
		{
			get { return _reportProvider.IsEnabled; }
		}
		public bool IsActive { get; set; }

		public void Filter(RelayCommand refreshCommand)
		{
			((IFilterableReport)_reportProvider).Filter(refreshCommand);
		}
		public void BuildReport(string fileName)
		{
			var singleReport = _reportProvider as ISingleReportProvider;
			if (singleReport != null)
			{
				_reportProvider.PdfProvider.ReportData = singleReport.GetData();
				using (var fs = new FileStream(fileName, FileMode.Create))
				using (var pdf = new PDFDocument(fs, _reportProvider.PdfProvider.PageFormat))
				{
					AddReportHeader(pdf.Document);
					_reportProvider.PdfProvider.Print(pdf.Document);
				}
			}
		}
		private void AddReportHeader(iTextSharp.text.Document document)
		{
			document.AddAuthor("Рубеж / Оперативные задачи");
			document.AddTitle(_reportProvider.Title);
			document.AddCreator("ОЗ");
		}
	}
}
