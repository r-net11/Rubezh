using System;
using System.Linq;
using System.Collections.Generic;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.ObjectModel;
using FiresecAPI.Events;
using FiresecAPI.Automation;
using FiresecAPI.Models;

namespace FiltersModule.ViewModels
{
	public class FilterNamesViewModel : BaseViewModel
	{
		public FilterNamesViewModel(JournalFilter filter)
		{
			BuildTree();
			foreach (var eventName in filter.EventNames)
			{
				var filterNameViewModel = AllFilters.FirstOrDefault(x => x.GlobalEventNameEnum == eventName);
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

			var systemViewModel = new FilterNameViewModel(GlobalSubsystemType.System);
			systemViewModel.IsExpanded = true;
			RootFilters.Add(systemViewModel);

			var gkViewModel = new FilterNameViewModel(GlobalSubsystemType.GK);
			gkViewModel.IsExpanded = true;
			RootFilters.Add(gkViewModel);

			var skdViewModel = new FilterNameViewModel(GlobalSubsystemType.SKD);
			skdViewModel.IsExpanded = true;
			RootFilters.Add(skdViewModel);

			foreach (GlobalEventNameEnum enumValue in Enum.GetValues(typeof(GlobalEventNameEnum)))
			{
				var filterNameViewModel = new FilterNameViewModel(enumValue);
				AllFilters.Add(filterNameViewModel);

				switch (filterNameViewModel.SubsystemType)
				{
					case GlobalSubsystemType.System:
						systemViewModel.AddChild(filterNameViewModel);
						break;

					case GlobalSubsystemType.GK:
						gkViewModel.AddChild(filterNameViewModel);
						break;

					case GlobalSubsystemType.SKD:
						skdViewModel.AddChild(filterNameViewModel);
						break;
				}
			}
		}
	}
}