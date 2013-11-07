using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class DeviceTreeViewModel : BaseViewModel
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
			if (zones != null)
				foreach (var zone in zones)
				{
					var objectViewModel = new ObjectViewModel(zone);
					objectViewModel.IsZone = true;
					objectViewModel.Parent = new ObjectViewModel(device);
					Objects.Add(objectViewModel);
					Zones.Add(objectViewModel);
				}
			if (directions != null)
				foreach (var direction in directions)
				{
					var objectViewModel = new ObjectViewModel(direction);
					objectViewModel.IsDirection = true;
					objectViewModel.Parent = new ObjectViewModel(device);
					Objects.Add(objectViewModel);
					Directions.Add(objectViewModel);
				}
		}

		void InitializeDevices(ObjectViewModel objectViewModel, ObjectViewModel parentObjectViewModel)
		{
			objectViewModel.IsDevice = true;
			objectViewModel.Parent = parentObjectViewModel;
			if ((parentObjectViewModel != null) && (!objectViewModel.IsVirtualDevice))
				parentObjectViewModel.Children.Add(objectViewModel);
			if (!objectViewModel.IsVirtualDevice)
			{
				Objects.Add(objectViewModel);
				Devices.Add(objectViewModel);
			}
			if (objectViewModel.Device.Children.Count > 0)
			{
				foreach (var childDevice in objectViewModel.Device.Children)
				{
					if (objectViewModel.IsVirtualDevice)
					{
						parentObjectViewModel.Children.Remove(objectViewModel);
						if (objectViewModel.Parent.IsVirtualDevice)
						{
							objectViewModel.Parent.Parent.Children.Remove(objectViewModel.Parent);
							InitializeDevices(new ObjectViewModel(childDevice), objectViewModel.Parent.Parent);
						}
						InitializeDevices(new ObjectViewModel(childDevice), objectViewModel.Parent);
					}
					else
						InitializeDevices(new ObjectViewModel(childDevice), objectViewModel);
				}
			}
			return;
		}

		public static List<List<ObjectViewModel>> CompareTrees(List<ObjectViewModel> objects1, List<ObjectViewModel> objects2, XDriverType driverType)
		{
			foreach (var object1 in objects1)
			{
				if (!ContainsObject(object1, objects2))
				{
					var newObject = (ObjectViewModel)object1.Clone();
					newObject.Children = new List<ObjectViewModel>();
					newObject.HasDifferences = true;
					objects2.Add(newObject);
					if (newObject.IsDevice)
					{
						var object2Parent = objects2.FirstOrDefault(x => (x.Name == newObject.Parent.Name) && (x.Address == newObject.Parent.Address));
						object2Parent.Children.Add(newObject);
						newObject.Parent = object2Parent;
					}
				}
			}

			foreach (var object2 in objects2)
			{
				if (!ContainsObject(object2, objects1))
				{
					var newObject = (ObjectViewModel)object2.Clone();
					newObject.Children = new List<ObjectViewModel>();
					newObject.HasDifferences = true;
					objects1.Add(newObject);
					if (newObject.IsDevice)
					{
						var object1Parent = objects1.FirstOrDefault(x => (x.Name == newObject.Parent.Name) && (x.Address == newObject.Parent.Address));
						object1Parent.Children.Add(newObject);
						newObject.Parent = object1Parent;
					}
				}
			}

			if ((objects1.Count != 0) && (objects1.FirstOrDefault().IsDevice))
			{
				SortTree(ref objects1, driverType);
				SortTree(ref objects2, driverType);
			}
			else
			{
				objects1 = objects1.OrderBy(x => x.Name).ToList();
				objects2 = objects2.OrderBy(x => x.Name).ToList();
			}
			return new List<List<ObjectViewModel>> { objects1, objects2 };
		}

		private static void SortTree(ref List<ObjectViewModel> objectViewModels, XDriverType driverType)
		{
			var rootObject = objectViewModels.FirstOrDefault(x => x.Device.DriverType == driverType);
			objectViewModels = new List<ObjectViewModel>();
			objectViewModels.Add(rootObject);
			AddChildren(objectViewModels, rootObject);
		}

		private static void AddChildren(List<ObjectViewModel> newobjectViewModels, ObjectViewModel rootObject)
		{
			rootObject.Children = rootObject.Children.OrderBy(x => x.Name).ToList().OrderBy(x => x.Address).ToList();
			foreach (var objectViewModel in rootObject.Children)
			{
				newobjectViewModels.Add(objectViewModel);
				if (objectViewModel.Children.Count() > 0)
					AddChildren(newobjectViewModels, objectViewModel);
			}
		}

		private static bool ContainsObject(ObjectViewModel objectViewModel, List<ObjectViewModel> objectViewModels)
		{
			var matchedObjectViewModel = objectViewModels.FirstOrDefault
				(x =>
				 (x.Name == objectViewModel.Name) &&
				 (x.Address == objectViewModel.Address) &&
				 ((x.Parent.Name == objectViewModel.Parent.Name) && (x.Parent.Address == objectViewModel.Parent.Address)));
			if ((matchedObjectViewModel != null) && (matchedObjectViewModel.HasDifferences == false))
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