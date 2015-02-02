using System;
using Common;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.SKD.ReportFilters;
using Infrastructure.Common.SKDReports;
using System.Collections.ObjectModel;
using FiresecAPI.Journal;
using Infrastructure.Common;

namespace SKDModule.Reports.ViewModels
{
    public class EventTypePageViewModel : FilterContainerViewModel
    {
        public EventTypePageViewModel()
        {
            Title = "Типы событий";
            BuildTree();
			SelectAllCommand = new RelayCommand(() => RootFilters.ForEach(item => { item.IsChecked = false; item.IsChecked = true; }));
			SelectNoneCommand = new RelayCommand(() => RootFilters.ForEach(item => { item.IsChecked = true; item.IsChecked = false; }));
		}

		public RelayCommand SelectAllCommand { get; private set; }
		public RelayCommand SelectNoneCommand { get; private set; }
		public ObservableCollection<EventTypeViewModel> RootFilters { get; private set; }
        private void BuildTree()
        {
            RootFilters = new ObservableCollection<EventTypeViewModel>();
            RootFilters.Add(new EventTypeViewModel(JournalSubsystemType.System));
            RootFilters.Add(new EventTypeViewModel(JournalSubsystemType.SKD));
            foreach (JournalEventNameType enumValue in Enum.GetValues(typeof(JournalEventNameType)))
                if (enumValue != JournalEventNameType.NULL)
                {
                    var eventTypeViewModel = new EventTypeViewModel(enumValue);
                    switch (eventTypeViewModel.JournalSubsystemType)
                    {
                        case JournalSubsystemType.System:
                            RootFilters[0].AddChild(eventTypeViewModel);
                            break;
                        case JournalSubsystemType.SKD:
                            RootFilters[1].AddChild(eventTypeViewModel);
                            break;
                    }
                }
        }

        public override void LoadFilter(SKDReportFilter filter)
        {
            var eventFilter = filter as ReportFilter401;
            if (eventFilter == null)
                return;
            RootFilters.ForEach(item =>
            {
                item.IsExpanded = true;
                item.IsChecked = false;
                item.Children.ForEach(child => child.IsChecked = false);
            });
            foreach (var eventName in eventFilter.JournalEventNameTypes)
            {
                var eventTypeViewModel = RootFilters.SelectMany(item => item.Children).FirstOrDefault(x => x.JournalEventNameType == eventName);
                if (eventTypeViewModel != null)
                    eventTypeViewModel.IsChecked = true;
            }
            foreach (var journalSubsystemTypes in eventFilter.JournalEventSubsystemTypes)
            {
                var eventTypeViewModel = RootFilters.FirstOrDefault(x => x.IsSubsystem && x.JournalSubsystemType == journalSubsystemTypes);
                if (eventTypeViewModel != null)
                    eventTypeViewModel.IsChecked = true;
            }
        }
        public override void UpdateFilter(SKDReportFilter filter)
        {
            var eventFilter = filter as ReportFilter401;
            if (eventFilter == null)
                return;
            eventFilter.JournalEventNameTypes = new List<JournalEventNameType>();
            foreach (var rootFilter in RootFilters)
                if (rootFilter.IsChecked)
                    eventFilter.JournalEventSubsystemTypes.Add(rootFilter.JournalSubsystemType);
                else
                    foreach (var filterViewModel in rootFilter.Children)
                        if (filterViewModel.IsChecked)
                            eventFilter.JournalEventNameTypes.Add(filterViewModel.JournalEventNameType);
        }
    }
}
