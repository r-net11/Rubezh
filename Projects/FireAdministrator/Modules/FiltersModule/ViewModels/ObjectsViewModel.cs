using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using StrazhAPI.Journal;
using StrazhAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace FiltersModule.ViewModels
{
	public class ObjectsViewModel : BaseViewModel
	{
		public ObjectsViewModel(JournalFilter filter)
		{
			BuildTree();
			Initialize(filter);
		}

		private void Initialize(JournalFilter filter)
		{
			AllObjects.ForEach(x => x.SetIsChecked(false));
			foreach (var journalObjectType in filter.JournalObjectTypes)
			{
				var objectTypeViewModel = AllObjects.FirstOrDefault(x => x.IsObjectGroup && x.JournalObjectType == journalObjectType);
				if (objectTypeViewModel != null)
				{
					objectTypeViewModel.IsChecked = true;
				}
			}
			foreach (var uid in filter.ObjectUIDs)
			{
				if (uid != Guid.Empty)
				{
					var objectUIDViewModel = AllObjects.FirstOrDefault(x => x.UID == uid);
					if (objectUIDViewModel != null)
						objectUIDViewModel.IsChecked = true;
				}
			}
		}

		public ArchiveFilter GetModel()
		{
			var filter = new ArchiveFilter();
			foreach (var subsystem in RootObjects)
			{
				foreach (var objectType in subsystem.Children)
				{
					if (objectType.IsChecked)
						filter.JournalObjectTypes.Add(objectType.JournalObjectType);
					else
					{
						foreach (var objectViewModel in objectType.GetAllChildren())
						{
							if (objectViewModel.IsChecked && objectViewModel.UID != Guid.Empty)
								filter.ObjectUIDs.Add(objectViewModel.UID);
						}
					}
				}
			}

			return filter;
		}

		public List<ObjectViewModel> AllObjects;

		public ObservableCollection<ObjectViewModel> RootObjects { get; private set; }

		private ObjectViewModel _selectedObject;
		public ObjectViewModel SelectedObject
		{
			get { return _selectedObject; }
			set
			{
				_selectedObject = value;
				OnPropertyChanged(() => SelectedObject);
			}
		}

		private void BuildTree()
		{
			RootObjects = new ObservableCollection<ObjectViewModel>();
			AllObjects = new List<ObjectViewModel>();

			var skdViewModel = new ObjectViewModel(JournalSubsystemType.SKD);
			skdViewModel.IsExpanded = true;
			RootObjects.Add(skdViewModel);

			var skdDevicesViewModel = new ObjectViewModel(JournalObjectType.SKDDevice);
			AddChild(skdViewModel, skdDevicesViewModel);
			foreach (var childDevice in SKDManager.SKDConfiguration.RootDevice.Children)
			{
				AddSKDDeviceInternal(childDevice, skdDevicesViewModel);
			}

			var skdZonesViewModel = new ObjectViewModel(JournalObjectType.SKDZone);
			AddChild(skdViewModel, skdZonesViewModel);
			foreach (var zone in SKDManager.Zones)
			{
				var objectViewModel = new ObjectViewModel(zone);
				AddChild(skdZonesViewModel, objectViewModel);
			}

			var skdDoorsViewModel = new ObjectViewModel(JournalObjectType.SKDDoor);
			AddChild(skdViewModel, skdDoorsViewModel);
			foreach (var door in SKDManager.Doors)
			{
				var objectViewModel = new ObjectViewModel(door);
				AddChild(skdDoorsViewModel, objectViewModel);
			}

			var videoViewModel = new ObjectViewModel(JournalSubsystemType.Video);
			videoViewModel.IsExpanded = true;
			RootObjects.Add(videoViewModel);

			var videoDevicesViewModel = new ObjectViewModel(JournalObjectType.VideoDevice);
			AddChild(videoViewModel, videoDevicesViewModel);
			foreach (var camera in FiresecClient.FiresecManager.SystemConfiguration.Cameras)
			{
				var objectViewModel = new ObjectViewModel(camera);
				AddChild(videoDevicesViewModel, objectViewModel);
			}
		}

		private ObjectViewModel AddSKDDeviceInternal(SKDDevice device, ObjectViewModel parentDeviceViewModel)
		{
			var deviceViewModel = new ObjectViewModel(device);
			if (parentDeviceViewModel != null)
				AddChild(parentDeviceViewModel, deviceViewModel);

			foreach (var childDevice in device.Children)
			{
				AddSKDDeviceInternal(childDevice, deviceViewModel);
			}
			return deviceViewModel;
		}

		private void AddChild(ObjectViewModel parentDeviceViewModel, ObjectViewModel childDeviceViewModel)
		{
			parentDeviceViewModel.AddChild(childDeviceViewModel);
			AllObjects.Add(childDeviceViewModel);
		}
	}
}