using System;
using Common;
using System.Collections.Generic;
using System.Linq;
using StrazhAPI.SKD.ReportFilters;
using Infrastructure.Common.SKDReports;
using System.Collections.ObjectModel;
using StrazhAPI.Journal;
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

		private IEnumerable<EventTypeViewModel> GetEventsByType(JournalSubsystemType inputType)
		{
			return (Enum.GetValues(typeof (JournalEventNameType))
				.Cast<JournalEventNameType>()
				.Where(x => x != JournalEventNameType.NULL)
				.Where(journalEventNameType => journalEventNameType.GetAttributeOfType<EventNameAttribute>().JournalSubsystemType == inputType)
				.Select(journalEventNameType => new EventTypeViewModel(journalEventNameType)))
				.OrderBy(x => x.Name)
				.ToList();
		}

		private void BuildTree()
		{
			RootFilters = new ObservableCollection<EventTypeViewModel>
			{
				new EventTypeViewModel(JournalSubsystemType.System),
				new EventTypeViewModel(JournalSubsystemType.SKD)
			};

			RootFilters[0].AddChildren(GetEventsByType(JournalSubsystemType.System));
			RootFilters[1].AddChildren(GetEventsByType(JournalSubsystemType.SKD));
		}

		public override void LoadFilter(SKDReportFilter filter)
		{
			var eventFilter = filter as EventsReportFilter;
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
			var eventFilter = filter as EventsReportFilter;
			if (eventFilter == null)
				return;
			eventFilter.JournalEventNameTypes = new List<JournalEventNameType>();
			eventFilter.JournalEventSubsystemTypes = new List<JournalSubsystemType>();
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
