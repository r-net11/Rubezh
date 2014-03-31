using System.Collections.Generic;
using System.Linq;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class ObjectsListViewModel : BaseViewModel
	{
		public ObjectsListViewModel (XDevice device, XDeviceConfiguration deviceConfiguration)
		{
			deviceConfiguration.Update();
			Objects = new List<ObjectViewModel>();

			foreach (var childDevice in deviceConfiguration.Devices)
			{
				var objectViewModel = new ObjectViewModel(childDevice);
				var parent = childDevice.AllParents.FirstOrDefault(x => x.ShortName == device.ShortName && x.Address == device.Address);
				if (parent != null && childDevice.IsRealDevice)
					Objects.Add(objectViewModel);
			}
			if (deviceConfiguration.Zones != null)
				foreach (var zone in deviceConfiguration.Zones.Where(x => x.GkDatabaseParent != null && x.GkDatabaseParent.Address == device.Address))
				{
					var objectViewModel = new ObjectViewModel(zone);
					Objects.Add(objectViewModel);
				}
			if (deviceConfiguration.Directions != null)
				foreach (var direction in deviceConfiguration.Directions.Where(x => x.GkDatabaseParent != null && x.GkDatabaseParent.Address == device.Address))
				{
					var objectViewModel = new ObjectViewModel(direction);
					Objects.Add(objectViewModel);
				}
			if (deviceConfiguration.PumpStations != null)
				foreach (var pumpStation in deviceConfiguration.PumpStations.Where(x => x.GkDatabaseParent != null && x.GkDatabaseParent.Address == device.Address))
				{
					var objectViewModel = new ObjectViewModel(pumpStation);
					Objects.Add(objectViewModel);
				}
			if (deviceConfiguration.MPTs != null)
				foreach (var mpt in deviceConfiguration.MPTs.Where(x => x.GkDatabaseParent != null && x.GkDatabaseParent.Address == device.Address))
				{
					var objectViewModel = new ObjectViewModel(mpt);
					Objects.Add(objectViewModel);
				}
			if (deviceConfiguration.Delays != null)
				foreach (var delay in deviceConfiguration.Delays.Where(x => x.GkDatabaseParent != null && x.GkDatabaseParent.Address == device.Address))
				{
					var objectViewModel = new ObjectViewModel(delay);
					Objects.Add(objectViewModel);
				}
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