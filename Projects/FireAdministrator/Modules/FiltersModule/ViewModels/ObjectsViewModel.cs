using Infrastructure.Common.Windows.Windows.ViewModels;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.Journal;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace FiltersModule.ViewModels
{
	public class ObjectsViewModel : BaseViewModel
	{
		public ObjectsViewModel(JournalFilter filter)
		{
			BuildTree();
			Initialize(filter);
		}

		void Initialize(JournalFilter filter)
		{
			AllObjects.ForEach(x => x.SetIsChecked(false));
			foreach (var journalObjectType in filter.JournalObjectTypes)
			{
				var objectTypeViewModel = AllObjects.FirstOrDefault(x => x.IsObjectGroup && x.JournalObjectType == journalObjectType);
				if (objectTypeViewModel != null)
				{
					objectTypeViewModel.IsChecked = true;
					objectTypeViewModel.IsRealChecked = true;
				}
			}
			foreach (var uid in filter.ObjectUIDs)
			{
				if (uid != Guid.Empty)
				{
					var objectUIDViewModel = AllObjects.FirstOrDefault(x => x.UID == uid);
					if (objectUIDViewModel != null)
					{
						objectUIDViewModel.IsChecked = true;
						objectUIDViewModel.IsRealChecked = true;
						objectUIDViewModel.ExpandToThis();
					}
				}
			}
		}

		public JournalFilter GetModel()
		{
			var filter = new JournalFilter();
			foreach (var subsystem in RootObjects)
			{
				foreach (var objectType in subsystem.Children)
				{
					if (objectType.IsChecked)
					{
						filter.JournalObjectTypes.Add(objectType.JournalObjectType);
					}
					else
					{
						foreach (var objectViewModel in objectType.GetAllChildren())
						{
							if (objectViewModel.IsChecked && objectViewModel.UID != Guid.Empty)
							{
								filter.ObjectUIDs.Add(objectViewModel.UID);
							}
						}
					}
				}
			}

			return filter;
		}

		public List<ObjectViewModel> AllObjects;

		public ObservableCollection<ObjectViewModel> RootObjects { get; private set; }

		ObjectViewModel _selectedObject;
		public ObjectViewModel SelectedObject
		{
			get { return _selectedObject; }
			set
			{
				_selectedObject = value;
				OnPropertyChanged(() => SelectedObject);
			}
		}

		void BuildTree()
		{
			RootObjects = new ObservableCollection<ObjectViewModel>();
			AllObjects = new List<ObjectViewModel>();

			var gkViewModel = new ObjectViewModel(JournalSubsystemType.GK);
			gkViewModel.IsExpanded = true;
			RootObjects.Add(gkViewModel);
			var gkDevicesViewModel = new ObjectViewModel(JournalObjectType.GKDevice);
			AddChild(gkViewModel, gkDevicesViewModel);
			foreach (var childDevice in GKManager.DeviceConfiguration.RootDevice.Children)
			{
				AddGKDeviceInternal(childDevice, gkDevicesViewModel);
			}

			var gkZonesViewModel = new ObjectViewModel(JournalObjectType.GKZone);
			AddChild(gkViewModel, gkZonesViewModel);
			foreach (var zone in GKManager.Zones)
			{
				var objectViewModel = new ObjectViewModel(zone);
				AddChild(gkZonesViewModel, objectViewModel);
			}

			var gkDirectionsViewModel = new ObjectViewModel(JournalObjectType.GKDirection);
			AddChild(gkViewModel, gkDirectionsViewModel);
			foreach (var direction in GKManager.Directions)
			{
				var objectViewModel = new ObjectViewModel(direction);
				AddChild(gkDirectionsViewModel, objectViewModel);
			}

			var gkMPTsViewModel = new ObjectViewModel(JournalObjectType.GKMPT);
			AddChild(gkViewModel, gkMPTsViewModel);
			foreach (var mpt in GKManager.MPTs)
			{
				var objectViewModel = new ObjectViewModel(mpt);
				AddChild(gkMPTsViewModel, objectViewModel);
			}

			var gkPumpStationsViewModel = new ObjectViewModel(JournalObjectType.GKPumpStation);
			AddChild(gkViewModel, gkPumpStationsViewModel);
			foreach (var pumpStation in GKManager.PumpStations)
			{
				var objectViewModel = new ObjectViewModel(pumpStation);
				AddChild(gkPumpStationsViewModel, objectViewModel);
			}

			var gkDelaysViewModel = new ObjectViewModel(JournalObjectType.GKDelay);
			AddChild(gkViewModel, gkDelaysViewModel);
			foreach (var delay in GKManager.Delays)
			{
				var objectViewModel = new ObjectViewModel(delay);
				AddChild(gkDelaysViewModel, objectViewModel);
			}

			var gkGuardZonesViewModel = new ObjectViewModel(JournalObjectType.GKGuardZone);
			AddChild(gkViewModel, gkGuardZonesViewModel);
			foreach (var guardZone in GKManager.GuardZones)
			{
				var objectViewModel = new ObjectViewModel(guardZone);
				AddChild(gkGuardZonesViewModel, objectViewModel);
			}

			var gkDoorsViewModel = new ObjectViewModel(JournalObjectType.GKDoor);
			AddChild(gkViewModel, gkDoorsViewModel);
			foreach (var door in GKManager.Doors)
			{
				var doorViewModel = new ObjectViewModel(door);
				AddChild(gkDoorsViewModel, doorViewModel);
			}

			var gkSKDZonesViewModel = new ObjectViewModel(JournalObjectType.GKSKDZone);
			AddChild(gkViewModel, gkSKDZonesViewModel);
			foreach (var skdZone in GKManager.SKDZones)
			{
				var filterObjectViewModel = new ObjectViewModel(skdZone);
				AddChild(gkSKDZonesViewModel, filterObjectViewModel);
			}

			var videoViewModel = new ObjectViewModel(JournalSubsystemType.Video);
			videoViewModel.IsExpanded = true;
			RootObjects.Add(videoViewModel);

			var videoDevicesViewModel = new ObjectViewModel(JournalObjectType.Camera);
			AddChild(videoViewModel, videoDevicesViewModel);
			foreach (var camera in ClientManager.SystemConfiguration.Cameras)
			{
				var objectViewModel = new ObjectViewModel(camera);
				AddChild(videoDevicesViewModel, objectViewModel);
			}
		}

		ObjectViewModel AddGKDeviceInternal(GKDevice device, ObjectViewModel parentDeviceViewModel)
		{
			var deviceViewModel = new ObjectViewModel(device);
			if (parentDeviceViewModel != null)
				AddChild(parentDeviceViewModel, deviceViewModel);

			foreach (var childDevice in device.Children)
			{
				AddGKDeviceInternal(childDevice, deviceViewModel);
			}
			return deviceViewModel;
		}

		void AddChild(ObjectViewModel parentDeviceViewModel, ObjectViewModel childDeviceViewModel)
		{
			parentDeviceViewModel.AddChild(childDeviceViewModel);
			AllObjects.Add(childDeviceViewModel);
		}
	}
}