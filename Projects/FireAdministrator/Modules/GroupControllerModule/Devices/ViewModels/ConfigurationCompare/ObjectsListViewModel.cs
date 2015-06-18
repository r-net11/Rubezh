using System.Collections.Generic;
using System.Linq;
using FiresecAPI.GK;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class ObjectsListViewModel : BaseViewModel
	{
		public ObjectsListViewModel (GKDevice device, GKDeviceConfiguration deviceConfiguration)
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
			if (deviceConfiguration.PumpStations != null)
				foreach (var pumpStation in deviceConfiguration.PumpStations.Where(x => x.GkDatabaseParent != null && x.GkDatabaseParent.Address == device.Address))
				{
					var objectViewModel = new ObjectViewModel(pumpStation);
					Objects.Add(objectViewModel);
				}
			if (deviceConfiguration.Doors != null)
				foreach (var door in deviceConfiguration.Doors.Where(x => x.GkDatabaseParent != null && x.GkDatabaseParent.Address == device.Address))
				{
					var objectViewModel = new ObjectViewModel(door);
					Objects.Add(objectViewModel);
				}
			//if (deviceConfiguration.SKDZones != null)
			//    foreach (var skdZones in deviceConfiguration.SKDZones.Where(x => x.GkDatabaseParent != null && x.GkDatabaseParent.Address == device.Address))
			//    {
			//        var objectViewModel = new ObjectViewModel(door);
			//        Objects.Add(objectViewModel);
			//    }
		}

		public List<ObjectViewModel> Objects { get; set; }

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
	}
}