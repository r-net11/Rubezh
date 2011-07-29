using System.Collections.Generic;
using System.Text;
using Infrastructure.Common;
using FiresecAPI.Models;

namespace JournalModule.ViewModels
{
    public class JournalViewModel : BaseViewModel
    {
        public JournalViewModel()
        {
            Journal = new JournalFilter();
            Initialize();
        }

        public JournalViewModel(JournalFilter journal)
        {
            Journal = journal;
            Initialize();
        }

        void Initialize()
        {
            SetEnabledEvents();
            SetEnabledCategories();
            SetRecordsConditions();
        }

        public JournalFilter Journal { get; private set; }

        public string Name
        {
            get { return Journal.Name; }
            set
            {
                Journal.Name = value;
                OnPropertyChanged("Name");
            }
        }

        public string LastRecordsCount
        {
            get { return Journal.LastRecordsCount; }
            set
            {
                Journal.LastRecordsCount = value;
                OnPropertyChanged("LastRecordsCount");
            }
        }

        public string LastDaysCount
        {
            get { return Journal.LastDaysCount; }
            set
            {
                Journal.LastDaysCount = value;
                OnPropertyChanged("LastRecordsCount");
            }
        }

        public List<Event> Events
        {
            get { return Journal.Events; }
        }

        public List<Category> Categories
        {
            get { return Journal.Categories; }
        }

        string _enableEvents;
        public string EnableEvents
        {
            get { return _enableEvents; }
            private set
            {
                _enableEvents = value;
                OnPropertyChanged("EnableEvents");
            }
        }

        string _enableCategories;
        public string EnableCategories
        {
            get { return _enableCategories; }
            private set
            {
                _enableCategories = value;
                OnPropertyChanged("EnableCategories");
            }
        }

        string _recordsConditions;
        public string RecordsConditions
        {
            get { return _recordsConditions; }
            private set
            {
                _recordsConditions = value;
                OnPropertyChanged("RecordsConditions");
            }
        }

        void SetEnabledEvents()
        {
            StringBuilder enabledEvents = new StringBuilder();
            foreach (var eventViewModel in Events)
            {
                if (eventViewModel.IsEnable)
                {
                    if (enabledEvents.Length != 0) enabledEvents.Append(" или ");
                    enabledEvents.Append(eventViewModel.Name);

                }
            }
            EnableEvents = enabledEvents.ToString();
        }

        void SetEnabledCategories()
        {
            StringBuilder enabledCategories = new StringBuilder();
            foreach (var categoryViewModel in Categories)
            {
                if (categoryViewModel.IsEnable)
                {
                    if (enabledCategories.Length != 0) enabledCategories.Append(" или ");
                    enabledCategories.Append(categoryViewModel.Name);
                }
            }
            EnableCategories = enabledCategories.ToString();
        }

        void SetRecordsConditions()
        {
            StringBuilder recordsConditions = new StringBuilder();
            recordsConditions.Append(LastRecordsCount);
            recordsConditions.Append(" последних");
            if (LastDaysCount != null)
            {
                recordsConditions.Append(" за ");
                recordsConditions.Append(LastDaysCount);
                recordsConditions.Append(" дней");
            }
            RecordsConditions = recordsConditions.ToString();
        }

        public void UpdateProperties()
        {
            SetEnabledEvents();
            SetEnabledCategories();
            SetRecordsConditions();
        }
    }
}
