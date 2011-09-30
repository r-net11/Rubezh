using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReportsModule.Models;
using FiresecAPI.Models;
using JournalModule.ViewModels;
using FiresecClient;
using Infrastructure;
using System.Data;

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

        public DateTime EndDate { get; set; }
        public DateTime StartDate { get; set; }
        public List<ReportJournalModel> JournalList { get; set; }
    }
}
