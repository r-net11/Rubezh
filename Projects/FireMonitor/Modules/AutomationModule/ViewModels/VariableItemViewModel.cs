using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.GK;
using FiresecAPI.SKD;
using FiresecAPI.Models;
using FiresecAPI.Automation;
using FiresecClient;

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
		public VariableItem VariableItem { get; private set; }
		public Action UpdateDescriptionHandler { get; set; }

		public VariableItemViewModel(VariableItem variableItem)
		{
			VariableItem = variableItem;
			Initialize(variableItem);
		}

		public void Initialize(VariableItem variableItem)
		{
			PropertyCopy.Copy<VariableItem, VariableItem>(variableItem, VariableItem);
			StateTypeValues = GetEnumObs<XStateClass>();
			DriverTypeValues = GetEnumObs<XDriverType>();
			var objectUid = variableItem.UidValue;
			Device = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == objectUid);
			Zone = XManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.UID == objectUid);
			GuardZone = XManager.DeviceConfiguration.GuardZones.FirstOrDefault(x => x.UID == objectUid);
			SKDDevice = SKDManager.Devices.FirstOrDefault(x => x.UID == objectUid);
			SKDZone = SKDManager.Zones.FirstOrDefault(x => x.UID == objectUid);
			Camera = FiresecManager.SystemConfiguration.AllCameras.FirstOrDefault(x => x.UID == objectUid);
			SKDDoor = SKDManager.Doors.FirstOrDefault(x => x.UID == objectUid);
			Direction = XManager.DeviceConfiguration.Directions.FirstOrDefault(x => x.UID == objectUid);
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

		public bool BoolValue
		{
			get { return VariableItem.BoolValue; }
			set
			{
				VariableItem.BoolValue = value;
				OnPropertyChanged(() => BoolValue);
			}
		}

		public DateTime DateTimeValue
		{
			get { return VariableItem.DateTimeValue; }
			set
			{
				VariableItem.DateTimeValue = value;
				OnPropertyChanged(() => DateTimeValue);
			}
		}

		public int IntValue
		{
			get { return VariableItem.IntValue; }
			set
			{
				VariableItem.IntValue = value;
				OnPropertyChanged(() => IntValue);
			}
		}

		public string StringValue
		{
			get { return VariableItem.StringValue; }
			set
			{
				VariableItem.StringValue = value;
				OnPropertyChanged(() => StringValue);
			}
		}

		public ObservableCollection<XStateClass> StateTypeValues { get; private set; }
		public XStateClass StateTypeValue
		{
			get { return VariableItem.StateTypeValue; }
			set
			{
				VariableItem.StateTypeValue = value;
				OnPropertyChanged(() => StateTypeValue);
			}
		}

		public ObservableCollection<XDriverType> DriverTypeValues { get; private set; }
		public XDriverType DriverTypeValue
		{
			get { return VariableItem.DriverTypeValue; }
			set
			{
				VariableItem.DriverTypeValue = value;
				OnPropertyChanged(() => DriverTypeValue);
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

		public static ObservableCollection<T> GetEnumObs<T>()
		{
			return new ObservableCollection<T>(Enum.GetValues(typeof(T)).Cast<T>().ToList());
		}
	}
}
