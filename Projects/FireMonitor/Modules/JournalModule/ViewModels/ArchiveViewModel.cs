using System;
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
        ArchiveFilter _archiveFilter;
        readonly ArchiveDefaultState _archiveDefaultState;

        public ArchiveViewModel()
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

            ShowFilterCommand = new RelayCommand(OnShowFilter);
            ShowSettingsCommand = new RelayCommand(OnSettingsCommand);
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

            JournalRecords = new ObservableCollection<JournalRecordViewModel>(
                FiresecManager.GetFilteredArchive(_archiveDefaultState.ArchiveFilter).
                Select(journalRecord => new JournalRecordViewModel(journalRecord))
            );
        }

        void ApplyFilter()
        {
            JournalRecords = new ObservableCollection<JournalRecordViewModel>(
                FiresecManager.GetFilteredArchive(_archiveFilter).
                Select(journalRecord => new JournalRecordViewModel(journalRecord))
            );
        }

        public RelayCommand ShowFilterCommand { get; private set; }
        void OnShowFilter()
        {
            var archiveFilterViewModel = new ArchiveFilterViewModel(_archiveFilter);
            if (ServiceFactory.UserDialogs.ShowModalWindow(archiveFilterViewModel))
            {
                _archiveFilter = archiveFilterViewModel.GetModel();
                IsFilterOn = true;
            }
        }

        public RelayCommand ShowSettingsCommand { get; private set; }
        void OnSettingsCommand()
        {
            var archiveSettingsViewModel = new ArchiveSettingsViewModel(_archiveDefaultState.ArchiveDefaultStateType);
            if (ServiceFactory.UserDialogs.ShowModalWindow(archiveSettingsViewModel))
            {
                var defaultStateType = archiveSettingsViewModel.ArchiveDefaultStates.First(x => x.IsActive).ArchiveDefaultStateType;
                if (defaultStateType != _archiveDefaultState.ArchiveDefaultStateType)
                {
                    _archiveDefaultState.ArchiveDefaultStateType = defaultStateType;
                }
            }
        }
    }
}