using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using FiresecAPI.Models;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using JournalModule.ViewModels;
using Microsoft.Reporting.WinForms;
using ReportsModule.Reports;

namespace ReportsModule.Views
{
	public partial class ReportControlView : Window, INotifyPropertyChanged
	{
		public ReportControlView()
		{
			InitializeComponent();
			DataContext = this;

			FirstPageCommand = new RelayCommand(OnFirstPage, GetIsReportLoad);
			NextPageCommand = new RelayCommand(OnNextPage, GetIsReportLoad);
			PreviousPageCommand = new RelayCommand(OnPreviousPage, GetIsReportLoad);
			LastPageCommand = new RelayCommand(OnLastPage, GetIsReportLoad);
			FidthToPageCommand = new RelayCommand(OnFidthToPage, GetIsReportLoad);
			PrintReportCommand = new RelayCommand(OnPrintReport, GetIsReportLoad);
			RefreshCommand = new RelayCommand(OnRefresh, GetIsReportLoad);
			FilterCommand = new RelayCommand(OnFilter, GetIsReportLoad);
			ZoomInCommand = new RelayCommand(OnZoomIn, GetIsReportLoad);
			ZoomOutCommand = new RelayCommand(OnZoomOut, GetIsReportLoad);

			SetViewerSettings(ReportViewer);

			_reportMap = new Dictionary<ReportType, BaseReport>()
		    {
		        {ReportType.ReportIndicationBlock, new ReportIndicationBlock()},
		        {ReportType.ReportJournal, new ReportJournal()},
		        {ReportType.ReportDriverCounter, new ReportDriverCounter()},
		        {ReportType.ReportDeviceParams, new ReportDeviceParams()},
		        {ReportType.ReportDevicesList, new ReportDevicesList()}
		    };
		}

		public ReportViewer ReportViewer
		{
			get { return (ReportViewer)_winFormHost.Child; }
			set
			{
				_winFormHost.Child = value;
				OnPropertyChanged("ReportViewer");
			}
		}

		public double ZoomMinimumValue { get { return 10; } }
		public double ZoomMaximumValue { get { return 1000; } }
		Dictionary<ReportType, BaseReport> _reportMap;

		public int ZoomValue
		{
			get
			{
				return ReportViewer.ZoomPercent;
			}
			set
			{
				ReportViewer.ZoomPercent = value;
				ReportViewer.ZoomMode = ZoomMode.Percent;
				Update();
			}
		}

		public bool IsReportLoad
		{
			get { return GetIsReportLoad(); }
		}
		public bool IsJournalReport
		{
			get { return SelectedReportName == ReportType.ReportJournal; }
		}
		public List<ReportType> AvailableReportTypes
		{
			get { return Enum.GetValues(typeof(ReportType)).Cast<ReportType>().ToList(); }
		}

		public string TotalPageNumber
		{
			get { return ReportViewer.GetTotalPages().ToString(); }
		}
		public string CurrentPageNumber
		{
			get { return ReportViewer.CurrentPage.ToString(); }
			set
			{
				var currentPage = int.Parse(value);
				if (currentPage <= ReportViewer.GetTotalPages())
				{
					ReportViewer.CurrentPage = currentPage;
					OnPropertyChanged("CurrentPageNumber");
				}
			}
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
			ReportViewer = new Microsoft.Reporting.WinForms.ReportViewer();
			SetViewerSettings(ReportViewer);
			baseReport.LoadReportViewer(ReportViewer);
			ReportViewer.RefreshReport();
			Update();
		}

		void SetViewerSettings(ReportViewer reportViewer)
		{
			reportViewer.ShowExportButton = false;
			reportViewer.ShowBackButton = false;
			reportViewer.ShowCredentialPrompts = false;
			reportViewer.ShowDocumentMapButton = false;
			reportViewer.ShowFindControls = false;
			reportViewer.ShowPageNavigationControls = false;
			reportViewer.ShowParameterPrompts = false;
			reportViewer.ShowPrintButton = false;
			reportViewer.ShowPromptAreaButton = false;
			reportViewer.ShowRefreshButton = false;
			reportViewer.ShowStopButton = false;
			reportViewer.ShowToolBar = false;
			reportViewer.ShowZoomControl = false;
			reportViewer.ProcessingMode = ProcessingMode.Local;
			reportViewer.SetDisplayMode(DisplayMode.PrintLayout);
		}

		public RelayCommand ZoomOutCommand { get; private set; }
		void OnZoomOut()
		{
			ReportViewer.ZoomMode = ZoomMode.Percent;
			ReportViewer.ZoomPercent += 10;
			Update();
		}

		public RelayCommand ZoomInCommand { get; private set; }
		void OnZoomIn()
		{
			ReportViewer.ZoomMode = ZoomMode.Percent;
			ReportViewer.ZoomPercent += 10;
			Update();
		}

		public RelayCommand FirstPageCommand { get; private set; }
		void OnFirstPage()
		{
			CurrentPageNumber = "1";
		}

		public RelayCommand NextPageCommand { get; private set; }
		void OnNextPage()
		{
			CurrentPageNumber = (int.Parse(CurrentPageNumber) + 1).ToString();
			Update();
		}

		public RelayCommand PreviousPageCommand { get; private set; }
		void OnPreviousPage()
		{
			var currentPageNumber = int.Parse(CurrentPageNumber);
			if (currentPageNumber > 1)
				CurrentPageNumber = (int.Parse(CurrentPageNumber) - 1).ToString();
		}

		public RelayCommand LastPageCommand { get; private set; }
		void OnLastPage()
		{
			CurrentPageNumber = ReportViewer.GetTotalPages().ToString();
		}

		public RelayCommand FidthToPageCommand { get; private set; }
		void OnFidthToPage()
		{
			ReportViewer.ZoomMode = ZoomMode.PageWidth;
		}

		public RelayCommand PrintReportCommand { get; private set; }
		void OnPrintReport()
		{
			ReportViewer.PrintDialog();
		}

		public RelayCommand RefreshCommand { get; private set; }
		void OnRefresh()
		{
			SelectedReportName = SelectedReportName;
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
				ShowReport(new ReportJournal(archiveFilterViewModel));
			}
		}

		bool GetIsReportLoad()
		{
			return SelectedReportName != null;
		}

		void Update()
		{
			OnPropertyChanged("CurrentPageNumber");
			OnPropertyChanged("TotalPageNumber");
			OnPropertyChanged("ZoomValue");
			OnPropertyChanged("IsReportLoad");
		}

		#region INotifyPropertyChanged Members and helper

		private readonly NotifyPropertyChangedHelper _propertyChangeHelper = new NotifyPropertyChangedHelper();

		public event PropertyChangedEventHandler PropertyChanged
		{
			add { _propertyChangeHelper.Add(value); }
			remove { _propertyChangeHelper.Remove(value); }
		}

		protected void SetValue<T>(ref T field, T value, params string[] propertyNames)
		{
			_propertyChangeHelper.SetValue(this, ref field, value, propertyNames);
		}

		public void OnPropertyChanged(string propertyName)
		{
			_propertyChangeHelper.NotifyPropertyChanged(this, propertyName);
		}

		#endregion
	}
}