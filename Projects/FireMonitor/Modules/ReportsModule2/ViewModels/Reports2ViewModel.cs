using System;
using System.Windows.Controls;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using ReportsModule2.DocumentPaginatorModel;
using ReportsModule2.Reports;

namespace ReportsModule2.ViewModels
{
	public class Reports2ViewModel : ViewPartViewModel
	{
		public Reports2ViewModel()
		{
			XpsDocumentCommand = new RelayCommand(OnXpsDocument);
			XpsDocumentViewer = new DocumentViewer();
			XpsViewer = XpsDocumentViewer;
		}

		object _xpsViewer;
		public object XpsViewer
		{
			get { return _xpsViewer; }
			set
			{
				_xpsViewer = value;
				OnPropertyChanged("XpsViewer");
			}
		}

		DocumentViewer _xpsDocumentViewer;
		public DocumentViewer XpsDocumentViewer
		{
			get { return _xpsDocumentViewer; }
			set
			{
				_xpsDocumentViewer = value;
				OnPropertyChanged("XpsDocumentViewer");
			}
		}

		public void ShowReport()
		{
		}

		public RelayCommand XpsDocumentCommand { get; private set; }
		void OnXpsDocument()
		{
			var startDate = DateTime.Now;
			var reportDevicesList = new ReportDevicesList();
			reportDevicesList.LoadData();
			reportDevicesList.CreateFlowDocumentStringBuilder();
			var sb = reportDevicesList.FlowDocumentStringBuilder;
			ConvertFlowToXPS.SaveAsXps2(sb.ToString(), reportDevicesList.XpsDocumentName);
			XpsDocumentViewer.Document = reportDevicesList.XpsDocument.GetFixedDocumentSequence();
			var endDate = DateTime.Now;
			var Time = (endDate - startDate).ToString();
			OnPropertyChanged("XpsDocumentViewer");

			//FileStream htmlFile = new FileStream("journal.html", FileMode.Open, FileAccess.Read);
			//StreamReader myStreamReader = new StreamReader(htmlFile, Encoding.GetEncoding(1251));
			//string xamlflowDocument = HtmlToXamlConverter.ConvertHtmlToXaml(myStreamReader.ReadToEnd(), true);
			//ConvertFlowToXPS.SaveAsXps2(xamlflowDocument);
			//XpsDocument xpsDocument = new XpsDocument("test.xps", FileAccess.Read);
			//xpsDocument.Close();
			//myStreamReader.Close();
			//htmlFile.Close();
		}
	}
}