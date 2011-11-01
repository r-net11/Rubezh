using Infrastructure.Common;
using ReportsModule.Reports;
using SAPBusinessObjects.WPF.Viewer;
using System.Windows.Controls;

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

        //public CrystalReportsViewer CrystalReportsViewer { get; set; }

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

        public object IndicationBlockReport
        {
            get { return ShowReport(new ReportIndicationBlock()); }
        }

        public object DriverCountReport
        {
            get { return ShowReport(new ReportDriverCounter()); }
        }

        public object DeviceParamsReport
        {
            get 
            {
                return ShowReport(new ReportDeviceParams());
            }
        }

        public object DeviceListReport
        {
            get { return ShowReport(new ReportDevicesList()); }
        }

        public object JournalReport
        {
            get { return ShowReport(new ReportJournal()); }
        }

        void ShowCrystalReport(BaseReport report)
        {
            report.LoadData();
            ReportContent = report.CreateCrystalReportViewer();
        }

        object ShowReport(BaseReport report)
        {
            report.LoadData();
            return report.CreateCrystalReportViewer();
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