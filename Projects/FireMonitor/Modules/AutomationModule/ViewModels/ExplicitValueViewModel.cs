using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.GK;
using FiresecAPI.SKD;
using FiresecAPI.Models;
using FiresecAPI.Automation;
using FiresecClient;
using System.Collections.ObjectModel;
using Infrastructure.Common;

namespace AutomationModule.ViewModels
{
	public class ExplicitValueViewModel : BaseViewModel
	{
		public GKDevice Device { get; private set; }
		public GKZone Zone { get; private set; }
		public GKGuardZone GuardZone { get; private set; }
		public SKDDevice SKDDevice { get; private set; }
		public SKDZone SKDZone { get; private set; }
		public Camera Camera { get; private set; }
		public SKDDoor SKDDoor { get; private set; }
		public GKDirection Direction { get; private set; }
		public ExplicitValue ExplicitValue { get; private set; }

		public ExplicitValueViewModel()
		{
			ExplicitValue = new ExplicitValue();
			StateTypeValues = ProcedureHelper.GetEnumObs<XStateClass>();
			DriverTypeValues = ProcedureHelper.GetEnumObs<GKDriverType>();
		}

		public ExplicitValueViewModel(ExplicitValue explicitValue)
		{
			ExplicitValue = explicitValue;
			StateTypeValues = ProcedureHelper.GetEnumObs<XStateClass>();
			DriverTypeValues = ProcedureHelper.GetEnumObs<GKDriverType>();
			Initialize(ExplicitValue.UidValue);
		}

		public void Initialize(Guid uidValue)
		{
			Device = GKManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == uidValue);
			Zone = GKManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.UID == uidValue);
			GuardZone = GKManager.DeviceConfiguration.GuardZones.FirstOrDefault(x => x.UID == uidValue);
			SKDDevice = SKDManager.Devices.FirstOrDefault(x => x.UID == uidValue);
			SKDZone = SKDManager.Zones.FirstOrDefault(x => x.UID == uidValue);
			Camera = FiresecManager.SystemConfiguration.AllCameras.FirstOrDefault(x => x.UID == uidValue);
			SKDDoor = SKDManager.Doors.FirstOrDefault(x => x.UID == uidValue);
			Direction = GKManager.DeviceConfiguration.Directions.FirstOrDefault(x => x.UID == uidValue);
			base.OnPropertyChanged(() => PresentationName);
		}

		public string PresentationName
		{
			get
			{
				if (Device != null)
					return (Device.PresentationName);
				if (Zone != null)
					return Zone.PresentationName;
				if (GuardZone != null)
					return GuardZone.PresentationName;
				if (SKDDevice != null)
					return SKDDevice.Name;
				if (SKDZone != null)
					return SKDZone.PresentationName;
				if (Camera != null)
					return Camera.PresentationName;
				if (SKDDoor != null)
					return SKDDoor.PresentationName;
				if (Direction != null)
					return Direction.PresentationName;
				return "";
			}
		}

		public bool BoolValue
		{
			get { return ExplicitValue.BoolValue; }
			set
			{
				ExplicitValue.BoolValue = value;
				OnPropertyChanged(() => BoolValue);
			}
		}

		public DateTime DateTimeValue
		{
			get { return ExplicitValue.DateTimeValue; }
			set
			{
				ExplicitValue.DateTimeValue = value;
				OnPropertyChanged(() => DateTimeValue);
			}
		}

		public int IntValue
		{
			get { return ExplicitValue.IntValue; }
			set
			{
				ExplicitValue.IntValue = value;
				OnPropertyChanged(() => IntValue);
			}
		}

		public string StringValue
		{
			get { return ExplicitValue.StringValue; }
			set
			{
				ExplicitValue.StringValue = value;
				OnPropertyChanged(() => StringValue);
			}
		}

		public Guid UidValue
		{
			get { return ExplicitValue.UidValue; }
			set
			{
				ExplicitValue.UidValue = value;
				Initialize(value);
				OnPropertyChanged(() => UidValue);
				OnPropertyChanged(() => IsEmpty);
			}
		}

		public ObservableCollection<XStateClass> StateTypeValues { get; private set; }
		public XStateClass StateTypeValue
		{
			get { return ExplicitValue.StateTypeValue; }
			set
			{
				ExplicitValue.StateTypeValue = value;
				OnPropertyChanged(() => StateTypeValue);
			}
		}

		public ObservableCollection<GKDriverType> DriverTypeValues { get; private set; }
		public GKDriverType DriverTypeValue
		{
			get { return ExplicitValue.DriverTypeValue; }
			set
			{
				ExplicitValue.DriverTypeValue = value;
				OnPropertyChanged(() => DriverTypeValue);
			}
		}

		public bool IsEmpty
		{
			get
			{
				return ((Device == null) && (Zone == null) && (GuardZone == null) && (SKDDevice == null) && (SKDZone == null) && (Camera == null) && (Direction == null) && (SKDDoor == null));
			}
			set
			{
				if (value)
					Device = null; Zone = null; GuardZone = null; SKDDevice = null; SKDZone = null; Camera = null; Direction = null; SKDDoor = null;
			}
		}
	}
}
