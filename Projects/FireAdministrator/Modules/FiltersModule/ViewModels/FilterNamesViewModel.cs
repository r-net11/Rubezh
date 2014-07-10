using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Journal;
using Infrastructure.Common.Windows.ViewModels;

namespace FiltersModule.ViewModels
{
	public class FilterNamesViewModel : BaseViewModel
	{
		public FilterNamesViewModel(JournalFilter filter)
		{
			BuildTree();
			foreach (var journalEventNameType in filter.JournalEventNameTypes)
			{
				var filterNameViewModel = AllFilters.FirstOrDefault(x => x.JournalEventNameType == journalEventNameType);
				if (filterNameViewModel != null)
				{
					filterNameViewModel.IsChecked = true;
				}
			}
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