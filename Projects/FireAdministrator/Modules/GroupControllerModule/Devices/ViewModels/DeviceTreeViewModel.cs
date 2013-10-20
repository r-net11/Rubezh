using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using System.Linq;

namespace GKModule.ViewModels
{
	public class DeviceTreeViewModel:BaseViewModel
	{
		public DeviceTreeViewModel(XDevice device, List<XZone> zones, List<XDirection> directions)
		{
			Devices = new List<ObjectViewModel>();
			Zones = new List<ObjectViewModel>();
			Directions = new List<ObjectViewModel>();
			IitializeObjects(device, zones, directions);
		}
		void IitializeObjects(XDevice device, List<XZone> zones, List<XDirection> directions)
		{
			if (device.Children != null)
			{
				InitializeDevices(device);
			}
			if(zones != null)
			foreach (var zone in zones)
			{
				var objectViewModel = new ObjectViewModel(zone);
				Objects.Add(objectViewModel);
				Zones.Add(objectViewModel);
			}
			if(directions != null)
			foreach (var direction in directions)
			{
				var objectViewModel = new ObjectViewModel(direction);
				Objects.Add(objectViewModel);
				Directions.Add(objectViewModel);
			}
		}

		ObjectViewModel InitializeDevices(XDevice device)
		{
			var objectViewModel = new ObjectViewModel(device);
			if (!device.Driver.IsGroupDevice)
			{
				Objects.Add(objectViewModel);
				Devices.Add(objectViewModel);
			}
			foreach (var childDevice in device.Children)
			{
				InitializeDevices(childDevice);
			}
			return objectViewModel;
		}

		public static void CompareTrees(List<ObjectViewModel> objects1, List<ObjectViewModel> objects2)
		{
			int max = Math.Max(objects1.Count, objects2.Count);
			for (int i = 0; i < max; i++)
			{
				if ((i == objects1.Count)&&(i != objects2.Count))
				{
					for (int j = i; j < objects2.Count; j++)
					{
						var object2 = (ObjectViewModel) objects2[j].Clone();
						object2.HasMissingDifferences = true;
						objects1.Add(object2);
					}
				}
				
				if ((i == objects2.Count)&&(i != objects1.Count))
				{
					for (int j = i; j < objects1.Count; j++)
					{
						var object1 = (ObjectViewModel)objects1[j].Clone();
						object1.HasMissingDifferences = true;
						objects2.Add(object1);
					}
				}

				if (!CompareObjects(objects1[i], objects2))
				{
					var object1 = (ObjectViewModel)objects1[i].Clone();
					object1.HasDifferences = true;
					if(object1.Address == "")
						continue;
					//if ((object1.Device != null) && (object2.Device != null) && (object1.Device.IntAddress > object2.Device.IntAddress))
					//{
					//    objects1.Insert(i, object2);
					//    objects2.Insert(i + 1, object1);
					//}
					//else
					//{
					//    objects1.Insert(i + 1, object2);
					//    objects2.Insert(i, object1);
					//}
					objects2.Add(object1);
					i++;
				}
				if (!CompareObjects(objects2[i], objects1))
				{
					var object2 = (ObjectViewModel)objects2[i].Clone();
					object2.HasDifferences = true;
					if (object2.Address == "")
						continue;
					objects1.Add(object2);
					i++;
				}
			}
		}

		private static bool CompareObjects(ObjectViewModel objectViewModel, List<ObjectViewModel> objectViewModels)
		{
			var matchedObjectViewModel = objectViewModels.FirstOrDefault
				(x =>
				 (x.Name == objectViewModel.Name) &&
				 (x.Address == objectViewModel.Address) &&
				 (x.ParentPath == objectViewModel.ParentPath));
			if ((matchedObjectViewModel != null) && (!matchedObjectViewModel.HasDifferences))
				return true;
			return false;
		}

		static void ExpandChild(DeviceViewModel parentDeviceViewModel)
		{
			parentDeviceViewModel.IsExpanded = true;
			foreach (var deviceViewModel in parentDeviceViewModel.Children)
			{
				ExpandChild(deviceViewModel);
			}
		}
		public ObservableCollection<ObjectViewModel> Objects
		{
			get
			{
				var objects = new List<ObjectViewModel>();
				objects.AddRange(Devices);
				objects.AddRange(Zones);
				objects.AddRange(Directions);
				return new ObservableCollection<ObjectViewModel>(objects);
			}
		}

		public List<ObjectViewModel> Devices { get; private set; }
		public List<ObjectViewModel> Zones { get; private set; }
		public List<ObjectViewModel> Directions { get; private set; }

		ObjectViewModel _selectedObject;
		public ObjectViewModel SelectedObject
		{
			get { return _selectedObject; }
			set
			{
				_selectedObject = value;
				OnPropertyChanged("SelectedObject");
			}
		}
	}
}
