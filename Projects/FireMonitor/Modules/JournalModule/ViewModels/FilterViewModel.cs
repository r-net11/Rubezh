using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using System.Collections.ObjectModel;

namespace JournalModule.ViewModels
{
    public class FilterViewModel : DialogContent
    {
        ObservableCollection<JournalItemViewModel> _journalItems;
        HashSet<string> _allEvents;
        HashSet<string> _allTypes;

        public FilterViewModel()
        {
            Title = "Фильтр журнала";
        }

        public void Initialize(ObservableCollection<JournalItemViewModel> journalItems)
        {
            _journalItems = journalItems;

            _allEvents = new HashSet<string>();
            _allTypes = new HashSet<string>();
            foreach (var journalItem in _journalItems)
            {
                _allEvents.Add(journalItem.Description);
                _allTypes.Add(journalItem.State);
            }

            Events = new ObservableCollection<string>(_allEvents);
            Types = new ObservableCollection<string>(_allTypes);
        }

        ObservableCollection<string> _events;
        public ObservableCollection<string> Events
        {
            get { return _events; }
            set
            {
                _events = value;
                OnPropertyChanged("Events");
            }
        }

        ObservableCollection<string> _types;
        public ObservableCollection<string> Types
        {
            get { return _types; }
            set
            {
                _types = value;
                OnPropertyChanged("Types");
            }
        }
    }
}
