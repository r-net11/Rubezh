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
            JournalItems = new ObservableCollection<JournalItemViewModel>();

            Thread thread = new Thread(Read);
            thread.Priority = ThreadPriority.Highest;
            thread.Start();
        }

        void Read()
        {
            int lastJournalId = 0;
            lastJournalId = 100000;
            while (true)
            {
                List<JournalItem> journalItems = FiresecManager.ReadJournal(0, 100000);

                if (journalItems == null)
                    break;

                Parallel.ForEach(journalItems, journalItem =>
                {
                    Dispatcher.Invoke((Action<JournalItemViewModel>) Add, new JournalItemViewModel(journalItem));
                }
                );

                //if (journalItems.Count < 100)
                //    break;

                break;

                if (journalItems.Count > 0)
                    lastJournalId = journalItems[journalItems.Count - 1].No - 1;
                else
                    lastJournalId--;
            }
        }

        void Add(JournalItemViewModel journalItemViewModel)
        {
            JournalItems.Add(journalItemViewModel);
        }

        ObservableCollection<JournalItemViewModel> _journalItems;
        public ObservableCollection<JournalItemViewModel> JournalItems
        {
            get { return _journalItems; }
            set
            {
                _journalItems = value;
                OnPropertyChanged("JournalItems");
            }
        }

        JournalItemViewModel _selectedItem;
        public JournalItemViewModel SelectedItem
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
            FilterViewModel filterViewModel = new FilterViewModel();
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
