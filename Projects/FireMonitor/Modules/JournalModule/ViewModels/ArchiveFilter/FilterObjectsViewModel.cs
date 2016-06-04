using FiresecAPI.Journal;
using FiresecAPI.SKD;
using Infrastructure.Common.TreeList;
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
			_allObjects.ForEach(x => x.SetIsChecked(false));
			foreach (var journalObjectType in filter.JournalObjectTypes)
			{
				var objectTypeViewModel = _allObjects.FirstOrDefault(x => x.IsObjectGroup && x.JournalObjectType == journalObjectType);
				if (objectTypeViewModel != null)
				{
					objectTypeViewModel.IsChecked = true;
					ExpandParent(objectTypeViewModel);
				}
			}
			foreach (var uid in filter.ObjectUIDs)
			{
				if (uid != Guid.Empty)
				{
					var objectUIDViewModel = _allObjects.FirstOrDefault(x => x.UID == uid);
					if (objectUIDViewModel != null)
					{
						objectUIDViewModel.IsChecked = true;
						ExpandParent(objectUIDViewModel);
					}
				}
			}
		}

		public ArchiveFilter GetModel()
		{
			var filter = new ArchiveFilter();
			foreach (var subsystemFilter in RootFilters)
			{
				foreach (var objectTypeFilter in subsystemFilter.Children)
				{
					if (objectTypeFilter.IsChecked)
						filter.JournalObjectTypes.Add(objectTypeFilter.JournalObjectType);
					else
					{
						foreach (var filterViewModel in objectTypeFilter.GetAllChildren())
						{
							if (filterViewModel.IsChecked && filterViewModel.UID != Guid.Empty)
								filter.ObjectUIDs.Add(filterViewModel.UID);
						}
					}
				}
			}

			return filter;
		}

		private List<FilterObjectViewModel> _allObjects;

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

		private void BuildTree()
		{
			RootFilters = new ObservableCollection<FilterObjectViewModel>();
			_allObjects = new List<FilterObjectViewModel>();

			var skdViewModel = new FilterObjectViewModel(JournalSubsystemType.SKD);
			RootFilters.Add(skdViewModel);

			var skdDevicesViewModel = new FilterObjectViewModel(JournalObjectType.SKDDevice);
			AddChild(skdViewModel, skdDevicesViewModel);
			foreach (var childDevice in SKDManager.SKDConfiguration.RootDevice.Children.OrderBy(x => x.Name))
			{
				AddSKDDeviceInternal(childDevice, skdDevicesViewModel);
			}

			var skdZonesViewModel = new FilterObjectViewModel(JournalObjectType.SKDZone);
			AddChild(skdViewModel, skdZonesViewModel);
			foreach (var zone in SKDManager.Zones.OrderBy(x => x.Name))
			{
				var filterObjectViewModel = new FilterObjectViewModel(zone);
				AddChild(skdZonesViewModel, filterObjectViewModel);
			}

			var skdDoorsViewModel = new FilterObjectViewModel(JournalObjectType.SKDDoor);
			AddChild(skdViewModel, skdDoorsViewModel);
			foreach (var door in SKDManager.Doors.OrderBy(x => x.Name))
			{
				var filterObjectViewModel = new FilterObjectViewModel(door);
				AddChild(skdDoorsViewModel, filterObjectViewModel);
			}

			var videoViewModel = new FilterObjectViewModel(JournalSubsystemType.Video);
			RootFilters.Add(videoViewModel);

			var videoDevicesViewModel = new FilterObjectViewModel(JournalObjectType.VideoDevice);
			AddChild(videoViewModel, videoDevicesViewModel);
			foreach (var camera in FiresecClient.FiresecManager.SystemConfiguration.Cameras.OrderBy(x => x.Name))
			{
				var filterObjectViewModel = new FilterObjectViewModel(camera);
				AddChild(videoDevicesViewModel, filterObjectViewModel);
			}
		}

		private FilterObjectViewModel AddSKDDeviceInternal(SKDDevice device, FilterObjectViewModel parentDeviceViewModel)
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

		private void AddChild(FilterObjectViewModel parentDeviceViewModel, FilterObjectViewModel childDeviceViewModel)
		{
			parentDeviceViewModel.AddChild(childDeviceViewModel);
			_allObjects.Add(childDeviceViewModel);
		}

		private void ExpandParent(TreeNodeViewModel<FilterObjectViewModel> child)
		{
			if (child.Parent == null)
				return;
			
			child.Parent.IsExpanded = true;
			ExpandParent(child.Parent);
		}
	}
}