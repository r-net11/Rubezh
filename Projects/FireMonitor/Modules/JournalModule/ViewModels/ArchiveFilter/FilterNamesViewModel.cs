using System;
using System.Linq;
using System.Collections.Generic;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.ObjectModel;
using FiresecAPI.Events;
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
			foreach (var eventName in filter.EventNames)
			{
				var filterNameViewModel = AllFilters.FirstOrDefault(x => x.GlobalEventNameEnum == eventName);
				if (filterNameViewModel != null)
				{
					filterNameViewModel.IsChecked = true;
				}
			}
			foreach (var subsystemType in filter.SubsystemTypes)
			{
				var filterNameViewModel = RootFilters.FirstOrDefault(x => x.IsSubsystem && x.SubsystemType == subsystemType);
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
					filter.SubsystemTypes.Add(rootFilter.SubsystemType);
				}
				else
				{
					foreach (var filterViewModel in rootFilter.Children)
					{
						if (filterViewModel.IsChecked)
						{
							filter.EventNames.Add(filterViewModel.GlobalEventNameEnum);
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
				if (filterNameViewModel.GlobalEventNameEnum == GlobalEventNameEnum.NULL)
					continue;

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