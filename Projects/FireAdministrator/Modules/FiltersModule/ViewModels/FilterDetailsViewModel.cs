using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using FiresecAPI.Models;
using Infrastructure.Common;

namespace FiltersModule.ViewModels
{
    public class FilterDetailsViewModel : DialogContent, IDataErrorInfo
    {
        public static readonly int DefaultDaysCount = 10;

        public FilterDetailsViewModel(List<string> existingNames)
        {
            Initialize(existingNames);

            JournalFilter.LastRecordsCount = MaxCountRecords;
            JournalFilter.LastDaysCount = DefaultDaysCount;
        }

        public FilterDetailsViewModel(JournalFilter journalFilter, List<string> existingNames)
        {
            Initialize(existingNames);

            JournalFilter.Name = journalFilter.Name;
            JournalFilter.LastRecordsCount = journalFilter.LastRecordsCount;
            JournalFilter.LastDaysCount = journalFilter.LastDaysCount;
            JournalFilter.IsLastDaysCountActive = journalFilter.IsLastDaysCountActive;

            foreach (var eventViewModel in EventViewModels)
            {
                if (journalFilter.Events.Any(x => (int) x == eventViewModel.Id))
                {
                    eventViewModel.IsChecked = true;
                }
            }

            foreach (var categoryViewModel in CategoryViewModels)
            {
                if (journalFilter.Categories.Any(x => (int) x == categoryViewModel.Id))
                {
                    categoryViewModel.IsChecked = true;
                }
            }
        }

        void Initialize(List<string> existingNames)
        {
            _existingNames = existingNames;
            if (_existingNames == null)
                _existingNames = new List<string>();

            JournalFilter = new JournalFilter();
            Title = "Настройка представления";

            EventViewModels = new ObservableCollection<Event>()
            {
                new Event(0),
                new Event(1),
                new Event(2),
                new Event(3),
                new Event(4),
                new Event(6),
                new Event(7),
            };
            CategoryViewModels = new ObservableCollection<Category>()
            {
                new Category(0),
                new Category(1),
                new Category(2),
                new Category(3),
                new Category(4),
                new Category(5),
                new Category(6),
            };

            OkCommand = new RelayCommand(
                () => OnOk(),
                (x) => this["FilterName"] == null);
            CancelCommand = new RelayCommand(OnCancel);
        }

        public JournalFilter JournalFilter { get; private set; }
        List<string> _existingNames;

        public string FilterName
        {
            get { return JournalFilter.Name; }
            set
            {
                JournalFilter.Name = value;
                OnPropertyChanged("FilterName");
            }
        }

        public int MaxCountRecords
        {
            get { return FiresecAPI.Models.JournalFilter.MaxRecordsCount; }
        }

        public ObservableCollection<Event> EventViewModels { get; private set; }
        public ObservableCollection<Category> CategoryViewModels { get; private set; }

        public JournalFilter GetModel()
        {
            JournalFilter.Events = new List<StateType>();
            foreach (var eventViewModel in EventViewModels)
            {
                if (eventViewModel.IsChecked)
                {
                    JournalFilter.Events.Add((StateType) eventViewModel.Id);
                }
            }

            JournalFilter.Categories = new List<DeviceCategoryType>();
            foreach (var categoryViewModel in CategoryViewModels)
            {
                if (categoryViewModel.IsChecked)
                {
                    JournalFilter.Categories.Add((DeviceCategoryType) categoryViewModel.Id);
                }
            }

            return JournalFilter;
        }

        public RelayCommand OkCommand { get; private set; }
        void OnOk()
        {
            JournalFilter.Name = JournalFilter.Name.Trim();
            Close(true);
        }

        public RelayCommand CancelCommand { get; private set; }
        void OnCancel()
        {
            Close(false);
        }

        public string Error { get { return null; } }

        public string this[string propertyName]
        {
            get
            {
                if (propertyName != "FilterName")
                    throw new ArgumentException();

                if (string.IsNullOrWhiteSpace(FilterName))
                {
                    return "Нужно задать имя";
                }

                var name = FilterName.Trim();
                if (_existingNames.Count > 0 &&
                    _existingNames.Any(x => x == name))
                {
                    return "Фильтр с таким именем уже существует";
                }

                return null;
            }
        }
    }
}