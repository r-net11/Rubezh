﻿using Common;
using FiresecAPI.Journal;
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
			foreach (var journalSubsystemTypes in filter.JournalSubsystemTypes)
			{
				var filterNameViewModel = RootFilters.FirstOrDefault(x => x.IsSubsystem && x.JournalSubsystemType == journalSubsystemTypes);
				if (filterNameViewModel != null)
				{
					filterNameViewModel.IsChecked = true;
				}
			}
		}

		public ArchiveFilter GetModel()
		{
			var filter = new ArchiveFilter();
			foreach (var rootFilter in RootFilters)
			{
				if (rootFilter.IsChecked)
				{
					filter.JournalSubsystemTypes.Add(rootFilter.JournalSubsystemType);
				}
				else
				{
					foreach (var filterViewModel in rootFilter.Children)
					{
						if (filterViewModel.IsChecked)
						{
							filter.JournalEventNameTypes.Add(filterViewModel.JournalEventNameType);
						}
					}
				}
			}
			return filter;
		}

		public ObservableCollection<FilterNameViewModel> RootFilters { get; private set; }

		FilterNameViewModel _selectedFilter;
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

		void BuildTree()
		{
			RootFilters = new ObservableCollection<FilterNameViewModel>
			{
				new FilterNameViewModel(JournalSubsystemType.System) {IsExpanded = true},
				new FilterNameViewModel(JournalSubsystemType.SKD) {IsExpanded = true},
				new FilterNameViewModel(JournalSubsystemType.Video) {IsExpanded = true},
			};

			RootFilters[0].AddChildren(GetEventsByType(JournalSubsystemType.System));
			RootFilters[1].AddChildren(GetEventsByType(JournalSubsystemType.SKD));
			RootFilters[2].AddChildren(GetEventsByType(JournalSubsystemType.Video));
		}
	}
}