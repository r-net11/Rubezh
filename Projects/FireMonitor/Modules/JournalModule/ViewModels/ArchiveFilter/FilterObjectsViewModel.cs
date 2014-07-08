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

			var gkDevicesViewModel = new FilterObjectViewModel(SKDJournalItemType.GKDevice);
			gkViewModel.AddChild(gkDevicesViewModel);
			foreach (var childDevice in FiresecClient.XManager.DeviceConfiguration.RootDevice.Children)
			{
				AddGKDeviceInternal(childDevice, gkDevicesViewModel);
			}

			var gkZonesViewModel = new FilterObjectViewModel(SKDJournalItemType.GKZone);
			gkViewModel.AddChild(gkZonesViewModel);
			foreach (var zone in FiresecClient.XManager.Zones)
			{
				var filterObjectViewModel = new FilterObjectViewModel(zone);
				gkZonesViewModel.AddChild(filterObjectViewModel);
			}

			var gkDirectionsViewModel = new FilterObjectViewModel(SKDJournalItemType.GKDirection);
			gkViewModel.AddChild(gkDirectionsViewModel);
			foreach (var direction in FiresecClient.XManager.Directions)
			{
				var filterObjectViewModel = new FilterObjectViewModel(direction);
				gkDirectionsViewModel.AddChild(filterObjectViewModel);
			}

			var gkMPTsViewModel = new FilterObjectViewModel(SKDJournalItemType.GKMPT);
			gkViewModel.AddChild(gkMPTsViewModel);
			foreach (var mpt in FiresecClient.XManager.MPTs)
			{
				var filterObjectViewModel = new FilterObjectViewModel(mpt);
				gkMPTsViewModel.AddChild(filterObjectViewModel);
			}

			var gkPumpStationsViewModel = new FilterObjectViewModel(SKDJournalItemType.GKPumpStation);
			gkViewModel.AddChild(gkPumpStationsViewModel);
			foreach (var pumpStation in FiresecClient.XManager.PumpStations)
			{
				var filterObjectViewModel = new FilterObjectViewModel(pumpStation);
				gkPumpStationsViewModel.AddChild(filterObjectViewModel);
			}

			var gkDelaysViewModel = new FilterObjectViewModel(SKDJournalItemType.GKDelay);
			gkViewModel.AddChild(gkDelaysViewModel);
			foreach (var delay in FiresecClient.XManager.Delays)
			{
				var filterObjectViewModel = new FilterObjectViewModel(delay);
				gkDelaysViewModel.AddChild(filterObjectViewModel);
			}

			var skdViewModel = new FilterObjectViewModel(GlobalSubsystemType.SKD);
			skdViewModel.IsExpanded = true;
			RootFilters.Add(skdViewModel);

			var skdDevicesViewModel = new FilterObjectViewModel(SKDJournalItemType.SKDDevice);
			skdViewModel.AddChild(skdDevicesViewModel);
			foreach (var childDevice in SKDManager.SKDConfiguration.RootDevice.Children)
			{
				AddSKDDeviceInternal(childDevice, skdDevicesViewModel);
			}

			var skdZonesViewModel = new FilterObjectViewModel(SKDJournalItemType.SKDZone);
			skdViewModel.AddChild(skdZonesViewModel);
			foreach (var zone in SKDManager.Zones)
			{
				var filterObjectViewModel = new FilterObjectViewModel(zone);
				skdZonesViewModel.AddChild(filterObjectViewModel);
			}

			var videoViewModel = new FilterObjectViewModel(GlobalSubsystemType.Video);
			videoViewModel.IsExpanded = true;
			RootFilters.Add(videoViewModel);

			var videoDevicesViewModel = new FilterObjectViewModel(SKDJournalItemType.VideoDevice);
			videoViewModel.AddChild(videoDevicesViewModel);
			foreach (var camera in FiresecClient.FiresecManager.SystemConfiguration.Cameras)
			{
				var filterObjectViewModel = new FilterObjectViewModel(camera);
				videoDevicesViewModel.AddChild(filterObjectViewModel);
			}

		}

		FilterObjectViewModel AddGKDeviceInternal(FiresecAPI.GK.XDevice device, FilterObjectViewModel parentDeviceViewModel)
		{
			var deviceViewModel = new FilterObjectViewModel(device);
			if (parentDeviceViewModel != null)
				parentDeviceViewModel.AddChild(deviceViewModel);

			foreach (var childDevice in device.Children)
			{
				if (!childDevice.IsNotUsed)
				{
					AddGKDeviceInternal(childDevice, deviceViewModel);
				}
			}
			return deviceViewModel;
		}

		FilterObjectViewModel AddSKDDeviceInternal(SKDDevice device, FilterObjectViewModel parentDeviceViewModel)
		{
			var deviceViewModel = new FilterObjectViewModel(device);
			if (parentDeviceViewModel != null)
				parentDeviceViewModel.AddChild(deviceViewModel);

			foreach (var childDevice in device.Children)
			{
				AddSKDDeviceInternal(childDevice, deviceViewModel);
			}
			return deviceViewModel;
		}
	}
}