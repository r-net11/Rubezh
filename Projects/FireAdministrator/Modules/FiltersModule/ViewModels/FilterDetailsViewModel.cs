using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using Infrastructure.Common;

namespace FiltersModule.ViewModels
{
    public class FilterDetailsViewModel : DialogContent
    {
        public static readonly int DefaultDaysCount = 10;

        public FilterDetailsViewModel()
        {
            Initialize();

            JournalFilter.LastRecordsCount = MaxCountRecords;
            JournalFilter.LastDaysCount = DefaultDaysCount;
        }

        public FilterDetailsViewModel(JournalFilter journalFilter)
        {
            Initialize();

            JournalFilter.Name = journalFilter.Name;
            JournalFilter.LastRecordsCount = journalFilter.LastRecordsCount;
            JournalFilter.LastDaysCount = journalFilter.LastDaysCount;
            JournalFilter.IsLastDaysCountActive = journalFilter.IsLastDaysCountActive;

            foreach (var eventViewModel in EventViewModels)
            {
                if (journalFilter.Events.Any(x => x.Id == eventViewModel.Id))
                {
                    eventViewModel.IsChecked = true;
                }
            }

            foreach (var categoryViewModel in CategoryViewModels)
            {
                if (journalFilter.Categories.Any(x => x.Id == categoryViewModel.Id))
                {
                    categoryViewModel.IsChecked = true;
                }
            }
        }

        void Initialize()
        {
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

            OkCommand = new RelayCommand(OnOk);
            CancelCommand = new RelayCommand(OnCancel);
        }

        public JournalFilter JournalFilter { get; private set; }

        public int MaxCountRecords
        {
            get { return FiresecAPI.Models.JournalFilter.MaxRecordsCount; }
        }

        public ObservableCollection<Event> EventViewModels { get; private set; }
        public ObservableCollection<Category> CategoryViewModels { get; private set; }

        public JournalFilter GetModel()
        {
            JournalFilter.Events = new List<State>();
            foreach (var eventViewModel in EventViewModels)
            {
                if (eventViewModel.IsChecked)
                {
                    JournalFilter.Events.Add(new State() { Id = eventViewModel.Id });
                }
            }

            JournalFilter.Categories = new List<DeviceCategory>();
            foreach (var categoryViewModel in CategoryViewModels)
            {
                if (categoryViewModel.IsChecked)
                {
                    JournalFilter.Categories.Add(new DeviceCategory() { Id = categoryViewModel.Id });
                }
            }

            return JournalFilter;
        }

        public RelayCommand OkCommand { get; private set; }
        void OnOk()
        {
            Close(true);
        }

        public RelayCommand CancelCommand { get; private set; }
        void OnCancel()
        {
            Close(false);
        }
    }
}