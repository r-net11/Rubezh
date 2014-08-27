using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.GK;
using FiresecClient;
using FiresecAPI.SKD;
using FiresecAPI.Models;

namespace AutomationModule.ViewModels
{
	public class VariableObjectViewModel : BaseViewModel
	{
		public XDevice Device { get; private set; }
		public XZone Zone { get; private set; }
		public XGuardZone GuardZone { get; private set; }
		public SKDDevice SKDDevice { get; private set; }
		public SKDZone SKDZone { get; private set; }
		public Camera Camera { get; private set; }
		public SKDDoor SKDDoor { get; private set; }
		public XDirection Direction { get; private set; }

		public VariableObjectViewModel(Guid objectUid)
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

		public Guid ObjectUid { get; private set; }

		public void Copy(VariableObjectViewModel variableObjectViewModel)
		{
			Device = variableObjectViewModel.Device;
			Zone = variableObjectViewModel.Zone;
			GuardZone = variableObjectViewModel.GuardZone;
			SKDDevice = variableObjectViewModel.SKDDevice;
			SKDZone = variableObjectViewModel.SKDZone;
			Camera = variableObjectViewModel.Camera;
			SKDDoor = variableObjectViewModel.SKDDoor;
			Direction = variableObjectViewModel.Direction;
			ObjectUid = variableObjectViewModel.ObjectUid;
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
