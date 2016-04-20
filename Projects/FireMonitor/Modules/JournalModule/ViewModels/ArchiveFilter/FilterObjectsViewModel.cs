using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.GK;
using RubezhAPI.Journal;
using RubezhAPI.SKD;
using Infrastructure.Common.Windows.Windows.ViewModels;
using Infrastructure.Common.Windows;
using RubezhClient;
using RubezhAPI;

namespace JournalModule.ViewModels
{
	public class FilterObjectsViewModel : BaseViewModel
	{
		public FilterObjectsViewModel(JournalFilter filter)
		{
			BuildTree();
			Initialize(filter);
		}

		public void Initialize(JournalFilter filter)
		{
			AllFilters.ForEach(x => x.IsChecked = false);

			foreach (var journalObjectType in filter.JournalObjectTypes)
			{
				var filterNameViewModel = AllFilters.FirstOrDefault(x => x.FilterObjectType == FilterObjectType.ObjectType && x.JournalObjectType == journalObjectType);
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
					{
						filterNameViewModel.IsChecked = true;
						filterNameViewModel.ExpandToThis();
					}
				}
			}
		}

		public JournalFilter GetModel()
		{
			var filter = new JournalFilter();
			foreach (var filterObject in RootFilters.SelectMany(x => x.GetAllChildren()).Where(x => x.IsChecked))
			{
				switch (filterObject.FilterObjectType)
				{
					case FilterObjectType.ObjectType:
						filter.JournalObjectTypes.Add(filterObject.JournalObjectType);
						break;
					case FilterObjectType.Object:
					case FilterObjectType.Camera:
						filter.ObjectUIDs.Add(filterObject.UID);
						break;
					case FilterObjectType.Subsystem:	
					default:
						break;
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
			foreach (var childDevice in GKManager.DeviceConfiguration.RootDevice.Children)
			{
				AddGKDeviceInternal(childDevice, gkDevicesViewModel);
			}

			var gkZonesViewModel = new FilterObjectViewModel(JournalObjectType.GKZone);
			AddChild(gkViewModel, gkZonesViewModel);
			foreach (var zone in GKManager.Zones)
			{
				var filterObjectViewModel = new FilterObjectViewModel(zone);
				AddChild(gkZonesViewModel, filterObjectViewModel);
			}

			var gkDirectionsViewModel = new FilterObjectViewModel(JournalObjectType.GKDirection);
			AddChild(gkViewModel, gkDirectionsViewModel);
			foreach (var direction in GKManager.Directions)
			{
				var filterObjectViewModel = new FilterObjectViewModel(direction);
				AddChild(gkDirectionsViewModel, filterObjectViewModel);
			}

			var gkMPTsViewModel = new FilterObjectViewModel(JournalObjectType.GKMPT);
			AddChild(gkViewModel, gkMPTsViewModel);
			foreach (var mpt in GKManager.MPTs)
			{
				var filterObjectViewModel = new FilterObjectViewModel(mpt);
				AddChild(gkMPTsViewModel, filterObjectViewModel);
			}

			var gkPumpStationsViewModel = new FilterObjectViewModel(JournalObjectType.GKPumpStation);
			AddChild(gkViewModel, gkPumpStationsViewModel);
			foreach (var pumpStation in GKManager.PumpStations)
			{
				var filterObjectViewModel = new FilterObjectViewModel(pumpStation);
				AddChild(gkPumpStationsViewModel, filterObjectViewModel);
			}

			var gkDelaysViewModel = new FilterObjectViewModel(JournalObjectType.GKDelay);
			AddChild(gkViewModel, gkDelaysViewModel);
			foreach (var delay in GKManager.Delays)
			{
				var filterObjectViewModel = new FilterObjectViewModel(delay);
				AddChild(gkDelaysViewModel, filterObjectViewModel);
			}

			var gkGuardZonesViewModel = new FilterObjectViewModel(JournalObjectType.GKGuardZone);
			AddChild(gkViewModel, gkGuardZonesViewModel);
			foreach (var guardZone in GKManager.GuardZones)
			{
				var filterObjectViewModel = new FilterObjectViewModel(guardZone);
				AddChild(gkGuardZonesViewModel, filterObjectViewModel);
			}

			var gkSKDZonesViewModel = new FilterObjectViewModel(JournalObjectType.GKSKDZone);
			AddChild(gkViewModel, gkSKDZonesViewModel);
			foreach (var skdZone in GKManager.SKDZones)
			{
				var filterObjectViewModel = new FilterObjectViewModel(skdZone);
				AddChild(gkSKDZonesViewModel, filterObjectViewModel);
			}

			var gkDoorsViewModel = new FilterObjectViewModel(JournalObjectType.GKDoor);
			AddChild(gkViewModel, gkDoorsViewModel);
			foreach (var door in GKManager.Doors)
			{
				var filterObjectViewModel = new FilterObjectViewModel(door);
				AddChild(gkDoorsViewModel, filterObjectViewModel);
			}

			var gkPIMsViewModel = new FilterObjectViewModel(JournalObjectType.GKPim);
			AddChild(gkViewModel, gkPIMsViewModel);
			foreach (var pim in GKManager.GlobalPims)
			{
				var filterObjectViewModel = new FilterObjectViewModel(pim);
				AddChild(gkPIMsViewModel, filterObjectViewModel);
			}

			var videoViewModel = new FilterObjectViewModel(JournalSubsystemType.Video);
			videoViewModel.IsExpanded = true;
			RootFilters.Add(videoViewModel);

			var videoDevicesViewModel = new FilterObjectViewModel(JournalObjectType.Camera);
			AddChild(videoViewModel, videoDevicesViewModel);
			foreach (var camera in ClientManager.SystemConfiguration.Cameras)
			{
				var filterObjectViewModel = new FilterObjectViewModel(camera);
				AddChild(videoDevicesViewModel, filterObjectViewModel);
			}

			var gkUsersViewModel = new FilterObjectViewModel(JournalObjectType.GKUser);
			RootFilters.Add(gkUsersViewModel);
			AllFilters.Add(gkUsersViewModel);

			var noneViewModel = new FilterObjectViewModel(JournalObjectType.None);
			RootFilters.Add(noneViewModel);
			AllFilters.Add(noneViewModel);
		}

		FilterObjectViewModel AddGKDeviceInternal(GKDevice device, FilterObjectViewModel parentDeviceViewModel)
		{
			var deviceViewModel = new FilterObjectViewModel(device);
			if (parentDeviceViewModel != null)
				AddChild(parentDeviceViewModel, deviceViewModel);

			foreach (var childDevice in device.Children)
			{
				AddGKDeviceInternal(childDevice, deviceViewModel);
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