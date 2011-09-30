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
        static public readonly DateTime ArchiveFirstDate = FiresecManager.GetArchiveStartDate();
        ArchiveDefaultState _archiveDefaultState;
        ArchiveFilter _archiveFilter;

        public ArchiveViewModel()
        {
            _archiveDefaultState = ClientSettings.ArchiveDefaultState;
            IsFilterOn = false;

            ShowFilterCommand = new RelayCommand(OnShowFilter);
            ShowSettingsCommand = new RelayCommand(OnShowSettings);
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

        public bool IsFilterExists
        {
            get { return _archiveFilter != null; }
        }

        ArchiveFilter GerFilterFromDefaultState(ArchiveDefaultState archiveDefaultState)
        {
            var filter = new ArchiveFilter() { StartDate = ArchiveFirstDate, EndDate = DateTime.Now };
            switch (archiveDefaultState.ArchiveDefaultStateType)
            {
                case ArchiveDefaultStateType.LastHours:
                    if (archiveDefaultState.Count.HasValue)
                        filter.StartDate = filter.EndDate.AddHours(-archiveDefaultState.Count.Value);
                    break;

                case ArchiveDefaultStateType.LastDays:
                    if (archiveDefaultState.Count.HasValue)
                        filter.StartDate = filter.EndDate.AddDays(-archiveDefaultState.Count.Value);
                    break;

                case ArchiveDefaultStateType.FromDate:
                    if (archiveDefaultState.StartDate.HasValue)
                        filter.StartDate = archiveDefaultState.StartDate.Value;
                    break;

                case ArchiveDefaultStateType.RangeDate:
                    if (archiveDefaultState.StartDate.HasValue)
                        filter.StartDate = archiveDefaultState.StartDate.Value;
                    if (archiveDefaultState.EndDate.HasValue)
                        filter.EndDate = archiveDefaultState.EndDate.Value;
                    break;

                case ArchiveDefaultStateType.All:
                default:
                    break;
            }

            return filter;
        }

        void SetDefaultArchiveContent()
        {
            JournalRecords = new ObservableCollection<JournalRecordViewModel>(
                FiresecManager.GetFilteredArchive(GerFilterFromDefaultState(_archiveDefaultState)).
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
            ArchiveFilterViewModel archiveFilterViewModel = null;
            if (_archiveFilter != null)
                archiveFilterViewModel = new ArchiveFilterViewModel(_archiveFilter);
            else
                archiveFilterViewModel = new ArchiveFilterViewModel(GerFilterFromDefaultState(_archiveDefaultState));

            if (ServiceFactory.UserDialogs.ShowModalWindow(archiveFilterViewModel))
            {
                _archiveFilter = archiveFilterViewModel.GetModel();
                OnPropertyChanged("IsFilterExists");
                IsFilterOn = true;
            }
        }

        public RelayCommand ShowSettingsCommand { get; private set; }
        void OnShowSettings()
        {
            var archiveSettingsViewModel = new ArchiveSettingsViewModel(_archiveDefaultState);
            if (ServiceFactory.UserDialogs.ShowModalWindow(archiveSettingsViewModel))
            {
                _archiveDefaultState = archiveSettingsViewModel.GetModel();
                ClientSettings.ArchiveDefaultState = _archiveDefaultState;
                if (IsFilterOn == false)
                    IsFilterOn = false;
            }
        }
    }
}