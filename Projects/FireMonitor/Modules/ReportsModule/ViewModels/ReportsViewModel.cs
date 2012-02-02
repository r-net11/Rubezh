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

namespace ReportsModule.ViewModels
{
    public class ReportsViewModel : RegionViewModel
    {
        public ReportsViewModel()
        {
            FirstPageCommand = new RelayCommand(OnFirstPage, IsReportLoad);
            NextPageCommand = new RelayCommand(OnNextPage, IsReportLoad);
            PreviousPageCommand = new RelayCommand(OnPreviousPage, IsReportLoad);
            LastPageCommand = new RelayCommand(OnLastPage, IsReportLoad);
            FidthToPageCommand = new RelayCommand(OnFidthToPage, IsReportLoad);
            SaveReportCommand = new RelayCommand(OnSaveReportCommand, IsReportLoad);

            ReportNames = new List<string>()
            {
                "Блоки индикации",
                "Журнал событий",
                "Количество устройств по типам",
                "Параметры устройств",
                "Список устройств"
            };
            ViewerCoreControl = new SAPBusinessObjects.WPF.Viewer.ViewerCore();
        }

        void ShowCrystalReport(BaseReport baseReport)
        {
            baseReport.LoadData();
            var viewerCore = new SAPBusinessObjects.WPF.Viewer.ViewerCore();
            viewerCore.ToggleSidePanel = Constants.SidePanelKind.None;
            viewerCore.SelectionMode = Constants.ObjectSelectionMode.One;
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

        public List<string> ReportNames { get; private set; }

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
                return ViewerCoreControl.CurrentPageNumber.ToString();
            }
            set
            {
                int pageNumber = 0;
                if (int.TryParse(value, out pageNumber))
                {
                    if (pageNumber > 0 && pageNumber < ViewerCoreControl.TotalPageNumber)
                    {
                        ViewerCoreControl.CurrentPageNumber = pageNumber;
                        ViewerCoreControl.ShowNthPage(pageNumber);
                        Update();
                    }
                }
            }
        }

        string _selectedReportName;
        public string SelectedReportName
        {
            get { return _selectedReportName; }
            set
            {
                _selectedReportName = value;
                OnPropertyChanged("SelectedReportName");

                switch (value)
                {
                    case "Блоки индикации":
                        ShowCrystalReport(new ReportIndicationBlock());
                        return;

                    case "Журнал событий":
                        ShowCrystalReport(new ReportJournal());
                        return;

                    case "Количество устройств по типам":
                        ShowCrystalReport(new ReportDriverCounter());
                        return;

                    case "Параметры устройств":
                        ShowCrystalReport(new ReportDeviceParams());
                        return;

                    case "Список устройств":
                        ShowCrystalReport(new ReportDevicesList());
                        return;
                }
            }
        }

        bool IsReportLoad()
        {
            return SelectedReportName != null;
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

        void Update()
        {
            OnPropertyChanged("ZoomValue");
            OnPropertyChanged("CurrentPageNumber");
            OnPropertyChanged("TotalPageNumber");
        }
    }
}