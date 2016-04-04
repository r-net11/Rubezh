using Infrastructure;
using Infrastructure.Automation;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI;
using RubezhAPI.Automation;
using RubezhAPI.GK;
using RubezhAPI.Journal;
using RubezhAPI.Models;
using RubezhAPI.SKD;
using RubezhClient;
using RubezhClient.SKDHelpers;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Media;

namespace AutomationModule.ViewModels
{
	public class ExplicitValueViewModel : BaseViewModel
	{
		public GKDevice Device { get; private set; }
		public GKDelay Delay { get; private set; }
		public GKZone Zone { get; private set; }
		public GKGuardZone GuardZone { get; private set; }
		public Camera Camera { get; private set; }
		public GKDoor GKDoor { get; private set; }
		public GKDirection Direction { get; private set; }
		public GKPumpStation PumpStation { get; private set; }
		public GKMPT MPT { get; private set; }
		public Organisation Organisation { get; private set; }
		public ExplicitValue ExplicitValue { get; private set; }
		public Action UpdateDescriptionHandler { get; set; }
		public Action UpdateObjectHandler { get; set; }

		public ExplicitValueViewModel()
		{
			ExplicitValue = new ExplicitValue();
			InitializeEnums();
		}

		public ExplicitValueViewModel(ExplicitValue explicitValue)
		{
			ExplicitValue = explicitValue;
			InitializeEnums();
			Initialize(ExplicitValue.ObjectReferenceValue);
		}

		void InitializeEnums()
		{
			StateTypeValues = AutomationHelper.GetEnumObs<XStateClass>();
			DriverTypeValues = AutomationHelper.GetEnumObs<GKDriverType>();
			PermissionTypeValues = AutomationHelper.GetEnumObs<PermissionType>();
			JournalEventNameTypeValues = AutomationHelper.GetEnumObs<JournalEventNameType>();
			JournalEventDescriptionTypeValues = AutomationHelper.GetEnumObs<JournalEventDescriptionType>();
			JournalObjectTypeValues = AutomationHelper.GetEnumObs<JournalObjectType>();
			MinIntValue = Int32.MinValue;
			MaxIntValue = Int32.MaxValue;
		}

		public void Initialize(ObjectReference objRef)
		{
			Device = null;
			Zone = null;
			GuardZone = null;
			Camera = null;
			GKDoor = null;
			Direction = null;
			PumpStation = null;
			MPT = null;
			Delay = null;
			Organisation = null;

			if (objRef.UID != Guid.Empty)
			{
				switch (objRef.ObjectType)
				{
					case ObjectType.Device:
						Device = GKManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == objRef.UID);
						break;
					case ObjectType.Zone:
						Zone = GKManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.UID == objRef.UID);
						break;
					case ObjectType.Direction:
						Direction = GKManager.DeviceConfiguration.Directions.FirstOrDefault(x => x.UID == objRef.UID);
						break;
					case ObjectType.Delay:
						Delay = GKManager.DeviceConfiguration.Delays.FirstOrDefault(x => x.UID == objRef.UID);
						break;
					case ObjectType.GuardZone:
						GuardZone = GKManager.DeviceConfiguration.GuardZones.FirstOrDefault(x => x.UID == objRef.UID);
						break;
					case ObjectType.PumpStation:
						PumpStation = GKManager.DeviceConfiguration.PumpStations.FirstOrDefault(x => x.UID == objRef.UID);
						break;
					case ObjectType.MPT:
						MPT = GKManager.DeviceConfiguration.MPTs.FirstOrDefault(x => x.UID == objRef.UID);
						break;
					case ObjectType.VideoDevice:
						Camera = ClientManager.SystemConfiguration.Cameras.FirstOrDefault(x => x.UID == objRef.UID);
						break;
					case ObjectType.GKDoor:
						GKDoor = GKManager.Doors.FirstOrDefault(x => x.UID == objRef.UID);
						break;
					case ObjectType.Organisation:
						Organisation = OrganisationHelper.GetSingle(objRef.UID);
						break;
				}
			}
			base.OnPropertyChanged(() => PresentationName);
		}

		public string PresentationName
		{
			get
			{
				return ExplicitValue.ToString();
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
				var tmp = ExplicitValue.DateTimeValue;
				ExplicitValue.DateTimeValue = new DateTime(value.Year, value.Month, value.Day, tmp.Hour, tmp.Minute, 0);
				OnPropertyChanged(() => DateTimeValue);
			}
		}

		public TimeSpan TimeValue
		{
			get { return ExplicitValue.DateTimeValue.TimeOfDay; }
			set
			{
				var tmp = ExplicitValue.DateTimeValue;
				ExplicitValue.DateTimeValue = new DateTime(tmp.Year, tmp.Month, tmp.Day, value.Hours, value.Minutes, 0);
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

		public double FloatValue
		{
			get { return ExplicitValue.FloatValue; }
			set
			{
				ExplicitValue.FloatValue = value;
				OnPropertyChanged(() => FloatValue);
			}
		}

		int _minIntValue;
		public int MinIntValue
		{
			get { return _minIntValue; }
			set
			{
				_minIntValue = value;
				OnPropertyChanged(() => MinIntValue);
			}
		}

		int _maxIntValue;
		public int MaxIntValue
		{
			get { return _maxIntValue; }
			set
			{
				_maxIntValue = value;
				OnPropertyChanged(() => MaxIntValue);
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

		public ObjectReference ObjectReferenceValue
		{
			get { return ExplicitValue.ObjectReferenceValue; }
			set
			{
				if (ExplicitValue.ObjectReferenceValue != value)
				{
					ExplicitValue.ObjectReferenceValue = value;
					Initialize(value);
					if (UpdateObjectHandler != null)
						UpdateObjectHandler();
					OnPropertyChanged(() => ObjectReferenceValue);
					OnPropertyChanged(() => IsEmpty);
				}
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

		public ObservableCollection<PermissionType> PermissionTypeValues { get; private set; }
		public PermissionType PermissionTypeValue
		{
			get { return ExplicitValue.PermissionTypeValue; }
			set
			{
				ExplicitValue.PermissionTypeValue = value;
				OnPropertyChanged(() => PermissionTypeValue);
			}
		}

		public ObservableCollection<JournalEventNameType> JournalEventNameTypeValues { get; private set; }
		public JournalEventNameType JournalEventNameTypeValue
		{
			get { return ExplicitValue.JournalEventNameTypeValue; }
			set
			{
				ExplicitValue.JournalEventNameTypeValue = value;
				OnPropertyChanged(() => JournalEventNameTypeValue);
			}
		}

		public ObservableCollection<JournalEventDescriptionType> JournalEventDescriptionTypeValues { get; private set; }
		public JournalEventDescriptionType JournalEventDescriptionTypeValue
		{
			get { return ExplicitValue.JournalEventDescriptionTypeValue; }
			set
			{
				ExplicitValue.JournalEventDescriptionTypeValue = value;
				OnPropertyChanged(() => JournalEventDescriptionTypeValue);
			}
		}

		public Color ColorValue
		{
			get { return ExplicitValue.ColorValue; }
			set
			{
				ExplicitValue.ColorValue = value;
				OnPropertyChanged(() => ColorValue);
			}
		}

		public ObservableCollection<JournalObjectType> JournalObjectTypeValues { get; private set; }
		public JournalObjectType JournalObjectTypeValue
		{
			get { return ExplicitValue.JournalObjectTypeValue; }
			set
			{
				ExplicitValue.JournalObjectTypeValue = value;
				OnPropertyChanged(() => PermissionTypeValue);
			}
		}

		public bool IsEmpty
		{
			get
			{
				return ((Device == null) && (Zone == null) && (GuardZone == null) && (Camera == null) && (Direction == null) && (GKDoor == null) && (Delay == null) && (PumpStation == null) && (MPT == null) && (Organisation == null));
			}
			set
			{
				if (value)
					Device = null; Zone = null; GuardZone = null; Camera = null; Direction = null; GKDoor = null; Delay = null; PumpStation = null; MPT = null; Organisation = null;
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