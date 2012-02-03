using System;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using JournalModule.ViewModels;
using ReportsModule.Models;
using SAPBusinessObjects.WPF.Viewer;
using CrystalDecisions.CrystalReports.Engine;
using System.Collections.Generic;

namespace ReportsModule.Reports
{
    public class ReportJournal : BaseReportGeneric<ReportJournalModel>
    {
        public ReportJournal()
        {
            base.ReportFileName = "JournalCrystalReport.rpt";
            ReportArchiveFilter = new ReportArchiveFilter();
        }

        public ReportJournal(ArchiveFilterViewModel archiveFilterViewModel)
        {
            base.ReportFileName = "JournalCrystalReport.rpt";
            ReportArchiveFilter = new ReportArchiveFilter(archiveFilterViewModel);
        }

        public override void LoadData()
        {
            foreach (var journalRecord in ReportArchiveFilter.JournalRecords)
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
            StartDate = ReportArchiveFilter.StartDate;
            EndDate = ReportArchiveFilter.EndDate;
        }

        public DateTime EndDate { get; set; }
        public DateTime StartDate { get; set; }
        public ReportArchiveFilter ReportArchiveFilter { get; set; }

        public override ReportDocument CreateCrystalReportDocument()
        {
            if (DataList.IsNotNullOrEmpty() == false)
            {
                reportDocument.Load(FileHelper.GetReportFilePath(ReportFileName));
                return reportDocument;
            }
            reportDocument.Load(FileHelper.GetReportFilePath(ReportFileName));
            reportDocument.SetDataSource(DataList);
            reportDocument.SetParameterValue("StartDate", StartDate.ToString());
            reportDocument.SetParameterValue("EndDate", EndDate.ToString());

            return reportDocument;
        }
    }

    public class ReportArchiveFilter
    {
        public ReportArchiveFilter()
        {
            SetDefaultFilter();
            Initialize();
        }

        public ReportArchiveFilter(ArchiveFilterViewModel archiveFilterViewModel)
        {
            ArchiveFilter = archiveFilterViewModel.GetModel();
            StartDate = archiveFilterViewModel.StartDate;
            EndDate = archiveFilterViewModel.EndDate;
            Initialize();
        }

        void Initialize()
        {
            JournalRecords = new List<JournalRecord>();
            LoadArchive();
        }

        public readonly DateTime ArchiveFirstDate = FiresecManager.GetArchiveStartDate();
        public List<JournalRecord> JournalRecords { get; set; }
        ArchiveFilter ArchiveFilter { get; set; }
        public bool IsFilterOn { get; set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }

        public void SetDefaultFilter()
        {
            ArchiveFilter = new ArchiveFilter() { StartDate = ArchiveFirstDate, EndDate = DateTime.Now };
            StartDate = ArchiveFirstDate;
            EndDate = DateTime.Now;
        }

        public void LoadArchive()
        {
            JournalRecords = new List<JournalRecord>(FiresecManager.GetFilteredArchive(ArchiveFilter));
        }
    }
}
