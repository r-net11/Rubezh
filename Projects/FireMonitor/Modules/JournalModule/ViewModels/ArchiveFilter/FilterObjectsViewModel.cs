using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.GK;
using FiresecAPI.Journal;
using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace JournalModule.ViewModels
{
	public class FilterObjectsViewModel : BaseViewModel
	{
		public FilterObjectsViewModel(ArchiveFilter filter)
		{
			BuildTree();
			Initialize(filter);
		}

		public void Initialize(ArchiveFilter filter)
		{
			AllFilters.ForEach(x => x.SetIsChecked(false));
			foreach (var subsystemType in filter.JournalSubsystemTypes)
			{
				var filterNameViewModel = RootFilters.FirstOrDefault(x => x.IsSubsystem && x.JournalSubsystemType == subsystemType);
				if (filterNameViewModel != null)
				{
					filterNameViewModel.IsChecked = true;
				}
			}
			foreach (var journalObjectType in filter.JournalObjectTypes)
			{
				var filterNameViewModel = AllFilters.FirstOrDefault(x => x.IsObjectGroup && x.JournalObjectType == journalObjectType);
				if (filterNameViewModel != null)
				{
					filterNameViewModel.IsChecked = true;
				}
			}
			foreach (var uid in filter.ObjectUIDs)
			{
				if (uid != Guid.Empty)
				{
					var filterNameViewModel = AllFilters.FirstOrDefault(x => x.UID == uid);
					if (filterNameViewModel != null)
						filterNameViewModel.IsChecked = true;
				}
			}
		}

		public ArchiveFilter GetModel()
		{
			var filter = new ArchiveFilter();
			foreach (var subsystemFilter in RootFilters)
			{
				if (subsystemFilter.IsChecked)
				{
					filter.JournalSubsystemTypes.Add(subsystemFilter.JournalSubsystemType);
				}
				else
				{
					foreach (var objectTypeFilter in subsystemFilter.Children)
					{
						if (objectTypeFilter.IsChecked)
						{
							filter.JournalObjectTypes.Add(objectTypeFilter.JournalObjectType);
						}
						else
						{
							foreach (var filterViewModel in objectTypeFilter.GetAllChildren())
							{
								if (filterViewModel.IsChecked && filterViewModel.UID != Guid.Empty)
								{
									filter.ObjectUIDs.Add(filterViewModel.UID);
								}
							}
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

			var gkViewModel = new FilterObjectViewModel(JournalSubsystemType.GK);
			gkViewModel.IsExpanded = true;
			RootFilters.Add(gkViewModel);

			var gkDevicesViewModel = new FilterObjectViewModel(JournalObjectType.GKDevice);
			AddChild(gkViewModel, gkDevicesViewModel);
			foreach (var childDevice in FiresecClient.XManager.DeviceConfiguration.RootDevice.Children)
			{
				AddGKDeviceInternal(childDevice, gkDevicesViewModel);
			}

			var gkZonesViewModel = new FilterObjectViewModel(JournalObjectType.GKZone);
			AddChild(gkViewModel, gkZonesViewModel);
			foreach (var zone in FiresecClient.XManager.Zones)
			{
				var filterObjectViewModel = new FilterObjectViewModel(zone);
				AddChild(gkZonesViewModel, filterObjectViewModel);
			}

			var gkDirectionsViewModel = new FilterObjectViewModel(JournalObjectType.GKDirection);
			AddChild(gkViewModel, gkDirectionsViewModel);
			foreach (var direction in FiresecClient.XManager.Directions)
			{
				var filterObjectViewModel = new FilterObjectViewModel(direction);
				AddChild(gkDirectionsViewModel, filterObjectViewModel);
			}

			var gkMPTsViewModel = new FilterObjectViewModel(JournalObjectType.GKMPT);
			AddChild(gkViewModel, gkMPTsViewModel);
			foreach (var mpt in FiresecClient.XManager.MPTs)
			{
				var filterObjectViewModel = new FilterObjectViewModel(mpt);
				AddChild(gkMPTsViewModel, filterObjectViewModel);
			}

			var gkPumpStationsViewModel = new FilterObjectViewModel(JournalObjectType.GKPumpStation);
			AddChild(gkViewModel, gkPumpStationsViewModel);
			foreach (var pumpStation in FiresecClient.XManager.PumpStations)
			{
				var filterObjectViewModel = new FilterObjectViewModel(pumpStation);
				AddChild(gkPumpStationsViewModel, filterObjectViewModel);
			}

			var gkDelaysViewModel = new FilterObjectViewModel(JournalObjectType.GKDelay);
			AddChild(gkViewModel, gkDelaysViewModel);
			foreach (var delay in FiresecClient.XManager.Delays)
			{
				var filterObjectViewModel = new FilterObjectViewModel(delay);
				AddChild(gkDelaysViewModel, filterObjectViewModel);
			}

			var skdViewModel = new FilterObjectViewModel(JournalSubsystemType.SKD);
			skdViewModel.IsExpanded = true;
			RootFilters.Add(skdViewModel);

			var skdDevicesViewModel = new FilterObjectViewModel(JournalObjectType.SKDDevice);
			AddChild(skdViewModel, skdDevicesViewModel);
			foreach (var childDevice in SKDManager.SKDConfiguration.RootDevice.Children)
			{
				AddSKDDeviceInternal(childDevice, skdDevicesViewModel);
			}

			var skdZonesViewModel = new FilterObjectViewModel(JournalObjectType.SKDZone);
			AddChild(skdViewModel, skdZonesViewModel);
			foreach (var zone in SKDManager.Zones)
			{
				var filterObjectViewModel = new FilterObjectViewModel(zone);
				AddChild(skdZonesViewModel, filterObjectViewModel);
			}

			var videoViewModel = new FilterObjectViewModel(JournalSubsystemType.Video);
			videoViewModel.IsExpanded = true;
			RootFilters.Add(videoViewModel);

			var videoDevicesViewModel = new FilterObjectViewModel(JournalObjectType.VideoDevice);
			AddChild(videoViewModel, videoDevicesViewModel);
			foreach (var camera in FiresecClient.FiresecManager.SystemConfiguration.Cameras)
			{
				var filterObjectViewModel = new FilterObjectViewModel(camera);
				AddChild(videoDevicesViewModel, filterObjectViewModel);
			}
		}

		FilterObjectViewModel AddGKDeviceInternal(XDevice device, FilterObjectViewModel parentDeviceViewModel)
		{
			var deviceViewModel = new FilterObjectViewModel(device);
			if (parentDeviceViewModel != null)
				AddChild(parentDeviceViewModel, deviceViewModel);

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
				AddChild(parentDeviceViewModel, deviceViewModel);

			foreach (var childDevice in device.Children)
			{
				AddSKDDeviceInternal(childDevice, deviceViewModel);
			}
			return deviceViewModel;
		}

		void AddChild(FilterObjectViewModel parentDeviceViewModel, FilterObjectViewModel childDeviceViewModel)
		{
			parentDeviceViewModel.AddChild(childDeviceViewModel);
			AllFilters.Add(childDeviceViewModel);
		}
	}
}