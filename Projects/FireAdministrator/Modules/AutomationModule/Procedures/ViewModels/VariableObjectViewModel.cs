using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.Windows.ViewModels;
using FiresecAPI.GK;
using FiresecClient;
using FiresecAPI.SKD;
using FiresecAPI.Models;
using System.Collections.ObjectModel;
using ValueType = FiresecAPI.Automation.ValueType;

namespace AutomationModule.ViewModels
{
	public class VariableItemViewModel : BaseViewModel
	{
		public XDevice Device { get; private set; }
		public XZone Zone { get; private set; }
		public XGuardZone GuardZone { get; private set; }
		public SKDDevice SKDDevice { get; private set; }
		public SKDZone SKDZone { get; private set; }
		public Camera Camera { get; private set; }
		public SKDDoor SKDDoor { get; private set; }
		public XDirection Direction { get; private set; }

		public bool BoolValue { get; private set; }
		public ValueType ValueType { get; set; }
		public Guid ObjectUid { get; private set; }

		public VariableItemViewModel(Guid objectUid)
		{
			ObjectUid = objectUid;
			Device = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == objectUid);
			Zone = XManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.UID == objectUid);
			GuardZone = XManager.DeviceConfiguration.GuardZones.FirstOrDefault(x => x.UID == objectUid);
			SKDDevice = SKDManager.Devices.FirstOrDefault(x => x.UID == objectUid);
			SKDZone = SKDManager.Zones.FirstOrDefault(x => x.UID == objectUid);
			Camera = FiresecManager.SystemConfiguration.AllCameras.FirstOrDefault(x => x.UID == objectUid);
			SKDDoor = SKDManager.Doors.FirstOrDefault(x => x.UID == objectUid);
			Direction = XManager.DeviceConfiguration.Directions.FirstOrDefault(x => x.UID == objectUid);
		}

		public void Copy(VariableItemViewModel variableItemViewModel)
		{
			Device = variableItemViewModel.Device;
			Zone = variableItemViewModel.Zone;
			GuardZone = variableItemViewModel.GuardZone;
			SKDDevice = variableItemViewModel.SKDDevice;
			SKDZone = variableItemViewModel.SKDZone;
			Camera = variableItemViewModel.Camera;
			SKDDoor = variableItemViewModel.SKDDoor;
			Direction = variableItemViewModel.Direction;
			ObjectUid = variableItemViewModel.ObjectUid;
			OnPropertyChanged(() => PresentationName);
			OnPropertyChanged(() => IsEmpty);
		}

		public string PresentationName
		{
			get
			{
				if (Device != null)
					return Device.PresentationName;
				if (Zone != null)
					return Zone.PresentationName;
				if (GuardZone != null)
					return GuardZone.PresentationName;
				if (SKDDevice != null)
					return SKDDevice.Name;
				if (SKDZone != null)
					return SKDZone.Name;
				if (Camera != null)
					return Camera.Name;
				if (SKDDoor != null)
					return SKDDoor.Name;
				if (Direction != null)
					return Direction.PresentationName;
				return "";
			}
		}

		public bool IsEmpty
		{
			get
			{
				return ((Device == null) && (Zone == null) && (GuardZone == null) && (SKDDevice == null) && (SKDZone == null) && (Camera == null) && (Direction == null));
			}
			set
			{
				if (value)
					Device = null; Zone = null; GuardZone = null; SKDDevice = null; SKDZone = null; Camera = null; Direction = null;
			}
		}
	}
}
