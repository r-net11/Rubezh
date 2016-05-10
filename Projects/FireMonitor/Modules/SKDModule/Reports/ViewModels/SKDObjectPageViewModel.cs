using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.SKDReports;
using StrazhAPI.SKD.ReportFilters;
using System.Collections.ObjectModel;
using StrazhAPI.Journal;
using StrazhAPI.SKD;
using StrazhAPI.GK;
using Common;
using Infrastructure.Common;

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

			var skdViewModel = new SKDObjectViewModel(JournalSubsystemType.SKD) {IsExpanded = true};
			RootFilters.Add(skdViewModel);

			var skdDevicesViewModel = new SKDObjectViewModel(JournalObjectType.SKDDevice);
			skdViewModel.AddChild(skdDevicesViewModel);
			foreach (var childDevice in SKDManager.SKDConfiguration.RootDevice.Children.OrderBy(x => x.Name))
				AddSKDDeviceInternal(childDevice, skdDevicesViewModel);

			var skdZonesViewModel = new SKDObjectViewModel(JournalObjectType.SKDZone);
			skdViewModel.AddChild(skdZonesViewModel);
			skdZonesViewModel.AddChildren(SKDManager.Zones.OrderBy(x => x.Name).Select(x => new SKDObjectViewModel(x)));

			var skdDoorsViewModel = new SKDObjectViewModel(JournalObjectType.SKDDoor);
			skdViewModel.AddChild(skdDoorsViewModel);
			skdDoorsViewModel.AddChildren(SKDManager.Doors.OrderBy(x => x.Name).Select(x => new SKDObjectViewModel(x)));
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
