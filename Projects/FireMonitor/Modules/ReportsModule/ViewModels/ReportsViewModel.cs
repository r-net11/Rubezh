using Infrastructure.Common;
using Microsoft.Reporting.WinForms;
using ReportsModule.Reports;
using ReportsModule.Views;
using SAPBusinessObjects.WPF.Viewer;
using ReportsModule.CrystalReports;

namespace ReportsModule.ViewModels
{
    public class ReportsViewModel : RegionViewModel
    {
        public ReportsViewModel()
        {
            ShowJournalReportCommand = new RelayCommand(OnShowJournalReportCommand);
            ShowDeviceListReportCommand = new RelayCommand(OnShowDeviceListReportCommand);
            ShowDeviceParamsReportCommand = new RelayCommand(OnShowDeviceParamsReportCommand);
            ShowDriverCountReportCommand = new RelayCommand(OnShowDriverCountReportCommand);
            ShowIndicationBlockReportCommand = new RelayCommand(OnShowIndicationBlockReportCommand);
            ShowTestCrystalReportCommand = new RelayCommand(OnShowTestCrystalReport);
        }
        
        public void Initialize()
        {
            _reportViewer = new ReportViewer();
            //_crystalReportsViewer = new CrystalReportsViewer();
            //ReportContent = _crystalReportsViewer;
        }

        ReportViewer _reportViewer;
        //CrystalReportsViewer _crystalReportsViewer;

        object _reportContent;
        public object ReportContent
        {
            get { return _reportContent; }
            set
            {
                _reportContent = value;
                OnPropertyChanged("ReportContent");
            }
        }

        void ShowReport(BaseReport report)
        {
            report.LoadData();
            _reportViewer = report.CreateReportViewer();
            _reportViewer.SetDisplayMode(DisplayMode.PrintLayout);

            var _reportViewerWinForm = new ReportViewerWinForm();
            _reportViewerWinForm.winFormsHost.Child = _reportViewer;
            _reportViewerWinForm.Show();
        }

        public RelayCommand ShowDriverCountReportCommand { get; private set; }
        void OnShowDriverCountReportCommand()
        {
            ShowReport(new ReportDriverCounter());
        }

        public RelayCommand ShowDeviceParamsReportCommand { get; private set; }
        void OnShowDeviceParamsReportCommand()
        {
            ShowReport(new ReportDeviceParams());
        }

        public RelayCommand ShowDeviceListReportCommand { get; private set; }
        void OnShowDeviceListReportCommand()
        {
            ShowReport(new ReportDevicesList());
        }

        public RelayCommand ShowJournalReportCommand { get; private set; }
        void OnShowJournalReportCommand()
        {
            ShowReport(new ReportJournal());
        }

        public RelayCommand ShowIndicationBlockReportCommand { get; private set; }
        void OnShowIndicationBlockReportCommand()
        {
            ShowReport(new ReportIndicationBlock());
        }

        public RelayCommand ShowTestCrystalReportCommand { get; private set; }
        void OnShowTestCrystalReport()
        {
            //var crystalReportTest = new CrystalReportIndicationBlock();
            ////var crystalReportTest = new CrystalReportTestIndicationBlock();
            //var reportIndicationBlockDataTable = new ReportIndicationBlock();
            //reportIndicationBlockDataTable.TestInitialize();
            //crystalReportTest.SetDataSource(reportIndicationBlockDataTable.DataList);
            //_crystalReportsViewer.ViewerCore.ReportSource = crystalReportTest;
            //ReportContent = _crystalReportsViewer;
        }
    }
}