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
			Objects = new ObservableCollection<ObjectViewModel>();
			IitializeObjects(device, zones, directions);
		}
		void IitializeObjects(XDevice device, List<XZone> zones, List<XDirection> directions)
		{
			if (device.Children != null)
			{
				InitializeDevices(device);
			}
			foreach (var zone in zones)
			{
				var objectViewModel = new ObjectViewModel(zone);
				Objects.Add(objectViewModel);
			}
			foreach (var direction in directions)
			{
				var objectViewModel = new ObjectViewModel(direction);
				Objects.Add(objectViewModel);
			}
		}

		ObjectViewModel InitializeDevices(XDevice device)
		{
			var objectViewModel = new ObjectViewModel(device);
			Objects.Add(objectViewModel);
			foreach (var childDevice in device.Children)
			{
				InitializeDevices(childDevice);
				//var childObjectViewModel = InitializeDevices(childDevice);
				//Objects.Add(childObjectViewModel);
			}
			return objectViewModel;
		}

		public static void CompareTrees(DeviceTreeViewModel deviceTreeViewModel1, DeviceTreeViewModel deviceTreeViewModel2)
		{
			var objects1 = deviceTreeViewModel1.Objects;
			var objects2 = deviceTreeViewModel2.Objects;
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

				if (!CompareObjects(objects1[i], objects2[i]))
				{
					var object1 = (ObjectViewModel)objects1[i].Clone();
					object1.HasDifferences = true;
					var object2 = (ObjectViewModel)objects2[i].Clone();
					object2.HasDifferences = true;
					if((object1.PresentationAddress == "")||(object2.PresentationAddress == ""))
						continue;
					if ((object1.Device != null) && (object2.Device != null) && (object1.Device.IntAddress > object2.Device.IntAddress))
					{
						objects1.Insert(i, object2);
						objects2.Insert(i + 1, object1);
					}
					else
					{
						objects1.Insert(i + 1, object2);
						objects2.Insert(i, object1);
					}
					i++;
				}
			}
		}

		private static bool CompareObjects(ObjectViewModel objectViewModel1, ObjectViewModel objectViewModel2)
		{
			if(objectViewModel1.PresentationName != objectViewModel2.PresentationName)
				return false;
			if (objectViewModel1.PresentationAddress != objectViewModel2.PresentationAddress)
				return false;
			return true;
		}

		static void ExpandChild(DeviceViewModel parentDeviceViewModel)
		{
			parentDeviceViewModel.IsExpanded = true;
			foreach (var deviceViewModel in parentDeviceViewModel.Children)
			{
				ExpandChild(deviceViewModel);
			}
		}
		public ObservableCollection<ObjectViewModel> Objects { get; set; }

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
