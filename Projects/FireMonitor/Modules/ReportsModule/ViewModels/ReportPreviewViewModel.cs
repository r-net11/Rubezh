using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Reports;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Printing;
using System.Windows.Xps;
using System.Windows;
using Common;
using CodeReason.Reports;
using System.Windows.Resources;
using Infrastructure.Common;
using System.IO;

namespace ReportsModule.ViewModels
{
	public class ReportPreviewViewModel : SaveCancelDialogViewModel
	{
		private IReportProvider _reportProvider;

		public ReportPreviewViewModel(IReportProvider provider)
		{
			_reportProvider = provider;
			Title = provider.Title;
			SaveCaption = "Печать";
			CancelCaption = "Отмена";
			GenerateReport();
		}
		protected override bool Save()
		{
			return Print();
		}

		public bool Print()
		{
			var printDialog = new PrintDialog();
			if (printDialog.ShowDialog() == true)
			{
				PreparePrinting(printDialog.PrintTicket, DocumentPaginator.PageSize);
				XpsDocumentWriter writer = PrintQueue.CreateXpsDocumentWriter(printDialog.PrintQueue);
				if (writer != null)
					writer.WriteAsync(DocumentPaginator, printDialog.PrintTicket);
				return true;
			}
			return false;
		}

		private DocumentPaginator _documentPaginator;
		public DocumentPaginator DocumentPaginator
		{
			get { return _documentPaginator; }
			private set
			{
				_documentPaginator = value;
				OnPropertyChanged(() => DocumentWidth);
				OnPropertyChanged(() => DocumentHeight);
				OnPropertyChanged(() => DocumentPaginator);
			}
		}
		public double DocumentWidth
		{
			get { return DocumentPaginator == null ? 0 : DocumentPaginator.PageSize.Width; }
		}
		public double DocumentHeight
		{
			get { return DocumentPaginator == null ? 0 : DocumentPaginator.PageSize.Height; }
		}

		private void GenerateReport()
		{
			using (new TimeCounter("Build report: {0}"))
			{
				var reportDocument = new ReportDocument();
				reportDocument.XamlData = GetXaml();
				DocumentPaginator = GetPaginator(reportDocument);
			}
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
		private void PreparePrinting(PrintTicket printTicket, Size pageSize)
		{
			printTicket.PageOrientation = pageSize.Height >= pageSize.Width ? PageOrientation.Portrait : PageOrientation.Landscape;
			var printExtension = _reportProvider as IReportPrintExtension;
			if (printExtension != null)
				printExtension.PreparePrinting(printTicket, pageSize);
		}
	}
}
