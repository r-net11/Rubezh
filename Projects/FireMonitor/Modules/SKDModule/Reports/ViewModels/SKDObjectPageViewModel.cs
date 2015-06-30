using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Common;
using FiresecAPI.GK;
using FiresecAPI.Journal;
using FiresecAPI.SKD;
using FiresecAPI.SKD.ReportFilters;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.SKDReports;

namespace SKDModule.Reports.ViewModels
{
	public class SKDObjectPageViewModel : FilterContainerViewModel
	{
		public SKDObjectPageViewModel()
		{
			Title = "Объекты Страж";
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

			var skdViewModel = new SKDObjectViewModel(JournalSubsystemType.SKD);
			skdViewModel.IsExpanded = true;
			RootFilters.Add(skdViewModel);

			var skdDevicesViewModel = new SKDObjectViewModel(JournalObjectType.SKDDevice);
			skdViewModel.AddChild(skdDevicesViewModel);
			foreach (var childDevice in SKDManager.SKDConfiguration.RootDevice.Children)
				AddSKDDeviceInternal(childDevice, skdDevicesViewModel);

			var skdZonesViewModel = new SKDObjectViewModel(JournalObjectType.SKDZone);
			skdViewModel.AddChild(skdZonesViewModel);
			foreach (var zone in SKDManager.Zones)
				skdZonesViewModel.AddChild(new SKDObjectViewModel(zone));

			var skdDoorsViewModel = new SKDObjectViewModel(JournalObjectType.SKDDoor);
			skdViewModel.AddChild(skdDoorsViewModel);
			foreach (var door in SKDManager.Doors)
				skdDoorsViewModel.AddChild(new SKDObjectViewModel(door));

			var gkViewModel = new SKDObjectViewModel(JournalSubsystemType.GK);
			gkViewModel.IsExpanded = true;
			RootFilters.Add(gkViewModel);

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
		}
		private SKDObjectViewModel AddGKDeviceInternal(GKDevice device, SKDObjectViewModel parentDeviceViewModel)
		{
			var deviceViewModel = new SKDObjectViewModel(device);
			if (parentDeviceViewModel != null)
				parentDeviceViewModel.AddChild(deviceViewModel);
			foreach (var childDevice in device.Children)
				if (!childDevice.IsNotUsed)
					AddGKDeviceInternal(childDevice, deviceViewModel);
			return deviceViewModel;
		}
		private SKDObjectViewModel AddSKDDeviceInternal(SKDDevice device, SKDObjectViewModel parentDeviceViewModel)
		{
			var deviceViewModel = new SKDObjectViewModel(device);
			if (parentDeviceViewModel != null)
				parentDeviceViewModel.AddChild(deviceViewModel);
			foreach (var childDevice in device.Children)
				AddSKDDeviceInternal(childDevice, deviceViewModel);
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