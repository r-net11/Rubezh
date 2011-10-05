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
            ShowIndicationBlockReportCommand = new RelayCommand(OnShowIndicationBlockReportCommand);
        }

        public void Initialize()
        {
            _testReportViewer = new TestReportViewer();
            reportJournalDataTable = new ReportJournalDataTable();
            ReportContent = _testReportViewer;
            _reportViewer = new ReportViewer();
            _reportViewerWinForm = new ReportViewerWinForm();
            _permissions = new PermissionSet(PermissionState.None);
            _permissions.AddPermission(new FileIOPermission(PermissionState.Unrestricted));
            _permissions.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));
        }

        TestReportViewer _testReportViewer;
        ReportJournalDataTable reportJournalDataTable;
        ReportViewer _reportViewer;
        ReportViewerWinForm _reportViewerWinForm;
        PermissionSet _permissions;

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

        void ShowReport()
        {
            _reportViewer.LocalReport.SetBasePermissionsForSandboxAppDomain(_permissions);
            _reportViewer.SetDisplayMode(DisplayMode.PrintLayout);

            _reportViewerWinForm = new ReportViewerWinForm();
            _reportViewerWinForm.winFormsHost.Child = _reportViewer;
            _reportViewer.LocalReport.ReleaseSandboxAppDomain();
            _reportViewerWinForm.ShowDialog();
        }

        public RelayCommand ShowDriverCountReportCommand { get; private set; }
        void OnShowDriverCountReportCommand()
        {
            var reportDriverCounterDataTable = new ReportDriverCounterDataTable();
            reportDriverCounterDataTable.Initialize();
            _reportViewer = Helper.CreateReportViewer<ReportDriverCounterModel>(reportDriverCounterDataTable.DriverCounterList,
                reportDriverCounterDataTable.DataTableName, reportDriverCounterDataTable.RdlcFileName);
            ShowReport();
        }

        public RelayCommand ShowDeviceParamsReportCommand { get; private set; }
        void OnShowDeviceParamsReportCommand()
        {
            var reportDeviceParamsDataTable = new ReportDeviceParamsDataTable();
            reportDeviceParamsDataTable.Initialize();
            using (_reportViewer = Helper.CreateReportViewer<ReportDeviceParamsModel>(reportDeviceParamsDataTable.DeviceParamsList,
                reportDeviceParamsDataTable.DataTableName, reportDeviceParamsDataTable.RdlcFileName))
            {
                ShowReport();
            }
        }

        public RelayCommand ShowDeviceListReportCommand { get; private set; }
        void OnShowDeviceListReportCommand()
        {
            var reportDevicesListDataTable = new ReportDevicesListDataTable();
            reportDevicesListDataTable.Initialize();
            _reportViewer = Helper.CreateReportViewer<ReportDeviceListModel>(reportDevicesListDataTable.DevicesList,
                reportDevicesListDataTable.DataTableName, reportDevicesListDataTable.RdlcFileName);
            ShowReport();
        }

        public RelayCommand ShowJournalReportCommand { get; private set; }
        void OnShowJournalReportCommand()
        {
            var reportJournalDataTable = new ReportJournalDataTable();
            reportJournalDataTable.Initialize();
            _reportViewer = Helper.CreateReportViewer<ReportJournalModel>(reportJournalDataTable.JournalList,
                reportJournalDataTable.DataTableName, reportJournalDataTable.RdlcFileName);
            var startDate = new ReportParameter("StartDate", reportJournalDataTable.StartDate.ToString(), true);
            var endDate = new ReportParameter("EndDate", reportJournalDataTable.EndDate.ToString(), true);
            var header = new ReportParameter("header", new string[] { "1", "2", "3", "4" });
            _reportViewer.LocalReport.SetParameters(new ReportParameter[] { startDate, endDate, header });
            ShowReport();

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

        public RelayCommand ShowIndicationBlockReportCommand { get; private set; }
        void OnShowIndicationBlockReportCommand()
        {
            _reportViewerWinForm = new ReportViewerWinForm();

            var reportIndicationBlockDataTable = new ReportIndicationBlockDataTable();
            reportIndicationBlockDataTable.Initialize();
            _reportViewer = Helper.CreateReportViewer<ReportIndicationBlockModel>(reportIndicationBlockDataTable.IndicationBlockList,
                reportIndicationBlockDataTable.DataTableName, reportIndicationBlockDataTable.RdlcFileName);
            ShowReport();
        }
    }
}