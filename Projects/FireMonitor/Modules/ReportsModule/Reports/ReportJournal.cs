using System;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using JournalModule.ViewModels;
using Microsoft.Reporting.WinForms;
using ReportsModule.Models;

namespace ReportsModule.Reports
{
    public class ReportJournal : BaseReportGeneric<ReportJournalModel>
    {
        public ReportJournal()
        {
            base.RdlcFileName = "ReportJournalRDLC.rdlc";
            base.DataTableName = "DataSetJournal";
        }

        public override void LoadData()
        {
            var reportArchiveFilter = new ReportArchiveFilter();
            if (reportArchiveFilter.ShowFilter())
            {
                foreach (var journalRecord in FiresecManager.GetFilteredArchive(reportArchiveFilter.ArchiveFilterViewModel.GetModel()))
                {
                    DataList.Add(new ReportJournalModel()
                    {
                        DeviceTime = journalRecord.DeviceTime.ToString(),
                        SystemTime = journalRecord.SystemTime.ToString(),
                        ZoneName = journalRecord.ZoneName,
                        Description = journalRecord.Description,
                        Device = journalRecord.DeviceName,
                        Panel = journalRecord.PanelName,
                        User = journalRecord.User
                    });
                }
                StartDate = reportArchiveFilter.ArchiveFilterViewModel.StartDate;
                EndDate = reportArchiveFilter.ArchiveFilterViewModel.EndDate;
            }
        }

        public DateTime EndDate { get; set; }
        public DateTime StartDate { get; set; }

        public override ReportViewer CreateReportViewer()
        {
            var reportViewer = base.CreateReportViewer();
            var startDate = new ReportParameter("StartDate", StartDate.ToString(), true);
            var endDate = new ReportParameter("EndDate", EndDate.ToString(), true);
            var header = new ReportParameter("header", new string[] { "1", "2", "3", "4" });
            reportViewer.LocalReport.SetParameters(new ReportParameter[] { startDate, endDate, header });

            return reportViewer;
        }

        class ReportArchiveFilter
        {
            public ArchiveFilterViewModel ArchiveFilterViewModel { get; set; }
            ArchiveFilter _archiveFilter;

            public ReportArchiveFilter()
            {
                _archiveFilter = new ArchiveFilter()
                {
                    EndDate = DateTime.Now,
                    StartDate = DateTime.Now.AddDays(-1),
                    UseSystemDate = false
                };
            }

            public bool ShowFilter()
            {
                ArchiveFilterViewModel = new ArchiveFilterViewModel(_archiveFilter);
                return ServiceFactory.UserDialogs.ShowModalWindow(ArchiveFilterViewModel);
            }
        }
    }
}
