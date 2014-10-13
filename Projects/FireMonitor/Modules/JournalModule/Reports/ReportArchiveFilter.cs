//using System;
//using System.Collections.Generic;
//using JournalModule.ViewModels;
//using FiresecAPI.Journal;

//namespace JournalModule.Reports
//{
//    internal class ReportArchiveFilter
//    {
//        public ReportArchiveFilter()
//        {
//            ArchiveFilter = new ArchiveFilter() { StartDate = ArchiveFirstDate < DateTime.Now.AddSeconds(-1) ? DateTime.Now.AddSeconds(-1) : ArchiveFirstDate, EndDate = DateTime.Now };
//            StartDate = ArchiveFilter.StartDate;
//            EndDate = ArchiveFilter.EndDate;
//            Initialize();
//        }

//        public ReportArchiveFilter(ArchiveFilterViewModel archiveFilterViewModel)
//        {
//            SetFilter(archiveFilterViewModel);
//            Initialize();
//        }

//        void Initialize()
//        {
//            JournalItems = new List<JournalItem>();
//        }

//        public readonly DateTime ArchiveFirstDate;// = FiresecManager.GetArchiveStartDate().Result;
//        public List<JournalItem> JournalItems { get; set; }
//        public ArchiveFilter ArchiveFilter { get; set; }
//        public bool IsFilterOn { get; set; }
//        public DateTime StartDate { get; private set; }
//        public DateTime EndDate { get; private set; }

//        void SetFilter(ArchiveFilterViewModel archiveFilterViewModel)
//        {
//            ArchiveFilter = archiveFilterViewModel.GetModel();
//            StartDate = archiveFilterViewModel.StartDateTime.DateTime;
//            EndDate = archiveFilterViewModel.EndDateTime.DateTime;
//        }

//        public void LoadArchive()
//        {
//            var filteredJournalItems = GKDBHelper.BeginGetGKFilteredArchive(ArchiveFilter, Guid.NewGuid(), true);
//            foreach (var journalItem in filteredJournalItems)
//            {
//                JournalItems.Add(journalItem);
//            }
//        }
//    }
//}