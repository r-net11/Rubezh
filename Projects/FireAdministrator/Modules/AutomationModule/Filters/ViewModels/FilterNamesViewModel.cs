using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.ObjectModel;
using GKProcessor;
using System.Windows.Data;
using FiresecAPI.GK;

namespace AutomationModule.ViewModels
{
	public class FilterNamesViewModel : BaseViewModel
	{
		public FilterNamesViewModel()
		{
			BuildTree();

			foreach (var device in AllFilters)
			{
				if (true)
					device.ExpandToThis();
			}
		}

		public List<FilterNameViewModel> AllFilters;

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

		public ObservableCollection<FilterNameViewModel> RootFilters { get; private set; }

		void BuildTree()
		{
			RootFilters = new ObservableCollection<FilterNameViewModel>();
			AllFilters = new List<FilterNameViewModel>();

			var gkViewModel = new FilterNameViewModel("ГК", "");
			RootFilters.Add(gkViewModel);
			foreach (var eventName in EventNameHelper.EventNames)
			{
				var filterNameViewModel = new FilterNameViewModel(eventName.Name, ToGKIconSource(eventName.StateClass));
				gkViewModel.AddChild(filterNameViewModel);
				AllFilters.Add(filterNameViewModel);
			}
		}

		public static string ToGKIconSource(XStateClass stateClass)
		{
			if (stateClass == XStateClass.Norm)
				return null;

			return "/Controls;component/StateClassIcons/" + stateClass.ToString() + ".png";
		}
	}
}