using Infrastructure.Common;
using Microsoft.Reporting.WinForms;
using ReportsModule.Reports;
using ReportsModule.Views;
using SAPBusinessObjects.WPF.Viewer;

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
        }
        
        public void Initialize()
        {
        }

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

        void ShowCrystalReport(BaseReport report)
        {
            report.LoadData();
            ReportContent = report.CreateCrystalReportViewer();
        }

        public RelayCommand ShowDriverCountReportCommand { get; private set; }
        void OnShowDriverCountReportCommand()
        {
            ShowCrystalReport(new ReportDriverCounter());
        }

        public RelayCommand ShowDeviceParamsReportCommand { get; private set; }
        void OnShowDeviceParamsReportCommand()
        {
            ShowCrystalReport(new ReportDeviceParams());
        }

        public RelayCommand ShowDeviceListReportCommand { get; private set; }
        void OnShowDeviceListReportCommand()
        {
            ShowCrystalReport(new ReportDevicesList());
        }

        public RelayCommand ShowJournalReportCommand { get; private set; }
        void OnShowJournalReportCommand()
        {
            ShowCrystalReport(new ReportJournal());
        }

        public RelayCommand ShowIndicationBlockReportCommand { get; private set; }
        void OnShowIndicationBlockReportCommand()
        {
            ShowCrystalReport(new ReportIndicationBlock());
        }
    }
}