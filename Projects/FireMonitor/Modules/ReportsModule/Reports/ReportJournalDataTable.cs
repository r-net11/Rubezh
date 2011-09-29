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
            _journalList = new List<ReportJournalModel>();
        }

        public void Initialize()
        {
            ReportArchiveFilter _reportArchiveFilter = new ReportArchiveFilter();
            _reportArchiveFilter.ShowFilter();
            foreach (var journalRecord in _reportArchiveFilter.JournalRecords)
            {
                _journalList.Add(new ReportJournalModel()
                {
                    DeviceTime = journalRecord.DeviceTime,
                    SystemTime = journalRecord.SystemTime,
                    ZoneName = journalRecord.ZoneName,
                    Description = journalRecord.Description,
                    Device = journalRecord.Device,
                    Panel = journalRecord.Panel,
                    User = journalRecord.User
                });
            }

            StartDate = _reportArchiveFilter.StartDate;
            EndDate = _reportArchiveFilter.EndDate;
        }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        
        List<ReportJournalModel> _journalList;
        public List<ReportJournalModel> JournalList
        {
            get { return new List<ReportJournalModel>(_journalList); }
        }

        class ReportArchiveFilter
        {
            ArchiveFilter _archiveFilter;
            readonly ArchiveDefaultState _archiveDefaultState;

            public ReportArchiveFilter()
            {
                _archiveDefaultState = new ArchiveDefaultState()
                {
                    ArchiveDefaultStateType = ArchiveDefaultStateType.LastDay,
                    ArchiveFilter = new ArchiveFilter()
                    {
                        EndDate = DateTime.Now,
                        StartDate = DateTime.Now.AddDays(-1),
                        UseSystemDate = false
                    }
                };
                _archiveFilter = _archiveDefaultState.ArchiveFilter;
                IsFilterOn = false;
            }

            public DateTime StartDate
            {
                get { return _archiveFilter.StartDate; }
            }

            public DateTime EndDate
            {
                get { return _archiveFilter.EndDate; }
            }

            bool _isFilterOn;
            public bool IsFilterOn
            {
                get { return _isFilterOn; }
                set
                {
                    if (value)
                    {
                        ApplyFilter();
                    }
                    else
                    {
                        SetDefaultArchiveContent();
                    }

                    _isFilterOn = value;
                }
            }

            List<JournalRecordViewModel> _journalRecords;
            public List<JournalRecordViewModel> JournalRecords
            {
                get { return _journalRecords; }
                private set
                {
                    _journalRecords = value;
                }
            }

            void SetDefaultArchiveContent()
            {
                switch (_archiveDefaultState.ArchiveDefaultStateType)
                {
                    case ArchiveDefaultStateType.LastHour:
                        _archiveDefaultState.ArchiveFilter.EndDate = DateTime.Now;
                        _archiveDefaultState.ArchiveFilter.StartDate = DateTime.Now.AddHours(-1);
                        break;
                    case ArchiveDefaultStateType.LastDay:
                        _archiveDefaultState.ArchiveFilter.EndDate = DateTime.Now;
                        _archiveDefaultState.ArchiveFilter.StartDate = DateTime.Now.AddDays(-1);
                        break;
                    case ArchiveDefaultStateType.FromDate:
                        _archiveDefaultState.ArchiveFilter.EndDate = _archiveFilter.EndDate;
                        _archiveDefaultState.ArchiveFilter.StartDate = _archiveFilter.StartDate;
                        break;
                    case ArchiveDefaultStateType.Range:
                        _archiveDefaultState.ArchiveFilter.EndDate = DateTime.Now;
                        _archiveDefaultState.ArchiveFilter.StartDate = _archiveFilter.StartDate;
                        break;
                }

                JournalRecords = new List<JournalRecordViewModel>(
                    FiresecManager.GetFilteredArchive(_archiveDefaultState.ArchiveFilter).
                    Select(journalRecord => new JournalRecordViewModel(journalRecord))
                );
            }

            void ApplyFilter()
            {
                JournalRecords = new List<JournalRecordViewModel>(
                    FiresecManager.GetFilteredArchive(_archiveFilter).
                    Select(journalRecord => new JournalRecordViewModel(journalRecord))
                );
            }

            public void ShowFilter()
            {
                var archiveFilterViewModel = new ArchiveFilterViewModel(_archiveFilter);
                if (ServiceFactory.UserDialogs.ShowModalWindow(archiveFilterViewModel))
                {
                    _archiveFilter = archiveFilterViewModel.GetModel();
                    IsFilterOn = true;
                }
            }
        }
    }
}
