using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using Infrastructure.Common;

namespace FiltersModule.ViewModels
{
    public class FilterDetailsViewModel : DialogContent
    {
        public static readonly string maxCountRecords = "100";
        public static readonly string DefaultDaysCount = "10";

        public FilterDetailsViewModel()
        {
            Initialize();

            JournalFilter = new JournalFilter();
            JournalFilter.LastRecordsCount = maxCountRecords;
            JournalFilter.LastDaysCount = DefaultDaysCount;

        }

        public FilterDetailsViewModel(JournalFilter journalFilter)
        {
            JournalFilter = journalFilter;
            Initialize();

            if (JournalFilter.LastDaysCount != null)
            {
                IsAdditionalFilter = true;
            }
            else
            {
                JournalFilter.LastDaysCount = DefaultDaysCount;
            }

            for (var i = 0; i < EventViewModels.Count; ++i)
            {
                if (JournalFilter.Events.Any(x => x.Id == EventViewModels[i].Id))
                {
                    EventViewModels[i].IsChecked = true;
                }
            }

            for (var i = 0; i < CategoryViewModels.Count; ++i)
            {
                if (JournalFilter.Categories.Any(x => x.Id == CategoryViewModels[i].Id))
                {
                    CategoryViewModels[i].IsChecked = true;
                }
            }
        }

        void Initialize()
        {
            Title = "Настройка представления";

            EventViewModels = new ObservableCollection<EventViewModel>()
            {
                new EventViewModel(0),
                new EventViewModel(1),
                new EventViewModel(2),
                new EventViewModel(3),
                new EventViewModel(4),
                new EventViewModel(6),
                new EventViewModel(7),
            };

            CategoryViewModels = new ObservableCollection<CategoryViewModel>()
            {
                new CategoryViewModel(0),
                new CategoryViewModel(1),
                new CategoryViewModel(2),
                new CategoryViewModel(3),
                new CategoryViewModel(4),
                new CategoryViewModel(5),
                new CategoryViewModel(6),
            };

            OkCommand = new RelayCommand(OnOk);
            CancelCommand = new RelayCommand(OnCancel);
        }

        public JournalFilter JournalFilter { get; private set; }

        public bool IsAdditionalFilter { get; set; }

        public string MaxCountRecords
        {
            get { return maxCountRecords; }
        }

        public ObservableCollection<EventViewModel> EventViewModels { get; private set; }
        public ObservableCollection<CategoryViewModel> CategoryViewModels { get; private set; }

        public JournalFilter GetModel()
        {
            JournalFilter.Events = new System.Collections.Generic.List<State>();
            foreach (var eventViewModel in EventViewModels)
            {
                if (eventViewModel.IsChecked)
                {
                    JournalFilter.Events.Add(new State() { Id = eventViewModel.Id });
                }
            }

            JournalFilter.Categories = new System.Collections.Generic.List<DeviceCategory>();
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
            if (!IsAdditionalFilter)
            {
                JournalFilter.LastDaysCount = null;
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
