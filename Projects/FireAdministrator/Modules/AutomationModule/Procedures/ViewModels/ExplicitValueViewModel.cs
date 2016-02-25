using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Media;
using FiresecAPI.Automation;
using FiresecAPI.GK;
using FiresecAPI.Journal;
using FiresecAPI.Models;
using FiresecAPI.SKD;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class ExplicitValueViewModel : BaseViewModel
	{
		public SKDDevice SKDDevice { get; private set; }
		public SKDZone SKDZone { get; private set; }
		public Camera Camera { get; private set; }
		public SKDDoor SKDDoor { get; private set; }
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
			Initialize(ExplicitValue.UidValue);
		}

		void InitializeEnums()
		{
			StateTypeValues = ProcedureHelper.GetEnumObs<XStateClass>();
			PermissionTypeValues = ProcedureHelper.GetEnumObs<PermissionType>();
			JournalEventNameTypeValues = ProcedureHelper.GetEnumObs<JournalEventNameType>();
			JournalEventDescriptionTypeValues = ProcedureHelper.GetEnumObs<JournalEventDescriptionType>();
			JournalObjectTypeValues = ProcedureHelper.GetEnumObs<JournalObjectType>();
			MinIntValue = Int32.MinValue;
			MaxIntValue = Int32.MaxValue;
		}

		public void Initialize(Guid uidValue)
		{
			SKDDevice = SKDManager.Devices.FirstOrDefault(x => x.UID == uidValue);
			SKDZone = SKDManager.Zones.FirstOrDefault(x => x.UID == uidValue);
			Camera = FiresecManager.SystemConfiguration.Cameras.FirstOrDefault(x => x.UID == uidValue);
			SKDDoor = SKDManager.Doors.FirstOrDefault(x => x.UID == uidValue);
			Organisation = OrganisationHelper.GetSingle(uidValue);
			base.OnPropertyChanged(() => PresentationName);
		}

		public string PresentationName
		{
			get
			{
				if (SKDDevice != null)
					return SKDDevice.Name;
				if (SKDZone != null)
					return SKDZone.PresentationName;
				if (Camera != null)
					return Camera.PresentationName;
				if (SKDDoor != null)
					return SKDDoor.PresentationName;
				if (Organisation != null)
					return Organisation.Name;
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

		public TimeSpan TimeSpanValue
		{
			get { return ExplicitValue.TimeSpanValue; }
			set
			{
				ExplicitValue.TimeSpanValue = value;
				OnPropertyChanged(() => TimeSpanValue);
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
				return ((SKDDevice == null) && (SKDZone == null) && (Camera == null) && (SKDDoor == null) && (Organisation == null));
			}
			set
			{
				if (value)
					SKDDevice = null; SKDZone = null; Camera = null; SKDDoor = null; Organisation = null;
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