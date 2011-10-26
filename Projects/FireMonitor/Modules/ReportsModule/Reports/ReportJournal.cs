using System;
using Common;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using JournalModule.ViewModels;
using Microsoft.Reporting.WinForms;
using ReportsModule.Models;
using SAPBusinessObjects.WPF.Viewer;
using ReportsModule.CrystalReports;

namespace ReportsModule.Reports
{
    public class ReportJournal : BaseReportGeneric<ReportJournalModel>
    {
        public ReportJournal()
        {
            base.ReportFileName = "JournalCrystalReport.rpt";
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

        public override CrystalReportsViewer CreateCrystalReportViewer()
        {
            if (DataList.IsNotNullOrEmpty() == false)
            {
                return new CrystalReportsViewer();
            }

            var filePath = FileHelper.GetReportFilePath(ReportFileName);
            reportDocument.Load(filePath);
            reportDocument.SetDataSource(DataList);
            reportDocument.SetParameterValue("StartDate", StartDate.ToString());
            reportDocument.SetParameterValue("EndDate", EndDate.ToString());
            var crystalReportsViewer = new CrystalReportsViewer();
            crystalReportsViewer.ViewerCore.ReportSource = reportDocument;
            return crystalReportsViewer;
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
