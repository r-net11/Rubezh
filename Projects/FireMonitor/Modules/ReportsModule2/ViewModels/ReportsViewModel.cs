using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Common;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using JournalModule.ViewModels;
using ReportsModule2.Reports;
using System.Windows;
using System.Windows.Documents;
using System.IO;
using System.Text;
using System.Windows.Controls;
using ReportsModule2.DocumentPaginatorModel;
using System.Windows.Xps.Packaging;

namespace ReportsModule2.ViewModels
{
	public class ReportsViewModel : ViewPartViewModel
	{
		public ReportsViewModel()
		{
			FirstPageCommand = new RelayCommand(OnFirstPage, GetIsReportLoad);
			NextPageCommand = new RelayCommand(OnNextPage, GetIsReportLoad);
			PreviousPageCommand = new RelayCommand(OnPreviousPage, GetIsReportLoad);
			LastPageCommand = new RelayCommand(OnLastPage, GetIsReportLoad);
			FidthToPageCommand = new RelayCommand(OnFidthToPage, GetIsReportLoad);
			SaveReportCommand = new RelayCommand(OnSaveReportCommand, GetIsReportLoad);
			PrintReportCommand = new RelayCommand(OnPrintReport, GetIsReportLoad);
			RefreshCommand = new RelayCommand(OnRefresh, GetIsReportLoad);
			FilterCommand = new RelayCommand(OnFilter, GetIsReportLoad);
			SearchCommand = new RelayCommand(OnSearch, GetIsReportLoad);
			ZoomInCommand = new RelayCommand(OnZoomIn, GetIsReportLoad);
			ZoomOutCommand = new RelayCommand(OnZoomOut, GetIsReportLoad);
			XpsDocumentCommand = new RelayCommand(OnXpsDocument);


			XpsDocumentViewer = new DocumentViewer();
		}

		private Dictionary<ReportType, BaseReport> _reportMap;

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

		void ShowCrystalReport(BaseReport baseReport)
		{
		}

		public int ZoomValue { get; set; }

		public double ZoomMinimumValue { get { return 1; } }
		public double ZoomMaximumValue { get { return 1000; } }
		public bool IsReportLoad { get { return GetIsReportLoad(); } }
		public bool IsJournalReport { get { return SelectedReportName == ReportType.ReportJournal; } }

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
					ShowCrystalReport(_reportMap[value.Value]);
				OnPropertyChanged("IsReportLoad");
				OnPropertyChanged("IsJournalReport");
				OnPropertyChanged("SelectedReportName");
			}
		}

		public string TotalPageNumber { get; set; }

		public string CurrentPageNumber { get; set; }

		string _searchText;
		public string SearchText
		{
			get { return _searchText; }
			set
			{
				_searchText = value;
				OnPropertyChanged("SearchText");
			}
		}

		public RelayCommand ZoomOutCommand { get; private set; }
		void OnZoomOut()
		{
			Update();
		}
		public RelayCommand ZoomInCommand { get; private set; }
		void OnZoomIn()
		{
			Update();
		}
		public RelayCommand FirstPageCommand { get; private set; }
		void OnFirstPage()
		{
			Update();
		}
		public RelayCommand NextPageCommand { get; private set; }
		void OnNextPage()
		{
			Update();
		}
		public RelayCommand PreviousPageCommand { get; private set; }
		void OnPreviousPage()
		{
			Update();
		}
		public RelayCommand LastPageCommand { get; private set; }
		void OnLastPage()
		{
			Update();
		}
		public RelayCommand FidthToPageCommand { get; private set; }
		void OnFidthToPage()
		{
			Update();
		}
		public RelayCommand SaveReportCommand { get; private set; }
		void OnSaveReportCommand()
		{
		}
		public RelayCommand PrintReportCommand { get; private set; }
		void OnPrintReport()
		{
		}
		public RelayCommand RefreshCommand { get; private set; }
		void OnRefresh()
		{
			SelectedReportName = SelectedReportName;
			Update();
		}
		public RelayCommand FilterCommand { get; private set; }
		void OnFilter()
		{
			var archiveFilter = new ArchiveFilter()
			{
				EndDate = DateTime.Now,
				StartDate = DateTime.Now.AddDays(-1),
				UseSystemDate = false
			};
			var archiveFilterViewModel = new ArchiveFilterViewModel(archiveFilter);
			if (DialogService.ShowModalWindow(archiveFilterViewModel))
			{
				ShowCrystalReport(new ReportJournal(archiveFilterViewModel));
			}
		}
		public RelayCommand SearchCommand { get; private set; }
		void OnSearch() { }

		string _time;
		public string Time
		{
			get { return _time; }
			set
			{
				_time = value;
				OnPropertyChanged("Time");
			}
		}

		public RelayCommand XpsDocumentCommand { get; private set; }
		void OnXpsDocument()
		{
			var startDate = DateTime.Now;
			var reportJournal = new ReportJournal();
			reportJournal.LoadData();
			reportJournal.CreateFlowDocumentStringBuilder();
			var sb = reportJournal.FlowDocumentStringBuilder;
			ConvertFlowToXPS.SaveAsXps2(sb.ToString());
			XpsDocumentViewer.Document = reportJournal.XpsDocument.GetFixedDocumentSequence();
			var endDate = DateTime.Now;
			Time = (endDate - startDate).ToString();
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

		public void Update()
		{
			OnPropertyChanged("ZoomValue");
			OnPropertyChanged("CurrentPageNumber");
			OnPropertyChanged("TotalPageNumber");
		}

		bool GetIsReportLoad()
		{
			return SelectedReportName != null;
		}
	}
}