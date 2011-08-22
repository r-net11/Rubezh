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

            JournalFilter = new JournalFilter()
            {
                LastRecordsCount = MaxCountRecords,
                LastDaysCount = DefaultDaysCount
            };
        }

        public FilterDetailsViewModel(JournalFilter journalFilter, List<string> existingNames)
        {
            Initialize(existingNames);

            JournalFilter = new JournalFilter()
            {
                Name = journalFilter.Name,
                LastRecordsCount = journalFilter.LastRecordsCount,
                LastDaysCount = journalFilter.LastDaysCount,
                IsLastDaysCountActive = journalFilter.IsLastDaysCountActive
            };

            EventViewModels.Where(
                eventViewModel => journalFilter.Events.Any(
                    x => x == eventViewModel.Id)).All(x => x.IsChecked = true);

            CategoryViewModels.Where(
                categoryViewModel => journalFilter.Categories.Any(
                    x => x == categoryViewModel.Id)).All(x => x.IsChecked = true);
        }

        void Initialize(List<string> existingNames)
        {
            Title = "Настройка представления";

            _existingNames = existingNames;
            if (_existingNames == null)
                _existingNames = new List<string>();

            EventViewModels = new ObservableCollection<EventViewModel>();
            foreach (StateType stateType in Enum.GetValues(typeof(StateType)))
            {
                if (string.IsNullOrEmpty(EnumsConverter.StateTypeToEventName(stateType)) == false)
                    EventViewModels.Add(new EventViewModel(stateType));
            }

            CategoryViewModels = new ObservableCollection<CategoryViewModel>();
            foreach (DeviceCategoryType deviceCategoryType in Enum.GetValues(typeof(DeviceCategoryType)))
            {
                CategoryViewModels.Add(new CategoryViewModel(deviceCategoryType));
            }

            OkCommand = new RelayCommand(OnOk, CanOk);
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

        public ObservableCollection<EventViewModel> EventViewModels { get; private set; }
        public ObservableCollection<CategoryViewModel> CategoryViewModels { get; private set; }

        public JournalFilter GetModel()
        {
            JournalFilter.Events =
                EventViewModels.Where(x => x.IsChecked).Select(x => x.Id).Cast<StateType>().ToList();

            JournalFilter.Categories =
                CategoryViewModels.Where(x => x.IsChecked).Select(x => x.Id).Cast<DeviceCategoryType>().ToList();

            return JournalFilter;
        }

        public RelayCommand OkCommand { get; private set; }
        void OnOk()
        {
            JournalFilter.Name = JournalFilter.Name.Trim();
            Close(true);
        }

        bool CanOk(object obj)
        {
            return this["FilterName"] == null;
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
                if (_existingNames.IsNotNullOrEmpry() &&
                    _existingNames.Any(x => x == name))
                {
                    return "Фильтр с таким именем уже существует";
                }

                return null;
            }
        }
    }
}