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
				InitializeDevices(new ObjectViewModel(device), new ObjectViewModel(device.Parent));
			}
			if(zones != null)
			foreach (var zone in zones)
			{
				var objectViewModel = new ObjectViewModel(zone);
				objectViewModel.Parent = new ObjectViewModel(device);
				Objects.Add(objectViewModel);
				Zones.Add(objectViewModel);
			}
			if(directions != null)
			foreach (var direction in directions)
			{
				var objectViewModel = new ObjectViewModel(direction);
				objectViewModel.Parent = new ObjectViewModel(device);
				Objects.Add(objectViewModel);
				Directions.Add(objectViewModel);
			}
		}
		void InitializeDevices(ObjectViewModel objectViewModel, ObjectViewModel parentObjectViewModel)
		{
			objectViewModel.Parent = parentObjectViewModel;
			if (objectViewModel.Device.Driver.IsGroupDevice)
				return;
				if (parentObjectViewModel != null)
					parentObjectViewModel.Children.Add(objectViewModel);
				Objects.Add(objectViewModel);
				Devices.Add(objectViewModel);
			if (objectViewModel.Device.Children.Count > 0)
			{
				foreach (var childDevice in objectViewModel.Device.Children)
				{
					InitializeDevices(new ObjectViewModel(childDevice), objectViewModel);
				}
			}
			return;
		}
		public static List<List<ObjectViewModel>> CompareTrees(List<ObjectViewModel> objects1, List<ObjectViewModel> objects2)
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

			if (objects1.FirstOrDefault().Device != null)
			{
				SortTree(ref objects1);
				SortTree(ref objects2);
			}
			return new List<List<ObjectViewModel>> {objects1, objects2};
		}

		private static void SortTree(ref List<ObjectViewModel> objectViewModels)
		{
			var rootObject = objectViewModels.FirstOrDefault(x => x.Device.Driver.DriverType == XDriverType.GK);
			objectViewModels = new List<ObjectViewModel>();
			AddChildren(objectViewModels, rootObject);
		}

		private static void AddChildren(List<ObjectViewModel> newobjectViewModels, ObjectViewModel rootObject)
		{
			foreach (var objectViewModel in rootObject.Children)
			{
				newobjectViewModels.Add(objectViewModel);
				if (objectViewModel.Children.Count() > 0)
					AddChildren(newobjectViewModels, objectViewModel);
			}
		}

		private static bool CompareObjects(ObjectViewModel objectViewModel, List<ObjectViewModel> objectViewModels)
		{
			var matchedObjectViewModel = objectViewModels.FirstOrDefault
				(x =>
				 (x.Name == objectViewModel.Name) &&
				 (x.Address == objectViewModel.Address) &&
				 ((x.Parent.Name == objectViewModel.Parent.Name) && (x.Parent.Address == objectViewModel.Parent.Address)));
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

		public List<ObjectViewModel> Devices { get; set; }
		public List<ObjectViewModel> Zones { get; set; }
		public List<ObjectViewModel> Directions { get; set; }

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
