using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
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
            Initialize();
        }

        void Initialize()
        {
            JournalRecords = new ObservableCollection<JournalRecordViewModel>();
            ThreadPool.QueueUserWorkItem(new WaitCallback(Read));
        }

        void Read(Object stateInfo)
        {
            List<JournalRecord> journalRecords = FiresecManager.ReadJournal(0, 300);
            foreach (var journalRecord in journalRecords)
            {
                Dispatcher.Invoke(new Action(
                    () => JournalRecords.Add(new JournalRecordViewModel(journalRecord))), null);
            }
            //Parallel.ForEach(journalRecords, journalRecord =>
            //{
            //Dispatcher.Invoke(new Action(
            //    () => JournalRecords.Add(new JournalRecordViewModel(journalRecord))), null);
            //}
            //);

            return;

            //int lastJournalId = 100;
            //while (true)
            //{
            //    List<JournalRecord> journalRecords = FiresecManager.ReadJournal(0, lastJournalId);

            //    if (journalRecords == null)
            //        break;

            //    Parallel.ForEach(journalRecords, journalItem =>
            //    {
            //        Dispatcher.Invoke((Action<JournalRecordViewModel>) Add, new JournalRecordViewModel(journalItem));
            //    }
            //    );

            //    //if (journalItems.Count < 100)
            //    //    break;

            //    break;

            //    if (journalRecords.Count > 0)
            //        lastJournalId = journalRecords[journalRecords.Count - 1].No - 1;
            //    else
            //        lastJournalId--;
            //}
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
            filterViewModel.Initialize(JournalRecords);
            ServiceFactory.UserDialogs.ShowModalWindow(filterViewModel);
        }
    }
}