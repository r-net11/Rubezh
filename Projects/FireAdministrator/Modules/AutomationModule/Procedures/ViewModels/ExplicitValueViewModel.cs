using Common;
using Infrastructure;
using Infrastructure.Automation;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using RubezhAPI.Automation;
using RubezhAPI.GK;
using RubezhAPI.Journal;
using RubezhAPI.Models;
using RubezhAPI.SKD;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

namespace AutomationModule.ViewModels
{
	public class ExplicitValueViewModel : BaseViewModel
	{
		public bool IsLabelDisabled { get; set; }
		public ExplicitValue ExplicitValue { get; private set; }
		public Action UpdateDescriptionHandler { get; set; }
		public Action UpdateObjectHandler { get; set; }

		public ExplicitValueViewModel(ExplicitValue explicitValue, bool isLabelDisabled = false)
		{
			ExplicitValue = explicitValue;
			ChangeCommand = new RelayCommand<ObjectReferenceViewModel>(OnChange);
			AddCommand = new RelayCommand(OnAdd);
			RemoveCommand = new RelayCommand<object>(OnRemove);
			EditStringCommand = new RelayCommand(OnEditString);
			EditListCommand = new RelayCommand(OnEditList);

			StateTypeValues = AutomationHelper.GetEnumObs<XStateClass>();
			DriverTypeValues = AutomationHelper.GetEnumObs<GKDriverType>();
			PermissionTypeValues = AutomationHelper.GetEnumObs<PermissionType>();
			JournalEventNameTypeValues = AutomationHelper.GetEnumObs<JournalEventNameType>();
			JournalEventDescriptionTypeValues = AutomationHelper.GetEnumObs<JournalEventDescriptionType>();
			JournalObjectTypeValues = AutomationHelper.GetEnumObs<JournalObjectType>();
			MinIntValue = Int32.MinValue;
			MaxIntValue = Int32.MaxValue;

			IntValueList = new ObservableCollection<ExplicitValueInstanceViewModel<int>>(explicitValue.IntValueList.Select(x => new ExplicitValueInstanceViewModel<int>(x)));
			FloatValueList = new ObservableCollection<ExplicitValueInstanceViewModel<double>>(explicitValue.FloatValueList.Select(x => new ExplicitValueInstanceViewModel<double>(x)));
			BoolValueList = new ObservableCollection<ExplicitValueInstanceViewModel<bool>>(explicitValue.BoolValueList.Select(x => new ExplicitValueInstanceViewModel<bool>(x)));
			DateTimeValueList = new ObservableCollection<ExplicitValueInstanceViewModel<DateTime>>(explicitValue.DateTimeValueList.Select(x => new ExplicitValueInstanceViewModel<DateTime>(x)));
			ObjectReferenceValueList = new ObservableCollection<ObjectReferenceViewModel>(explicitValue.UidValueList.Select(x => new ObjectReferenceViewModel(new ObjectReference { UID = x, ObjectType = explicitValue.ObjectType })));
			StringValueList = new ObservableCollection<ExplicitValueInstanceViewModel<string>>(explicitValue.StringValueList.Select(x => new ExplicitValueInstanceViewModel<string>(x)));
			StateTypeValueList = new ObservableCollection<ExplicitValueInstanceViewModel<XStateClass>>(explicitValue.StateTypeValueList.Select(x => new ExplicitValueInstanceViewModel<XStateClass>(x)));
			DriverTypeValueList = new ObservableCollection<ExplicitValueInstanceViewModel<GKDriverType>>(explicitValue.DriverTypeValueList.Select(x => new ExplicitValueInstanceViewModel<GKDriverType>(x)));
			PermissionTypeValueList = new ObservableCollection<ExplicitValueInstanceViewModel<PermissionType>>(explicitValue.PermissionTypeValueList.Select(x => new ExplicitValueInstanceViewModel<PermissionType>(x)));
			JournalEventNameTypeValueList = new ObservableCollection<ExplicitValueInstanceViewModel<JournalEventNameType>>(explicitValue.JournalEventNameTypeValueList.Select(x => new ExplicitValueInstanceViewModel<JournalEventNameType>(x)));
			JournalEventDescriptionTypeValueList = new ObservableCollection<ExplicitValueInstanceViewModel<JournalEventDescriptionType>>(explicitValue.JournalEventDescriptionTypeValueList.Select(x => new ExplicitValueInstanceViewModel<JournalEventDescriptionType>(x)));
			JournalObjectTypeValueList = new ObservableCollection<ExplicitValueInstanceViewModel<JournalObjectType>>(explicitValue.JournalObjectTypeValueList.Select(x => new ExplicitValueInstanceViewModel<JournalObjectType>(x)));
			ColorValueList = new ObservableCollection<ExplicitValueInstanceViewModel<Color>>(explicitValue.ColorValueList.Select(x => new ExplicitValueInstanceViewModel<Color>(x)));

			Initialize();

			IsLabelDisabled = isLabelDisabled;
		}

		public void Initialize()
		{
			if (ExplicitValue.Value is ObjectReference)
			{
				var objRef = (ObjectReference)ExplicitValue.Value;
				if (objRef.UID != Guid.Empty)
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

					if (objRef.ObjectType == this.ObjectType)

						switch (objRef.ObjectType)
						{
							case ObjectType.Device:
								Device = (GKDevice)ExplicitValue.ResolvedValue;
								break;
							case ObjectType.Zone:
								Zone = (GKZone)ExplicitValue.ResolvedValue;
								break;
							case ObjectType.Direction:
								Direction = (GKDirection)ExplicitValue.ResolvedValue;
								break;
							case ObjectType.Delay:
								Delay = (GKDelay)ExplicitValue.ResolvedValue;
								break;
							case ObjectType.GuardZone:
								GuardZone = (GKGuardZone)ExplicitValue.ResolvedValue;
								break;
							case ObjectType.PumpStation:
								PumpStation = (GKPumpStation)ExplicitValue.ResolvedValue;
								break;
							case ObjectType.MPT:
								MPT = (GKMPT)ExplicitValue.ResolvedValue;
								break;
							case ObjectType.VideoDevice:
								Camera = (Camera)ExplicitValue.ResolvedValue;
								break;
							case ObjectType.GKDoor:
								GKDoor = (GKDoor)ExplicitValue.ResolvedValue;
								break;
							case ObjectType.Organisation:
								Organisation = (Organisation)ExplicitValue.ResolvedValue;
								break;
						}

					ExplicitType = ExplicitType.Object;
					ObjectType = objRef.ObjectType;
					OnPropertyChanged(() => PresentationName);
				}
			}

		}

		public bool IsList
		{
			get { return ExplicitValue.IsList; }
			set
			{
				ExplicitValue.IsList = value;
				OnPropertyChanged(() => IsList);
			}
		}

		public ExplicitType ExplicitType
		{
			get { return ExplicitValue.ExplicitType; }
			set
			{
				IsSimpleType = value != ExplicitType.Object;
				ExplicitValue.ExplicitType = value;
				OnPropertyChanged(() => ExplicitType);
			}
		}

		public EnumType EnumType
		{
			get { return ExplicitValue.EnumType; }
			set
			{
				ExplicitValue.EnumType = value;
				OnPropertyChanged(() => EnumType);
			}
		}

		public ObjectType ObjectType
		{
			get { return ExplicitValue.ObjectType; }
			set
			{
				ExplicitValue.ObjectType = value;
				if (ObjectReferenceValueList != null && ObjectReferenceValueList.Any(x => x.Value.ObjectType != value))
					ObjectReferenceValueList.Clear();
				OnPropertyChanged(() => ObjectType);
			}
		}

		bool _isSimpleType = true;
		public bool IsSimpleType
		{
			get { return _isSimpleType; }
			set
			{
				_isSimpleType = value;
				OnPropertyChanged(() => IsSimpleType);
			}
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

		object _value;
		public object Value
		{
			get { return ExplicitValue.ResolvedValue; }
			set
			{
				_value = ExplicitValue.ResolvedValue;
				OnPropertyChanged(() => Value);
				OnPropertyChanged(() => IsEmpty);
			}
		}

		GKDevice _device;
		public GKDevice Device
		{
			get { return _device; }
			set
			{
				ExplicitValue.Value = _device = value;
				OnPropertyChanged(() => Device);
				OnPropertyChanged(() => IsEmpty);
			}
		}

		GKDelay _delay;
		public GKDelay Delay
		{
			get { return _delay; }
			set
			{
				ExplicitValue.Value = _delay = value;
				OnPropertyChanged(() => Delay);
				OnPropertyChanged(() => IsEmpty);
			}
		}

		GKZone _zone;
		public GKZone Zone
		{
			get { return _zone; }
			set
			{
				ExplicitValue.Value = _zone = value;
				OnPropertyChanged(() => Zone);
				OnPropertyChanged(() => IsEmpty);
			}
		}

		GKGuardZone _guardZone;
		public GKGuardZone GuardZone
		{
			get { return _guardZone; }
			set
			{
				ExplicitValue.Value = _guardZone = value;
				OnPropertyChanged(() => GuardZone);
				OnPropertyChanged(() => IsEmpty);
			}
		}

		Camera _camera;
		public Camera Camera
		{
			get { return _camera; }
			set
			{
				ExplicitValue.Value = _camera = value;
				OnPropertyChanged(() => Camera);
				OnPropertyChanged(() => IsEmpty);
			}
		}

		GKDoor _gkDoor;
		public GKDoor GKDoor
		{
			get { return _gkDoor; }
			set
			{
				ExplicitValue.Value = _gkDoor = value;
				OnPropertyChanged(() => GKDoor);
				OnPropertyChanged(() => IsEmpty);
			}
		}

		GKDirection _direction;
		public GKDirection Direction
		{
			get { return _direction; }
			set
			{
				ExplicitValue.Value = _direction = value;
				OnPropertyChanged(() => Direction);
				OnPropertyChanged(() => IsEmpty);
			}
		}

		GKPumpStation _pumpStation;
		public GKPumpStation PumpStation
		{
			get { return _pumpStation; }
			set
			{
				ExplicitValue.Value = _pumpStation = value;
				OnPropertyChanged(() => PumpStation);
				OnPropertyChanged(() => IsEmpty);
			}
		}

		GKMPT _mpt;
		public GKMPT MPT
		{
			get { return _mpt; }
			set
			{
				ExplicitValue.Value = _mpt = value;
				OnPropertyChanged(() => MPT);
				OnPropertyChanged(() => IsEmpty);
			}
		}

		Organisation _organisation;
		public Organisation Organisation
		{
			get { return _organisation; }
			set
			{
				ExplicitValue.Value = _organisation = value;
				OnPropertyChanged(() => Organisation);
				OnPropertyChanged(() => IsEmpty);
			}
		}

		public ObservableCollection<ExplicitValueInstanceViewModel<int>> IntValueList { get; set; }
		public ObservableCollection<ExplicitValueInstanceViewModel<double>> FloatValueList { get; set; }
		public ObservableCollection<ExplicitValueInstanceViewModel<bool>> BoolValueList { get; set; }
		public ObservableCollection<ExplicitValueInstanceViewModel<DateTime>> DateTimeValueList { get; set; }
		public ObservableCollection<ObjectReferenceViewModel> ObjectReferenceValueList { get; set; }
		public ObservableCollection<ExplicitValueInstanceViewModel<string>> StringValueList { get; set; }
		public ObservableCollection<ExplicitValueInstanceViewModel<XStateClass>> StateTypeValueList { get; set; }
		public ObservableCollection<ExplicitValueInstanceViewModel<GKDriverType>> DriverTypeValueList { get; set; }
		public ObservableCollection<ExplicitValueInstanceViewModel<PermissionType>> PermissionTypeValueList { get; set; }
		public ObservableCollection<ExplicitValueInstanceViewModel<JournalEventNameType>> JournalEventNameTypeValueList { get; set; }
		public ObservableCollection<ExplicitValueInstanceViewModel<JournalEventDescriptionType>> JournalEventDescriptionTypeValueList { get; set; }
		public ObservableCollection<ExplicitValueInstanceViewModel<JournalObjectType>> JournalObjectTypeValueList { get; set; }
		public ObservableCollection<ExplicitValueInstanceViewModel<Color>> ColorValueList { get; set; }

		public IList GetListValue()
		{
			if (!IsList)
				return null;

			switch (ExplicitType)
			{
				case ExplicitType.Integer: return IntValueList.Select(x => x.Value).ToList();
				case ExplicitType.Float: return FloatValueList.Select(x => x.Value).ToList();
				case ExplicitType.Boolean: return BoolValueList.Select(x => x.Value).ToList();
				case ExplicitType.DateTime: return DateTimeValueList.Select(x => x.Value).ToList();
				case ExplicitType.String: return StringValueList.Select(x => x.Value).ToList();
				case ExplicitType.Object: return ObjectReferenceValueList.Select(x => x.Value).ToList();
				case ExplicitType.Enum:
					switch (EnumType)
					{
						case EnumType.StateType: return StateTypeValueList.Select(x => x.Value).ToList();
						case EnumType.DriverType: return DriverTypeValueList.Select(x => x.Value).ToList();
						case EnumType.PermissionType: return PermissionTypeValueList.Select(x => x.Value).ToList();
						case EnumType.JournalEventNameType: return JournalEventNameTypeValueList.Select(x => x.Value).ToList();
						case EnumType.JournalEventDescriptionType: return JournalEventDescriptionTypeValueList.Select(x => x.Value).ToList();
						case EnumType.JournalObjectType: return JournalObjectTypeValueList.Select(x => x.Value).ToList();
						case EnumType.ColorType: return ColorValueList.Select(x => x.Value).ToList();
					}
					break;
			}
			return null;
		}
		public bool IsEmpty
		{
			get
			{
				return ExplicitType != ExplicitType.Object
					|| (Device == null && ObjectType == ObjectType.Device)
					|| (Zone == null && ObjectType == ObjectType.Zone)
					|| (GuardZone == null && ObjectType == ObjectType.GuardZone)
					|| (Camera == null && ObjectType == ObjectType.VideoDevice)
					|| (Direction == null && ObjectType == ObjectType.Direction)
					|| (GKDoor == null && ObjectType == ObjectType.GKDoor)
					|| (Delay == null && ObjectType == ObjectType.Delay)
					|| (PumpStation == null && ObjectType == ObjectType.PumpStation)
					|| (MPT == null && ObjectType == ObjectType.MPT)
					|| (Organisation == null && ObjectType == ObjectType.Organisation);
			}
			set
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
			}
		}

		public new void OnPropertyChanged<T>(Expression<Func<T>> propertyExpression)
		{
			ServiceFactory.SaveService.AutomationChanged = true;
			base.OnPropertyChanged(propertyExpression);
			if (UpdateDescriptionHandler != null)
				UpdateDescriptionHandler();
		}

		public RelayCommand<ObjectReferenceViewModel> ChangeCommand { get; private set; }
		void OnChange(ObjectReferenceViewModel objectReferenceViewModel)
		{
			if (IsList)
				OnEditList();
			else
				ProcedureHelper.SelectObject(ObjectType, this);
			OnPropertyChanged(() => ExplicitValue);
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			switch (ExplicitType)
			{
				case ExplicitType.Integer: IntValueList.Add(new ExplicitValueInstanceViewModel<int>(default(int))); break;
				case ExplicitType.Float: FloatValueList.Add(new ExplicitValueInstanceViewModel<double>(default(double))); break;
				case ExplicitType.Boolean: BoolValueList.Add(new ExplicitValueInstanceViewModel<bool>(default(bool))); break;
				case ExplicitType.DateTime: DateTimeValueList.Add(new ExplicitValueInstanceViewModel<DateTime>(DateTime.Now)); break;
				case ExplicitType.String: StringValueList.Add(new ExplicitValueInstanceViewModel<string>(default(string))); break;
				case ExplicitType.Object: ObjectReferenceValueList.Add(new ObjectReferenceViewModel(new ObjectReference { ObjectType = this.ObjectType })); break;
				case ExplicitType.Enum:
					switch (EnumType)
					{
						case EnumType.StateType: StateTypeValueList.Add(new ExplicitValueInstanceViewModel<XStateClass>(default(XStateClass))); break;
						case EnumType.DriverType: DriverTypeValueList.Add(new ExplicitValueInstanceViewModel<GKDriverType>(default(GKDriverType))); break;
						case EnumType.PermissionType: PermissionTypeValueList.Add(new ExplicitValueInstanceViewModel<PermissionType>(default(PermissionType))); break;
						case EnumType.JournalEventNameType: JournalEventNameTypeValueList.Add(new ExplicitValueInstanceViewModel<JournalEventNameType>(default(JournalEventNameType))); break;
						case EnumType.JournalEventDescriptionType: JournalEventDescriptionTypeValueList.Add(new ExplicitValueInstanceViewModel<JournalEventDescriptionType>(default(JournalEventDescriptionType))); break;
						case EnumType.JournalObjectType: JournalObjectTypeValueList.Add(new ExplicitValueInstanceViewModel<JournalObjectType>(default(JournalObjectType))); break;
						case EnumType.ColorType: ColorValueList.Add(new ExplicitValueInstanceViewModel<Color>(default(Color))); break;
					}
					break;
			}
		}

		public RelayCommand<object> RemoveCommand { get; private set; }
		void OnRemove(object explicitValueInstanceViewModel)
		{
			if (explicitValueInstanceViewModel is ExplicitValueInstanceViewModel<int>)
				IntValueList.Remove((ExplicitValueInstanceViewModel<int>)explicitValueInstanceViewModel);
			if (explicitValueInstanceViewModel is ExplicitValueInstanceViewModel<double>)
				FloatValueList.Remove((ExplicitValueInstanceViewModel<double>)explicitValueInstanceViewModel);
			if (explicitValueInstanceViewModel is ExplicitValueInstanceViewModel<bool>)
				BoolValueList.Remove((ExplicitValueInstanceViewModel<bool>)explicitValueInstanceViewModel);
			if (explicitValueInstanceViewModel is ExplicitValueInstanceViewModel<DateTime>)
				DateTimeValueList.Remove((ExplicitValueInstanceViewModel<DateTime>)explicitValueInstanceViewModel);
			if (explicitValueInstanceViewModel is ExplicitValueInstanceViewModel<string>)
				StringValueList.Remove((ExplicitValueInstanceViewModel<string>)explicitValueInstanceViewModel);
			if (explicitValueInstanceViewModel is ObjectReferenceViewModel)
				ObjectReferenceValueList.Remove((ObjectReferenceViewModel)explicitValueInstanceViewModel);
			if (explicitValueInstanceViewModel is ExplicitValueInstanceViewModel<XStateClass>)
				StateTypeValueList.Remove((ExplicitValueInstanceViewModel<XStateClass>)explicitValueInstanceViewModel);
			if (explicitValueInstanceViewModel is ExplicitValueInstanceViewModel<GKDriverType>)
				DriverTypeValueList.Remove((ExplicitValueInstanceViewModel<GKDriverType>)explicitValueInstanceViewModel);
			if (explicitValueInstanceViewModel is ExplicitValueInstanceViewModel<PermissionType>)
				PermissionTypeValueList.Remove((ExplicitValueInstanceViewModel<PermissionType>)explicitValueInstanceViewModel);
			if (explicitValueInstanceViewModel is ExplicitValueInstanceViewModel<JournalEventNameType>)
				JournalEventNameTypeValueList.Remove((ExplicitValueInstanceViewModel<JournalEventNameType>)explicitValueInstanceViewModel);
			if (explicitValueInstanceViewModel is ExplicitValueInstanceViewModel<JournalEventDescriptionType>)
				JournalEventDescriptionTypeValueList.Remove((ExplicitValueInstanceViewModel<JournalEventDescriptionType>)explicitValueInstanceViewModel);
			if (explicitValueInstanceViewModel is ExplicitValueInstanceViewModel<JournalObjectType>)
				JournalObjectTypeValueList.Remove((ExplicitValueInstanceViewModel<JournalObjectType>)explicitValueInstanceViewModel);
			if (explicitValueInstanceViewModel is ExplicitValueInstanceViewModel<Color>)
				ColorValueList.Remove((ExplicitValueInstanceViewModel<Color>)explicitValueInstanceViewModel);
		}

		public RelayCommand EditStringCommand { get; private set; }
		void OnEditString()
		{
			var stringDetailsViewModel = new StringDetailsViewModel(ExplicitValue.StringValue);
			if (DialogService.ShowModalWindow(stringDetailsViewModel))
				StringValue = stringDetailsViewModel.StringValue;
		}

		public RelayCommand EditListCommand { get; private set; }
		void OnEditList()
		{
			var objectReferences = ObjectReferenceValueList.Select(x => x.Value).ToList();
			if (ExplicitType == ExplicitType.Object)
				if (!ProcedureHelper.SelectObjects(ObjectType, ref objectReferences))
					return;
			if (objectReferences != null)
			{
				ObjectReferenceValueList.Clear();
				objectReferences.ForEach(x => ObjectReferenceValueList.Add(new ObjectReferenceViewModel(x)));
			}

		}
	}
}