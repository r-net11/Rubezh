using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class ObjectsListViewModel : BaseViewModel
	{
		public List<ObjectViewModel> Devices { get; set; }
		public List<ObjectViewModel> Zones { get; set; }
		public List<ObjectViewModel> Directions { get; set; }

		public ObjectsListViewModel (XDevice device, XDeviceConfiguration xDeviceConfiguration)
		{
			Devices = new List<ObjectViewModel>();
			Zones = new List<ObjectViewModel>();
			Directions = new List<ObjectViewModel>();

			var devices = xDeviceConfiguration.Devices;
			var zones = xDeviceConfiguration.Zones;
			var directions = xDeviceConfiguration.Directions;
			xDeviceConfiguration.Update();

			foreach (var childDevice in devices)
			{
				var objectViewModel = new ObjectViewModel(childDevice) { ObjectType = ObjectType.Device};
				var parent = childDevice.AllParents.FirstOrDefault(x => x.ShortName == device.ShortName && x.Address == device.Address);
				if (parent != null && childDevice.IsRealDevice)
					Devices.Add(objectViewModel);
			}

			if (zones != null)
				foreach (var zone in zones.Where(x => !x.IsEmpty))
				{
					var objectViewModel = new ObjectViewModel(zone) {ObjectType = ObjectType.Zone};
					Zones.Add(objectViewModel);
				}
			if (directions != null)
				foreach (var direction in directions.Where(x => !x.IsEmpty))
				{
					var objectViewModel = new ObjectViewModel(direction) { ObjectType = ObjectType.Direction };
					Directions.Add(objectViewModel);
				}
		}

		public static List<List<ObjectViewModel>> CompareTrees(List<ObjectViewModel> objects1, List<ObjectViewModel> objects2, XDriverType driverType)
		{
			var unionObjects = new List<ObjectViewModel>();
			foreach (var object1 in objects1)
			{
				var newObject = (ObjectViewModel)object1.Clone();
				unionObjects.Add(newObject);
			}
			foreach (var object2 in objects2)
			{
				if (!unionObjects.Any(x => x == object2))
					unionObjects.Add(object2);
			}
			unionObjects.Sort();
			//foreach (var object1 in objects1)
			//{
			//    if (!ContainsObject(object1, objects2))
			//    {
			//        var newObject = (ObjectViewModel)object1.Clone();
			//        newObject.HasDifferences = true;
			//        objects2.Add(newObject);
			//    }
			//}

			//foreach (var object2 in objects2)
			//{
			//    if (!ContainsObject(object2, objects1))
			//    {
			//        var newObject = (ObjectViewModel)object2.Clone();
			//        newObject.HasDifferences = true;
			//        objects1.Add(newObject);
			//    }
			//}

			//if ((objects1.Count != 0) && (!objects1.FirstOrDefault().IsDevice))
			//{
			//    objects1 = objects1.OrderBy(x => x.Name).ToList();
			//    objects2 = objects2.OrderBy(x => x.Name).ToList();
			//});
			return new List<List<ObjectViewModel>> { unionObjects, unionObjects };
		}


		private static bool ContainsObject(ObjectViewModel objectViewModel, List<ObjectViewModel> objectViewModels)
		{
			var matchedObjectViewModel = objectViewModels.FirstOrDefault (x => ObjectViewModel.Equels(x, objectViewModel));
			if ((matchedObjectViewModel != null) && (matchedObjectViewModel.HasDifferences == false))
				return true;
			return false;
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