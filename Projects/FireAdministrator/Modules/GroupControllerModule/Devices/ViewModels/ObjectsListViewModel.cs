using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using FiresecClient;

namespace GKModule.ViewModels
{
	public class ObjectsListViewModel : BaseViewModel
	{

		public ObjectsListViewModel (XDevice device, XDeviceConfiguration xDeviceConfiguration)
		{
			var devices = xDeviceConfiguration.Devices;
			var zones = xDeviceConfiguration.Zones;
			var directions = xDeviceConfiguration.Directions;
			xDeviceConfiguration.Update();
			Objects = new List<ObjectViewModel>();

			foreach (var childDevice in devices)
			{
				var objectViewModel = new ObjectViewModel(childDevice) { ObjectType = ObjectType.Device};
				var parent = childDevice.AllParents.FirstOrDefault(x => x.ShortName == device.ShortName && x.Address == device.Address);
				if (parent != null && childDevice.IsRealDevice)
					Objects.Add(objectViewModel);
			}
			if (zones != null)
				foreach (var zone in zones.Where(x => x.GkDatabaseParent != null && x.GkDatabaseParent.Address == device.Address))
				{
					var objectViewModel = new ObjectViewModel(zone) {ObjectType = ObjectType.Zone};
					Objects.Add(objectViewModel);
				}
			if (directions != null)
				foreach (var direction in directions.Where(x => x.GkDatabaseParent != null && x.GkDatabaseParent.Address == device.Address))
				{
					var objectViewModel = new ObjectViewModel(direction) { ObjectType = ObjectType.Direction };
					Objects.Add(objectViewModel);
				}
		}

		public static List<List<ObjectViewModel>> CompareTrees(List<ObjectViewModel> objects1, List<ObjectViewModel> objects2, XDriverType driverType)
		{
			var unionObjects = objects1.Select(object1 => (ObjectViewModel) object1.Clone()).ToList();

			foreach (var object2 in objects2)
			{
				if (!unionObjects.Any(x => x.Compare(x, object2) == 0))
				    unionObjects.Add(object2);
			}
			unionObjects.Sort();

			var unionObjects1 = new List<ObjectViewModel>();
			foreach (var unionObject in unionObjects)
			{
				var newObject = (ObjectViewModel)unionObject.Clone();
				if (!objects1.Any(x => x.Compare(x, unionObject) == 0))
					newObject.HasDifferences = true;
				unionObjects1.Add(newObject);
			}

			var unionObjects2 = new List<ObjectViewModel>();
			foreach (var unionObject in unionObjects)
			{
				var newObject = (ObjectViewModel)unionObject.Clone();
				if (!objects2.Any(x => x.Compare(x, unionObject) == 0))
					newObject.HasDifferences = true;
				unionObjects2.Add(newObject);
			}

			return new List<List<ObjectViewModel>> { unionObjects1, unionObjects2 };
		}

		public List<ObjectViewModel> Objects { get; set; }

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