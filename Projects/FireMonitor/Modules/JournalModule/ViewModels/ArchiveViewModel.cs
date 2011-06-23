using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using System.Collections.ObjectModel;
using FiresecClient;
using System.Diagnostics;
using System.Threading;
using JournalModule.Views;
using Infrastructure;

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
            thread.Start();
        }

        void Read()
        {
            int lastJournalId = 0;
            lastJournalId = 2510;
            lastJournalId = 1000;
            while (true)
            {
                List<Firesec.ReadEvents.journalType> journalItems = FiresecManager.ReadJournal(0, 10000);

                if (journalItems == null)
                    break;

                foreach (var journalItem in journalItems)
                {
                    JournalItemViewModel journalItemViewModel = new JournalItemViewModel(journalItem);
                    Dispatcher.Invoke((Action<JournalItemViewModel>)Add, journalItemViewModel);
                }

                //if (journalItems.Count < 100)
                //    break;

                break;

                if (journalItems.Count > 0)
                    lastJournalId = Convert.ToInt32(journalItems[journalItems.Count - 1].IDEvents) - 1;
                else
                    lastJournalId--;
                Trace.WriteLine(lastJournalId.ToString());
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
