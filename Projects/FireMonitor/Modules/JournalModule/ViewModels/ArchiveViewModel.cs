using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
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
            ShowFilterCommand = new RelayCommand(OnShowFilter);
        }

        public void Initialize()
        {
            JournalItems = new ObservableCollection<JournalRecordViewModel>();

            Thread thread = new Thread(Read);
            thread.Priority = ThreadPriority.Highest;
            thread.Start();
        }

        void Read()
        {
            int lastJournalId = 100000;
            while (true)
            {
                List<JournalRecord> journalRecords = FiresecManager.ReadJournal(0, 1000);

                if (journalRecords == null)
                    break;

                Parallel.ForEach(journalRecords, journalItem =>
                {
                    Dispatcher.Invoke((Action<JournalRecordViewModel>) Add, new JournalRecordViewModel(journalItem));
                }
                );

                //if (journalItems.Count < 100)
                //    break;

                break;

                if (journalRecords.Count > 0)
                    lastJournalId = journalRecords[journalRecords.Count - 1].No - 1;
                else
                    lastJournalId--;
            }
        }

        void Add(JournalRecordViewModel journalItemViewModel)
        {
            JournalItems.Add(journalItemViewModel);
        }

        ObservableCollection<JournalRecordViewModel> _journalItems;
        public ObservableCollection<JournalRecordViewModel> JournalItems
        {
            get { return _journalItems; }
            set
            {
                _journalItems = value;
                OnPropertyChanged("JournalItems");
            }
        }

        JournalRecordViewModel _selectedItem;
        public JournalRecordViewModel SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged("SelectedItem");
            }
        }

        public RelayCommand ShowFilterCommand { get; private set; }
        void OnShowFilter()
        {
            ArchiveFilterViewModel filterViewModel = new ArchiveFilterViewModel();
            filterViewModel.Initialize(JournalItems);
            ServiceFactory.UserDialogs.ShowModalWindow(filterViewModel);
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
    }
}
