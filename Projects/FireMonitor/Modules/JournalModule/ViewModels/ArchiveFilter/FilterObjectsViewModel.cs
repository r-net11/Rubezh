using StrazhAPI.Journal;
using StrazhAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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
			foreach (var subsystemType in filter.JournalSubsystemTypes)
			{
				var filterNameViewModel = RootFilters.FirstOrDefault(x => x.IsSubsystem && x.JournalSubsystemType == subsystemType);
				if (filterNameViewModel != null)
				{
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

			var skdViewModel = new FilterObjectViewModel(JournalSubsystemType.SKD) {IsExpanded = true};
			RootFilters.Add(skdViewModel);

			var skdDevicesViewModel = new FilterObjectViewModel(JournalObjectType.SKDDevice);
			skdViewModel.AddChild(skdDevicesViewModel);
			foreach (var childDevice in SKDManager.SKDConfiguration.RootDevice.Children.OrderBy(x => x.Name))
			{
				AddSKDDeviceInternal(childDevice, skdDevicesViewModel);
			}

			var skdZonesViewModel = new FilterObjectViewModel(JournalObjectType.SKDZone);
			skdViewModel.AddChild(skdZonesViewModel);
			foreach (var zone in SKDManager.Zones.OrderBy(x => x.Name))
			{
				var filterObjectViewModel = new FilterObjectViewModel(zone);
				skdZonesViewModel.AddChild(filterObjectViewModel);
			}

			var skdDoorsViewModel = new FilterObjectViewModel(JournalObjectType.SKDDoor);
			skdViewModel.AddChild(skdDoorsViewModel);
			foreach (var door in SKDManager.Doors.OrderBy(x => x.Name))
			{
				var filterObjectViewModel = new FilterObjectViewModel(door);
				skdDoorsViewModel.AddChild(filterObjectViewModel);
			}

			var videoViewModel = new FilterObjectViewModel(JournalSubsystemType.Video) {IsExpanded = true};
			RootFilters.Add(videoViewModel);

			var videoDevicesViewModel = new FilterObjectViewModel(JournalObjectType.VideoDevice);
			videoViewModel.AddChild(videoDevicesViewModel);
			foreach (var camera in FiresecClient.FiresecManager.SystemConfiguration.Cameras.OrderBy(x => x.Name))
			{
				var filterObjectViewModel = new FilterObjectViewModel(camera);
				videoDevicesViewModel.AddChild(filterObjectViewModel);
			}
		}

		FilterObjectViewModel AddSKDDeviceInternal(SKDDevice device, FilterObjectViewModel parentDeviceViewModel)
		{
			var deviceViewModel = new FilterObjectViewModel(device);
			if (parentDeviceViewModel != null)
				parentDeviceViewModel.AddChild(deviceViewModel);
				//AddChild(parentDeviceViewModel, deviceViewModel);

			foreach (var childDevice in device.Children)
			{
				AddSKDDeviceInternal(childDevice, deviceViewModel);
			}
			return deviceViewModel;
		}

		//void AddChild(FilterObjectViewModel parentDeviceViewModel, FilterObjectViewModel childDeviceViewModel)
		//{
		//	parentDeviceViewModel.AddChild(childDeviceViewModel);
		//}
	}
}