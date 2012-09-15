using System;
using System.Diagnostics;
using System.IO;
using System.Printing;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Resources;
using CodeReason.Reports;
using Infrastructure.Common;
using Infrastructure.Common.Reports;
using Infrastructure.Common.Windows.ViewModels;

namespace ReportsModule.ViewModels
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

		public DocumentPaginator GenerateReport()
		{
			DateTime dt = DateTime.Now;
			ReportDocument reportDocument = new ReportDocument();
			reportDocument.XamlData = GetXaml();
			DocumentPaginator documentPaginator = GetPaginator(reportDocument);
			Debug.WriteLine("Build report: {0}", DateTime.Now - dt);
			return documentPaginator;
		}

		public void Filter(RelayCommand refreshCommand)
		{
			((IFilterableReport)_reportProvider).Filter(refreshCommand);
		}
		public void PreparePrinting(PrintTicket printTicket, Size pageSize)
		{
			printTicket.PageOrientation = pageSize.Height >= pageSize.Width ? PageOrientation.Portrait : PageOrientation.Landscape;
			var printExtension = _reportProvider as IReportPrintExtension;
			if (printExtension != null)
				printExtension.PreparePrinting(printTicket, pageSize);
		}

		private DocumentPaginator GetPaginator(ReportDocument reportDocument)
		{
			var singleReport = _reportProvider as ISingleReportProvider;
			if (singleReport != null)
				return new ReportPaginator(reportDocument, singleReport.GetData());
			var multiReport = _reportProvider as IMultiReportProvider;
			if (multiReport != null)
				return new MultipleReportPaginator(reportDocument, multiReport.GetData());
			return null;
		}
		private string GetXaml()
		{
			StreamResourceInfo info = Application.GetResourceStream(ResourceHelper.ComposeResourceUri(_reportProvider.GetType().Assembly, _reportProvider.Template));
			using (var reader = new StreamReader(info.Stream))
				return reader.ReadToEnd();
		}
	}
}
