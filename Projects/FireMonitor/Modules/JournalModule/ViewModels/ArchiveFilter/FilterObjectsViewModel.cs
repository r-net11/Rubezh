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
	public class FilterObjectsViewModel : BaseViewModel
	{
		public FilterObjectsViewModel(SKDArchiveFilter filter)
		{
			BuildTree();
			Initialize(filter);
		}

		public void Initialize(SKDArchiveFilter filter)
		{
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
				var filterNameViewModel = AllFilters.FirstOrDefault(x => x.IsSubsystem && x.SubsystemType == subsystemType);
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

		public List<FilterObjectViewModel> AllFilters;

		public ObservableCollection<FilterObjectViewModel> RootFilters { get; private set; }

		FilterObjectViewModel _selectedFilter;
		public FilterObjectViewModel SelectedFilter
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
			RootFilters = new ObservableCollection<FilterObjectViewModel>();
			AllFilters = new List<FilterObjectViewModel>();

			var gkViewModel = new FilterObjectViewModel(GlobalSubsystemType.GK);
			gkViewModel.IsExpanded = true;
			RootFilters.Add(gkViewModel);

			var gkDevicesViewModel = new FilterObjectViewModel(GlobalSubsystemType.GK);
			gkViewModel.AddChild(gkDevicesViewModel);

			foreach (var childDevice in FiresecClient.XManager.DeviceConfiguration.RootDevice.Children)
			{
				AddDeviceInternal(childDevice, gkDevicesViewModel);
			}

			var skdViewModel = new FilterObjectViewModel(GlobalSubsystemType.SKD);
			skdViewModel.IsExpanded = true;
			RootFilters.Add(skdViewModel);
			
		}

		FilterObjectViewModel AddDeviceInternal(FiresecAPI.GK.XDevice device, FilterObjectViewModel parentDeviceViewModel)
		{
			var deviceViewModel = new FilterObjectViewModel(device);
			if (parentDeviceViewModel != null)
				parentDeviceViewModel.AddChild(deviceViewModel);

			foreach (var childDevice in device.Children)
			{
				if (!childDevice.IsNotUsed)
				{
					AddDeviceInternal(childDevice, deviceViewModel);
				}
			}
			return deviceViewModel;
		}
	}
}