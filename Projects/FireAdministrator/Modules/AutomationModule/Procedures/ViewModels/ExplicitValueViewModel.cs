using System;
using System.Linq;
using System.Linq.Expressions;
using FiresecAPI.Journal;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.GK;
using FiresecAPI.SKD;
using FiresecAPI.Models;
using FiresecAPI.Automation;
using FiresecClient;
using System.Collections.ObjectModel;

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
			Initialize(ExplicitValue.UidValue);
		}

		void InitializeEnums()
		{
			StateTypeValues = ProcedureHelper.GetEnumObs<XStateClass>();
			DriverTypeValues = ProcedureHelper.GetEnumObs<GKDriverType>();
			PermissionTypeValues = ProcedureHelper.GetEnumObs<PermissionType>();
			JournalEventNameTypeValues = ProcedureHelper.GetEnumObs<JournalEventNameType>();
			JournalEventDescriptionTypeValues = ProcedureHelper.GetEnumObs<JournalEventDescriptionType>();
			JournalObjectTypeValues = ProcedureHelper.GetEnumObs<JournalObjectType>();
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
				return "Null";
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