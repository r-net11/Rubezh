using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure;
using System.Collections.ObjectModel;
using FiresecClient;
using System.Diagnostics;
using System.Threading;
using JournalModule.Views;

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
            int offset = 0;
            while (true)
            {
                List<Firesec.ReadEvents.journalType> journalItems = FiresecManager.ReadJournal(offset * 100, 100);

                if ((journalItems == null) || (journalItems.Count < 100))
                    break;

                foreach (Firesec.ReadEvents.journalType journalItem in journalItems)
                {
                    JournalItemViewModel journalItemViewModel = new JournalItemViewModel(journalItem);
                    Dispatcher.Invoke((Action<JournalItemViewModel>)Add, journalItemViewModel);
                }

                offset++;
                Trace.WriteLine(offset.ToString());
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
            filterViewModel.Initialize();
            FilterView filterView = new FilterView();
            filterView.DataContext = filterViewModel;
            filterView.ShowDialog();
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

        public override void Dispose()
        {
        }
    }
}
