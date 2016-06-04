using Common;
using StrazhAPI.Journal;
using Infrastructure.Common.TreeList;
using Infrastructure.Common.Windows.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace JournalModule.ViewModels
{
	public class FilterNamesViewModel : BaseViewModel
	{
		public FilterNamesViewModel(ArchiveFilter filter)
		{
			BuildTree();
			Initialize(filter);
		}

		public void Initialize(ArchiveFilter filter)
		{
			// Тип события
			var allJournalEventNameTypes = new List<FilterNameViewModel>();
			foreach (var rootFilter in RootFilters)
			{
				allJournalEventNameTypes.AddRange(rootFilter.Children.ToList());
			}
			foreach (var journalEventNameType in filter.JournalEventNameTypes)
			{
				var filterNameViewModel = allJournalEventNameTypes.FirstOrDefault(x => x.JournalEventNameType == journalEventNameType);
				if (filterNameViewModel != null)
				{
					filterNameViewModel.IsChecked = true;
					ExpandParent(filterNameViewModel);
				}
			}
		}

		public ArchiveFilter GetModel()
		{
			var filter = new ArchiveFilter();
			foreach (var rootFilter in RootFilters)
			{
				foreach (var filterViewModel in rootFilter.Children)
				{
					if (filterViewModel.IsChecked)
						filter.JournalEventNameTypes.Add(filterViewModel.JournalEventNameType);
				}
			}
			return filter;
		}

		public ObservableCollection<FilterNameViewModel> RootFilters { get; private set; }

		private FilterNameViewModel _selectedFilter;
		public FilterNameViewModel SelectedFilter
		{
			get { return _selectedFilter; }
			set
			{
				_selectedFilter = value;
				OnPropertyChanged(() => SelectedFilter);
			}
		}

		private IEnumerable<FilterNameViewModel> GetEventsByType(JournalSubsystemType inputType)
		{
			return (Enum.GetValues(typeof(JournalEventNameType))
				.Cast<JournalEventNameType>()
				.Where(x => x != JournalEventNameType.NULL)
				.Where(journalEventNameType => journalEventNameType.GetAttributeOfType<EventNameAttribute>().JournalSubsystemType == inputType)
				.Select(journalEventNameType => new FilterNameViewModel(journalEventNameType)))
				.OrderBy(x => x.Name)
				.ToList();
		}

		private void BuildTree()
		{
			RootFilters = new ObservableCollection<FilterNameViewModel>
			{
				new FilterNameViewModel(JournalSubsystemType.System),
				new FilterNameViewModel(JournalSubsystemType.SKD),
				new FilterNameViewModel(JournalSubsystemType.Video),
			};

			RootFilters[0].AddChildren(GetEventsByType(JournalSubsystemType.System));
			RootFilters[1].AddChildren(GetEventsByType(JournalSubsystemType.SKD));
			RootFilters[2].AddChildren(GetEventsByType(JournalSubsystemType.Video));
		}

		private void ExpandParent(TreeNodeViewModel<FilterNameViewModel> child)
		{
			if (child.Parent == null)
				return;

			child.Parent.IsExpanded = true;
			ExpandParent(child.Parent);
		}
	}
}