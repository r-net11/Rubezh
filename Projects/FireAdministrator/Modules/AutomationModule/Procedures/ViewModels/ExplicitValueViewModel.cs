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
		public XDevice Device { get; private set; }
		public XZone Zone { get; private set; }
		public XGuardZone GuardZone { get; private set; }
		public SKDDevice SKDDevice { get; private set; }
		public SKDZone SKDZone { get; private set; }
		public Camera Camera { get; private set; }
		public SKDDoor SKDDoor { get; private set; }
		public XDirection Direction { get; private set; }
		public ExplicitValue ExplicitValue { get; private set; }
		public Action UpdateDescriptionHandler { get; set; }
		public Action UpdateObjectHandler { get; set; }

		public ExplicitValueViewModel(ExplicitValue explicitValue)
		{
			ExplicitValue = explicitValue;
			Initialize(ExplicitValue.UidValue);
		}

		public void Initialize(Guid uidValue)
		{
			StateTypeValues = ProcedureHelper.GetEnumObs<XStateClass>();
			DriverTypeValues = ProcedureHelper.GetEnumObs<XDriverType>();
			Device = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == uidValue);
			Zone = XManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.UID == uidValue);
			GuardZone = XManager.DeviceConfiguration.GuardZones.FirstOrDefault(x => x.UID == uidValue);
			SKDDevice = SKDManager.Devices.FirstOrDefault(x => x.UID == uidValue);
			SKDZone = SKDManager.Zones.FirstOrDefault(x => x.UID == uidValue);
			Camera = FiresecManager.SystemConfiguration.AllCameras.FirstOrDefault(x => x.UID == uidValue);
			SKDDoor = SKDManager.Doors.FirstOrDefault(x => x.UID == uidValue);
			Direction = XManager.DeviceConfiguration.Directions.FirstOrDefault(x => x.UID == uidValue);
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
				if (UpdateObjectHandler != null)
					UpdateObjectHandler();
				OnPropertyChanged(() => UidValue);
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

		public ObservableCollection<XDriverType> DriverTypeValues { get; private set; }
		public XDriverType DriverTypeValue
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

		public new void OnPropertyChanged<T>(Expression<Func<T>> propertyExpression)
		{
			ServiceFactory.SaveService.AutomationChanged = true;
			base.OnPropertyChanged(propertyExpression);
			if (UpdateDescriptionHandler != null)
				UpdateDescriptionHandler();
		}
	}
}
