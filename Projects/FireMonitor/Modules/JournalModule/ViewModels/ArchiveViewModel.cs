using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;

namespace JournalModule.ViewModels
{
    public class ArchiveViewModel : RegionViewModel
    {
        readonly ArchiveFilterViewModel _archiveFilter;

        public ArchiveViewModel()
        {
            SetDefaultArchiveContent();

            _archiveFilter = new ArchiveFilterViewModel();

            ShowFilterCommand = new RelayCommand(OnShowFilter);
        }

        bool _isFilterOn;
        public bool IsFilterOn
        {
            get { return _isFilterOn; }
            set
            {
                if (value)
                {
                    if (_archiveFilter.IsClear)
                    {
                        OnShowFilter();
                        return;
                    }
                    else
                    {
                        ApplyFilter();
                    }
                }
                else
                {
                    SetDefaultArchiveContent();
                }

                _isFilterOn = value;
                OnPropertyChanged("IsFilterOn");
            }
        }

        ObservableCollection<JournalRecordViewModel> _journalRecords;
        public ObservableCollection<JournalRecordViewModel> JournalRecords
        {
            get { return _journalRecords; }
            private set
            {
                _journalRecords = value;
                OnPropertyChanged("JournalRecords");
            }
        }

        JournalRecordViewModel _selectedRecord;
        public JournalRecordViewModel SelectedRecord
        {
            get { return _selectedRecord; }
            set
            {
                _selectedRecord = value;
                OnPropertyChanged("SelectedRecord");
            }
        }

        void SetDefaultArchiveContent()
        {
            try
            {
                JournalRecords = new ObservableCollection<JournalRecordViewModel>(
                    FiresecManager.GetFilteredJournal(new JournalFilter() { LastRecordsCount = 100 }).
                    Select(journalRecord => new JournalRecordViewModel(journalRecord))
                );
            }
            catch { ;}
        }

        void ApplyFilter()
        {
            ArchiveFilter filter = new ArchiveFilter()
            {
                Descriptions = new List<string>(
                    _archiveFilter.JournalEvents.Where(x => x.IsEnable).Select(x => x.Name)
                ),
                Subsystems = new List<SubsystemType>(
                    _archiveFilter.Subsystems.Where(x => x.IsEnable).Select(x => x.Subsystem)
                ),
                UseSystemDate = _archiveFilter.UseSystemDate,
                StartDate = _archiveFilter.StartDate,
                EndDate = _archiveFilter.EndDate,
            };
            if (filter.Subsystems.Count == 0)
            {
                foreach (SubsystemType subsystem in Enum.GetValues(typeof(SubsystemType)))
                {
                    filter.Subsystems.Add(subsystem);
                }
            }

            JournalRecords = new ObservableCollection<JournalRecordViewModel>(
                FiresecManager.GetFilteredArchive(filter).Select(journalRecord => new JournalRecordViewModel(journalRecord))
            );
        }

        public RelayCommand ShowFilterCommand { get; private set; }
        void OnShowFilter()
        {
            ArchiveFilterViewModel tmpArchiveFilter = new ArchiveFilterViewModel();
            _archiveFilter.CopyTo(tmpArchiveFilter);
            if (ServiceFactory.UserDialogs.ShowModalWindow(tmpArchiveFilter))
            {
                tmpArchiveFilter.CopyTo(_archiveFilter);
                IsFilterOn = true;
            }
        }
    }
}