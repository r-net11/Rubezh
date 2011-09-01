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
        public ArchiveViewModel()
        {
            JournalRecords = new ObservableCollection<JournalRecordViewModel>(
                FiresecManager.GetFilteredJournal(new JournalFilter() { LastRecordsCount = 100 }).
                Select(journalRecord => new JournalRecordViewModel(journalRecord))
            );

            ShowFilterCommand = new RelayCommand(OnShowFilter);
        }

        bool _isFiltered;
        public bool IsFiltered
        {
            get { return _isFiltered; }
            set
            {
                _isFiltered = value;
                OnPropertyChanged("IsFiltered");
            }
        }

        public ObservableCollection<JournalRecordViewModel> JournalRecords { get; private set; }

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

        public RelayCommand ShowFilterCommand { get; private set; }
        void OnShowFilter()
        {
            var filterViewModel = new ArchiveFilterViewModel(JournalRecords);
            ServiceFactory.UserDialogs.ShowModalWindow(filterViewModel);
        }
    }
}