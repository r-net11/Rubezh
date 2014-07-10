using System;
using System.Linq;
using System.Collections.Generic;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.ObjectModel;
using FiresecAPI.Journal;
using FiresecAPI.Automation;
using FiresecAPI.Models;
using FiresecAPI.SKD;

namespace JournalModule.ViewModels
{
	public class FilterNamesViewModel : BaseViewModel
	{
		public FilterNamesViewModel(SKDArchiveFilter filter)
		{
			BuildTree();
			Initialize(filter);
		}

		public void Initialize(SKDArchiveFilter filter)
		{
			AllFilters.ForEach(x => x.IsChecked = false);
			foreach (var eventName in filter.JournalEventNameTypes)
			{
				var filterNameViewModel = AllFilters.FirstOrDefault(x => x.JournalEventNameType == eventName);
				if (filterNameViewModel != null)
				{
					filterNameViewModel.IsChecked = true;
				}
			}
			foreach (var journalSubsystemTypes in filter.JournalSubsystemTypes)
			{
				var filterNameViewModel = RootFilters.FirstOrDefault(x => x.IsSubsystem && x.JournalSubsystemType == journalSubsystemTypes);
				if (filterNameViewModel != null)
				{
					filterNameViewModel.IsChecked = true;
				}
			}
		}

		public SKDArchiveFilter GetModel()
		{
			var filter = new SKDArchiveFilter();
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

		public List<FilterNameViewModel> AllFilters;

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

		void BuildTree()
		{
			RootFilters = new ObservableCollection<FilterNameViewModel>();
			AllFilters = new List<FilterNameViewModel>();

			var systemViewModel = new FilterNameViewModel(JournalSubsystemType.System);
			systemViewModel.IsExpanded = true;
			RootFilters.Add(systemViewModel);

			var gkViewModel = new FilterNameViewModel(JournalSubsystemType.GK);
			gkViewModel.IsExpanded = true;
			RootFilters.Add(gkViewModel);

			var skdViewModel = new FilterNameViewModel(JournalSubsystemType.SKD);
			skdViewModel.IsExpanded = true;
			RootFilters.Add(skdViewModel);

			foreach (JournalEventNameType enumValue in Enum.GetValues(typeof(JournalEventNameType)))
			{
				var filterNameViewModel = new FilterNameViewModel(enumValue);
				if (filterNameViewModel.JournalEventNameType == JournalEventNameType.NULL)
					continue;

				AllFilters.Add(filterNameViewModel);

				switch (filterNameViewModel.JournalSubsystemType)
				{
					case JournalSubsystemType.System:
						systemViewModel.AddChild(filterNameViewModel);
						break;

					case JournalSubsystemType.GK:
						gkViewModel.AddChild(filterNameViewModel);
						break;

					case JournalSubsystemType.SKD:
						skdViewModel.AddChild(filterNameViewModel);
						break;
				}
			}
		}
	}
}