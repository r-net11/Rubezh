using System;
using System.Windows.Controls;
using System.Linq;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using ReportsModule2.DocumentPaginatorModel;
using ReportsModule2.Reports;
using System.Windows.Markup;
using System.Windows.Documents;
using Controls;
using System.Windows;
using MS.Internal.Documents;
using FiresecAPI.Models;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Windows.Xps.Packaging;
using System.Windows.Xps.Serialization;
using System.IO;
using System.Windows.Forms.Integration;
using Microsoft.Reporting.WinForms;
using System.Windows.Forms;
using ReportsModule2.Views;

namespace ReportsModule2.ViewModels
{
	public class Reports2ViewModel : ViewPartViewModel
	{
		public Reports2ViewModel()
		{
			XpsDocumentCommand = new RelayCommand(OnXpsDocument);
			WindowsFormsHost = new System.Windows.Forms.Integration.WindowsFormsHost();
			ReportViewer = new Microsoft.Reporting.WinForms.ReportViewer();
			WindowsFormsHost.Child = ReportViewer;
			OnPropertyChanged("WindowsFormsHost");
			//XpsDocumentViewer = new DocumentViewer();
			//XpsViewer = XpsDocumentViewer;
			_reportMap = new Dictionary<ReportType, BaseReport>()
		    {
		        {ReportType.ReportIndicationBlock, new ReportIndicationBlock()},
		        {ReportType.ReportJournal, new ReportJournal()},
		        {ReportType.ReportDriverCounter, new ReportDriverCounter()},
		        {ReportType.ReportDeviceParams, new ReportDeviceParams()},
		        {ReportType.ReportDevicesList, new ReportDevicesList()}
		    };
		}

		Dictionary<ReportType, BaseReport> _reportMap;

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

		ReportViewer _reportViewer;
		public ReportViewer ReportViewer
		{
			get { return _reportViewer; }
			set
			{
				_reportViewer = value;
				_reportViewer.RefreshReport();
				OnPropertyChanged("ReportViewer");
			}
		}

		WindowsFormsHost _windowsFormsHost;
		public WindowsFormsHost WindowsFormsHost
		{
			get { return _windowsFormsHost; }
			set
			{
				_windowsFormsHost = value;
				OnPropertyChanged("WindowsFormsHost");
			}
		}

		public List<ReportType> AvailableReportTypes
		{
			get { return Enum.GetValues(typeof(ReportType)).Cast<ReportType>().ToList(); }
		}

		ReportType? _selectedReportName;
		public ReportType? SelectedReportName
		{
			get { return _selectedReportName; }
			set
			{
				_selectedReportName = value;
				if (value.HasValue)
					ShowReport(_reportMap[value.Value]);
			}
		}

		void ShowReport(BaseReport baseReport)
		{
			baseReport.LoadData();
			WindowsFormsHost winFormsHost = new WindowsFormsHost();
			ReportViewer reportViewer = new ReportViewer();
			reportViewer.ProcessingMode = ProcessingMode.Local;
			reportViewer.LocalReport.ReportPath = @"DeviceListRdlc.rdlc";
			reportViewer.LocalReport.DataSources.Add(baseReport.CreateDataSource());
			reportViewer.RefreshReport();
			winFormsHost.Child = reportViewer;
			Window window = new Window();
			window.Content = winFormsHost;
			window.AllowsTransparency = false;
			window.Show();
			//XpsViewer = winFormsHost;
			//Form1 newForm = new Form1();
			//newForm.ReportViewer.ProcessingMode = ProcessingMode.Local;
			//newForm.ReportViewer.LocalReport.ReportPath = @"DeviceListRdlc.rdlc";
			//newForm.ReportViewer.LocalReport.DataSources.Add(baseReport.CreateDataSource());
			//newForm.ReportViewer.RefreshReport();
			//newForm.Show();
			
			//baseReport.CreateFlowDocumentStringBuilder();
			//SaveAsXps(baseReport.FlowDocumentStringBuilder.ToString(), baseReport.XpsDocumentName);
			//XpsDocumentViewer.Document = baseReport.XpsDocument.GetFixedDocumentSequence();
			//OnPropertyChanged("XpsDocumentViewer");
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
			var scrollViewer = VisualTreeFinder.FindVisualChild<ScrollViewer>(XpsDocumentViewer);
			var endDate = DateTime.Now;
			var Time = (endDate - startDate).ToString();
			OnPropertyChanged("XpsDocumentViewer");
			//var fd = (FlowDocument)XamlReader.Parse(sb.ToString());
			//var flowDocumentReader = new FlowDocumentReader();
			//flowDocumentReader.Document = fd;
			//XpsViewer = flowDocumentReader;
			//OnPropertyChanged("XpsViewer");

			//FileStream htmlFile = new FileStream("journal.html", FileMode.Open, FileAccess.Read);
			//StreamReader myStreamReader = new StreamReader(htmlFile, Encoding.GetEncoding(1251));
			//string xamlflowDocument = HtmlToXamlConverter.ConvertHtmlToXaml(myStreamReader.ReadToEnd(), true);
			//ConvertFlowToXPS.SaveAsXps2(xamlflowDocument);
			//XpsDocument xpsDocument = new XpsDocument("test.xps", FileAccess.Read);
			//xpsDocument.Close();
			//myStreamReader.Close();
			//htmlFile.Close();
		}

		void SaveAsXps(string xamlFlowDoc, string xpsDocumentName)
		{
			object doc;
			doc = XamlReader.Parse(xamlFlowDoc);

			using (Package container = Package.Open(xpsDocumentName, FileMode.Create))
			{
				using (XpsDocument xpsDoc = new XpsDocument(container, CompressionOption.NotCompressed))
				{
					XpsSerializationManager rsm = new XpsSerializationManager(new XpsPackagingPolicy(xpsDoc), false);
					DocumentPaginator paginator = ((IDocumentPaginatorSource)doc).DocumentPaginator;
					paginator = new ReportPaginator(paginator);
					rsm.SaveAsXaml(paginator);
				}
			}
		}
	}
}