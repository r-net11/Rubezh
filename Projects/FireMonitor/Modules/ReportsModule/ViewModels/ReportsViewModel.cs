using System.Collections.Generic;
using Infrastructure.Common;
using ReportsModule.Reports;
using SAPBusinessObjects.WPF.Viewer;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using Infrastructure;
using ReportsModule.Views;
using CrystalDecisions.CrystalReports.Engine;
using System.ComponentModel;
using CrystalDecisions.Shared;
using FiresecAPI.Models;
using System;
using JournalModule.ViewModels;
using FiresecClient;
using System.Linq;

namespace ReportsModule.ViewModels
{
    public class ReportsViewModel : RegionViewModel
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

            ViewerCoreControl = new ViewerCore();
            SelectedReportName = null;
        }

        void ShowCrystalReport(BaseReport baseReport)
        {
            baseReport.LoadData();
            var viewerCore = new SAPBusinessObjects.WPF.Viewer.ViewerCore();
            viewerCore.ToggleSidePanel = Constants.SidePanelKind.None;
            viewerCore.ReportSource = baseReport.CreateCrystalReportDocument();
            ViewerCoreControl = viewerCore;
        }

        public int ZoomValue
        {
            get { return ViewerCoreControl.ZoomFactor; }
            set
            {
                ViewerCoreControl.ZoomFactor = value;
                ViewerCoreControl.Zoom(value);
                Update();
            }
        }
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
                OnPropertyChanged("SelectedReportName");
                OnPropertyChanged("IsReportLoad");
                OnPropertyChanged("IsJournalReport");
                switch (value)
                {
                    case ReportType.ReportIndicationBlock:
                        ShowCrystalReport(new ReportIndicationBlock());
                        return;

                    case ReportType.ReportJournal:
                        ShowCrystalReport(new ReportJournal());
                        return;

                    case ReportType.ReportDriverCounter:
                        ShowCrystalReport(new ReportDriverCounter());
                        return;

                    case ReportType.ReportDeviceParams:
                        ShowCrystalReport(new ReportDeviceParams());
                        return;

                    case ReportType.ReportDevicesList:
                        ShowCrystalReport(new ReportDevicesList());
                        return;
                }
            }
        }

        public string TotalPageNumber
        {
            get
            {
                if (ViewerCoreControl.GetLastPageNumber() > 0)
                    return ViewerCoreControl.GetLastPageNumber().ToString();
                else
                    return "";
            }
        }

        public string CurrentPageNumber
        {
            get
            {
                if (ViewerCoreControl.CurrentPageNumber == 0)
                    return 1.ToString();
                else
                    return ViewerCoreControl.CurrentPageNumber.ToString();
            }
            set
            {
                int pageNumber = 0;
                if (int.TryParse(value, out pageNumber))
                {
                    if (pageNumber > 0 && pageNumber <= ViewerCoreControl.TotalPageNumber)
                    {
                        ViewerCoreControl.CurrentPageNumber = pageNumber;
                        ViewerCoreControl.ShowNthPage(pageNumber);
                        Update();
                    }
                }
            }
        }

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

        ViewerCore _viewerCore;
        public ViewerCore ViewerCoreControl
        {
            get { return _viewerCore; }
            set
            {
                _viewerCore = value;
                OnPropertyChanged("ViewerCoreControl");
            }
        }

        public RelayCommand FirstPageCommand { get; private set; }
        void OnFirstPage()
        {
            ViewerCoreControl.ShowFirstPage();
            Update();
        }

        public RelayCommand NextPageCommand { get; private set; }
        void OnNextPage()
        {
            ViewerCoreControl.ShowNextPage();
            Update();
        }

        public RelayCommand PreviousPageCommand { get; private set; }
        void OnPreviousPage()
        {
            ViewerCoreControl.ShowPreviousPage();
            Update();
        }

        public RelayCommand LastPageCommand { get; private set; }
        void OnLastPage()
        {
            ViewerCoreControl.ShowLastPage();
            Update();
        }

        public RelayCommand FidthToPageCommand { get; private set; }
        void OnFidthToPage()
        {
            ViewerCoreControl.ZoomFactor = 79;
            ViewerCoreControl.Zoom(ViewerCoreControl.ZoomFactor);
            Update();
        }

        public RelayCommand SaveReportCommand { get; private set; }
        void OnSaveReportCommand()
        {
            ViewerCoreControl.ExportReport();
        }

        public RelayCommand PrintReportCommand { get; private set; }
        void OnPrintReport()
        {
            ViewerCoreControl.PrintReport();
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
            if (ServiceFactory.UserDialogs.ShowModalWindow(archiveFilterViewModel))
            {
                ShowCrystalReport(new ReportJournal(archiveFilterViewModel));
            }
        }

        public RelayCommand SearchCommand { get; private set; }
        void OnSearch()
        {
            if (!String.IsNullOrEmpty(SearchText))
            {
                ViewerCoreControl.SearchForText(SearchText, false, false);
                Update();
            }
        }

        void Update()
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