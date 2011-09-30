using System.Collections.Generic;
using Common;
using Infrastructure.Common;
using Microsoft.Reporting.WinForms;
using ReportsModule.Models;
using ReportsModule.Reports;
using ReportsModule.Views;
using System.Security;
using System.Security.Permissions;

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
        }

        public void Initialize()
        {
            _testReportViewer = new TestReportViewer();
            reportJournalDataTable = new ReportJournalDataTable();
            ReportContent = _testReportViewer;
            _reportViewer = new ReportViewer();
        }

        TestReportViewer _testReportViewer;
        ReportJournalDataTable reportJournalDataTable;
        ReportViewer _reportViewer;

        public ReportViewer CreateReportViewer<T>(List<T> dataList, string dataListName, string rdlcFileName)
        {
            if (dataList.IsNotNullOrEmpty() == false)
            {
                return null;
            }
            var reportViewer = new ReportViewer();
            
            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.LocalReport.ReportPath = rdlcFileName;
            //this.reportViewer1.LocalReport.SetParameters(new ReportParameter[] { startDate2 });
            reportViewer.LocalReport.DataSources.Add(new ReportDataSource(dataListName, dataList));
            return reportViewer;
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

        public RelayCommand ShowDriverCountReportCommand { get; private set; }
        void OnShowDriverCountReportCommand()
        {
            var reportDriverCounterDataTable = new ReportDriverCounterDataTable();
            reportDriverCounterDataTable.Initialize();
            _reportViewer = Helper.CreateReportViewer<ReportDriverCounterModel>(reportDriverCounterDataTable.DriverCounterList,
                reportDriverCounterDataTable.DataTableName, reportDriverCounterDataTable.RdlcFileName);
            _reportViewer.SetDisplayMode(DisplayMode.PrintLayout);
            Window1 window = new Window1();
            window.winFormsHost.Child = _reportViewer;
            window.Show();
        }

        public RelayCommand ShowDeviceParamsReportCommand { get; private set; }
        void OnShowDeviceParamsReportCommand()
        {
            var reportDeviceParamsDataTable = new ReportDeviceParamsDataTable();
            reportDeviceParamsDataTable.Initialize();
            _reportViewer = Helper.CreateReportViewer<ReportDeviceParamsModel>(reportDeviceParamsDataTable.DeviceParamsList,
                reportDeviceParamsDataTable.DataTableName, reportDeviceParamsDataTable.RdlcFileName);
            Window1 window = new Window1();
            window.winFormsHost.Child = _reportViewer;
            window.Show();
        }

        public RelayCommand ShowDeviceListReportCommand { get; private set; }
        void OnShowDeviceListReportCommand()
        {
            var reportDevicesListDataTable = new ReportDevicesListDataTable();
            reportDevicesListDataTable.Initialize();
            _reportViewer = Helper.CreateReportViewer<ReportDeviceListModel>(reportDevicesListDataTable.DevicesList,
                reportDevicesListDataTable.DataTableName, reportDevicesListDataTable.RdlcFileName);
            Window1 window = new Window1();
            window.winFormsHost.Child = _reportViewer;
            window.Show();
        }

        public RelayCommand ShowJournalReportCommand { get; private set; }
        void OnShowJournalReportCommand()
        {
            PermissionSet permissions = new PermissionSet(PermissionState.None);
            permissions.AddPermission(new FileIOPermission(PermissionState.Unrestricted));
            permissions.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));

            var reportJournalDataTable = new ReportJournalDataTable();
            reportJournalDataTable.Initialize();
            var _reportViewer2 = CreateReportViewer<ReportJournalModel>(reportJournalDataTable.JournalList,
                reportJournalDataTable.DataTableName, reportJournalDataTable.RdlcFileName);
            var startDate = new ReportParameter("StartDate",reportJournalDataTable.StartDate.ToString(),true);
            var endDate = new ReportParameter("EndDate", reportJournalDataTable.EndDate.ToString(),true);
            _reportViewer2.LocalReport.SetParameters(new ReportParameter[] { startDate, endDate });
            _reportViewer2.SetDisplayMode(DisplayMode.PrintLayout);
            Window1 window = new Window1();
            window.winFormsHost.Child = _reportViewer2;
            _reportViewer2.LocalReport.ReleaseSandboxAppDomain();
            window.ShowDialog();
            window.winFormsHost.Child = null;
            _reportViewer2.LocalReport.DataSources.Clear();
            
            //_reportViewer.Dispose();
            ////Form1 form = new Form1();
            ////form.Initialize(Helper.CreateReportViewer<ReportJournalModel>(reportJournalDataTable.JournalList,
            ////    reportJournalDataTable.DataTableName, reportJournalDataTable.RdlcFileName));
            ////form.ShowDialog();
            ////form.Dispose();

            ////System.Windows.Forms.Button button = new System.Windows.Forms.Button();
            ////button.Text = "OKEY";
            ////button.Width = 200;
            ////button.Height = 200;
            ////System.Windows.Forms.UserControl userControl = new System.Windows.Forms.UserControl();
            ////userControl.Controls.Add(button);
            //reportViewer.Height = 400;
            //reportViewer.Width = 400;
            //_testReportViewer.winFormsHost.Child = reportViewer;
            //_testReportViewer.winFormsHost.Visibility = Visibility.Visible;
            //_testReportViewer.winFormsHost.Height = 400;
            //_testReportViewer.winFormsHost.Width = 400;
            //_testReportViewer.winFormsHost.Child.Refresh();
            //ReportContent = _testReportViewer;
            //ReportContent = _testReportViewer;
            //OnPropertyChanged("ReportContent");
        }
    }
}