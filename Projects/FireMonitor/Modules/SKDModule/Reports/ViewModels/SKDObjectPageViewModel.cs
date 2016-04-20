using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Common;
using RubezhAPI.GK;
using RubezhAPI.Journal;
using RubezhAPI.SKD;
using RubezhAPI.SKD.ReportFilters;
using RubezhClient;
using Infrastructure.Common;
using Infrastructure.Common.SKDReports;
using RubezhAPI;

namespace SKDModule.Reports.ViewModels
{
	public class SKDObjectPageViewModel : FilterContainerViewModel
	{
		public SKDObjectPageViewModel()
		{
			Title = "Объекты";
			BuildTree();
			SelectAllCommand = new RelayCommand(() => RootFilters.ForEach(item => { item.IsChecked = false; item.IsChecked = true; }));
			SelectNoneCommand = new RelayCommand(() => RootFilters.ForEach(item => { item.IsChecked = true; item.IsChecked = false; }));
		}

		public RelayCommand SelectAllCommand { get; private set; }
		public RelayCommand SelectNoneCommand { get; private set; }
		public ObservableCollection<SKDObjectViewModel> RootFilters { get; private set; }
		private void BuildTree()
		{
			RootFilters = new ObservableCollection<SKDObjectViewModel>();

			var gkViewModel = new SKDObjectViewModel(JournalSubsystemType.GK);
			gkViewModel.IsExpanded = true;
			RootFilters.Add(gkViewModel);

			var video = new SKDObjectViewModel(JournalSubsystemType.Video);
			video.IsExpanded = true;
			RootFilters.Add(video);

			var gkDevicesViewModel = new SKDObjectViewModel(JournalObjectType.GKDevice);
			gkViewModel.AddChild(gkDevicesViewModel);
			foreach (var childDevice in GKManager.DeviceConfiguration.RootDevice.Children)
				AddGKDeviceInternal(childDevice, gkDevicesViewModel);

			var gkZonesViewModel = new SKDObjectViewModel(JournalObjectType.GKZone);
			gkViewModel.AddChild(gkZonesViewModel);
			foreach (var zone in GKManager.Zones)
				gkZonesViewModel.AddChild(new SKDObjectViewModel(zone));

			var gkDoorsViewModel = new SKDObjectViewModel(JournalObjectType.GKDoor);
			gkViewModel.AddChild(gkDoorsViewModel);
			foreach (var door in GKManager.Doors)
				gkDoorsViewModel.AddChild(new SKDObjectViewModel(door));

			var gkDerectionsViewModel = new SKDObjectViewModel(JournalObjectType.GKDirection);
			gkViewModel.AddChild(gkDerectionsViewModel);
			foreach (var direction in GKManager.Directions)
				gkDerectionsViewModel.AddChild(new SKDObjectViewModel(direction));

			var gkDelaysViewModel = new SKDObjectViewModel(JournalObjectType.GKDelay);
			gkViewModel.AddChild(gkDelaysViewModel);
			foreach (var delay in GKManager.Delays)
				gkDelaysViewModel.AddChild(new SKDObjectViewModel(delay));

			var gkGuardZonesViewModel = new SKDObjectViewModel(JournalObjectType.GKGuardZone);
			gkViewModel.AddChild(gkGuardZonesViewModel);
			foreach (var guardZone in GKManager.GuardZones)
				gkGuardZonesViewModel.AddChild(new SKDObjectViewModel(guardZone));

			var gkMPTsViewModel = new SKDObjectViewModel(JournalObjectType.GKMPT);
			gkViewModel.AddChild(gkMPTsViewModel);
			foreach (var mpt in GKManager.MPTs)
				gkMPTsViewModel.AddChild(new SKDObjectViewModel(mpt));

			var gkPumpsViewModel = new SKDObjectViewModel(JournalObjectType.GKPumpStation);
			gkViewModel.AddChild(gkPumpsViewModel);
			foreach (var pump in GKManager.PumpStations)
				gkPumpsViewModel.AddChild(new SKDObjectViewModel(pump));

			var gkSKDZonesViewModel = new SKDObjectViewModel(JournalObjectType.GKSKDZone);
			gkViewModel.AddChild(gkSKDZonesViewModel);
			foreach (var SKDZone in GKManager.SKDZones)
				gkSKDZonesViewModel.AddChild(new SKDObjectViewModel(SKDZone));

			var gkVideoDevicesViewModel = new SKDObjectViewModel(JournalObjectType.Camera);
			video.AddChild(gkVideoDevicesViewModel);
			foreach (var camera in ClientManager.SystemConfiguration.Cameras)
				gkVideoDevicesViewModel.AddChild(new SKDObjectViewModel(camera));

		}
		private SKDObjectViewModel AddGKDeviceInternal(GKDevice device, SKDObjectViewModel parentDeviceViewModel)
		{
			var deviceViewModel = new SKDObjectViewModel(device);
			if (parentDeviceViewModel != null)
				parentDeviceViewModel.AddChild(deviceViewModel);
			foreach (var childDevice in device.Children)
				AddGKDeviceInternal(childDevice, deviceViewModel);
			return deviceViewModel;
		}

		public override void LoadFilter(SKDReportFilter filter)
		{
			var objectFilter = filter as EventsReportFilter;
			if (objectFilter == null)
				return;
			var allFilters = RootFilters.SelectMany(item => item.GetAllChildren());
			allFilters.ForEach(item => item.IsChecked = false);
			foreach (var subsystemType in objectFilter.JournalOjbectSubsystemTypes)
			{
				var filterNameViewModel = RootFilters.FirstOrDefault(x => x.IsSubsystem && x.JournalSubsystemType == subsystemType);
				if (filterNameViewModel != null)
					filterNameViewModel.IsChecked = true;
			}
			foreach (var journalObjectType in objectFilter.JournalObjectTypes)
			{
				var filterNameViewModel = allFilters.FirstOrDefault(x => x.IsObjectGroup && x.JournalObjectType == journalObjectType);
				if (filterNameViewModel != null)
					filterNameViewModel.IsChecked = true;
			}
			foreach (var uid in objectFilter.ObjectUIDs)
			{
				if (uid != Guid.Empty)
				{
					var filterNameViewModel = allFilters.FirstOrDefault(x => x.UID == uid);
					if (filterNameViewModel != null)
					{
						filterNameViewModel.IsChecked = true;
						filterNameViewModel.ExpandToThis();
					}
				}
			}
		}
		public override void UpdateFilter(SKDReportFilter filter)
		{
			var objectFilter = filter as EventsReportFilter;
			if (objectFilter == null)
				return;
			objectFilter.JournalOjbectSubsystemTypes = new List<JournalSubsystemType>();
			objectFilter.JournalObjectTypes = new List<JournalObjectType>();
			objectFilter.ObjectUIDs = new List<Guid>();
			foreach (var subsystemFilter in RootFilters)
				if (subsystemFilter.IsChecked)
					objectFilter.JournalOjbectSubsystemTypes.Add(subsystemFilter.JournalSubsystemType);
				else
					foreach (var objectTypeFilter in subsystemFilter.Children)
					{
						if (objectTypeFilter.IsChecked)
							objectFilter.JournalObjectTypes.Add(objectTypeFilter.JournalObjectType);
						else
							foreach (var filterViewModel in objectTypeFilter.GetAllChildren())
								if (filterViewModel.IsChecked && filterViewModel.UID != Guid.Empty)
									objectFilter.ObjectUIDs.Add(filterViewModel.UID);
					}
		}
	}
}