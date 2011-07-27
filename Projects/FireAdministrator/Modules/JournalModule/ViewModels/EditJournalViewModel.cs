using System.Collections.Generic;
using FiresecClient.Models;
using Infrastructure.Common;

namespace JournalModule.ViewModels
{
    public class EditJournalViewModel : DialogContent
    {
        public static readonly string maxCountRecords = "100";
        public static readonly string DefaultCountDays = "10";

        public EditJournalViewModel()
        {
            JournalViewModel = new JournalViewModel();
            JournalViewModel.LastDaysCount = DefaultCountDays;
            JournalViewModel.LastRecordsCount = maxCountRecords;

            Initialize();
        }

        public EditJournalViewModel(JournalViewModel journal)
        {
            JournalViewModel = new JournalViewModel();
            Helper.CopyContent(journal, JournalViewModel);

            Initialize();
        }

        void Initialize()
        {
            Title = "Настройка представления";

            SaveCommand = new RelayCommand(OnSave);
            CancelCommand = new RelayCommand(OnCancel);
        }

        public JournalViewModel JournalViewModel { get; private set; }

        bool _isAdditionalFilter;
        public bool IsAdditionalFilter
        {
            get { return _isAdditionalFilter; }
            set
            {
                _isAdditionalFilter = value;
                OnPropertyChanged("IsAdditionalFilter");
            }
        }

        public string MaxCountRecords
        {
            get { return maxCountRecords; }
        }

        public string Name
        {
            get { return JournalViewModel.Name; }
            set
            {
                JournalViewModel.Name = value;
                OnPropertyChanged("Name");
            }
        }

        public string LastRecordsCount
        {
            get { return JournalViewModel.LastRecordsCount; }
            set
            {
                JournalViewModel.LastRecordsCount = value;
                OnPropertyChanged("LastRecordsCount");
            }
        }

        public string LastDaysCount
        {
            get { return JournalViewModel.LastDaysCount; }
            set
            {
                JournalViewModel.LastDaysCount = value;
                OnPropertyChanged("LastDaysCount");
            }
        }

        public List<Event> Events
        {
            get { return JournalViewModel.Events; }
        }

        public List<Category> Categories
        {
            get { return JournalViewModel.Categories; }
        }

        public RelayCommand SaveCommand { get; private set; }
        void OnSave()
        {
            if (!IsAdditionalFilter)
            {
                JournalViewModel.LastDaysCount = null;
            }
            Close(true);
        }

        public RelayCommand CancelCommand { get; private set; }
        void OnCancel()
        {
            Close(false);
        }
    }
}
