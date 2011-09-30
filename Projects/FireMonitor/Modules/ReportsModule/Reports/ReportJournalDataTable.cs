using System;
using System.Collections.Generic;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using JournalModule.ViewModels;
using ReportsModule.Models;

namespace ReportsModule.Reports
{
    public class ReportJournalDataTable
    {
        public string RdlcFileName = "ReportJournalRDLC.rdlc";
        public string DataTableName = "DataSetJournal";

        public ReportJournalDataTable()
        {
            JournalList = new List<ReportJournalModel>();
        }

        public void Initialize()
        {
            var reportArchiveFilter = new ReportArchiveFilter();
            if (reportArchiveFilter.ShowFilter())
            {
                foreach (var journalRecord in FiresecManager.GetFilteredArchive(reportArchiveFilter.ArchiveFilterViewModel.GetModel()))
                {
                    JournalList.Add(new ReportJournalModel()
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
        public List<ReportJournalModel> JournalList { get; set; }

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
