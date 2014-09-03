using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FiresecAPI.Automation;
using Infrastructure.Common.Windows.ViewModels;
using ValueType = FiresecAPI.Automation.ValueType;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using FiresecClient;
using System.Linq;
using FiresecAPI.SKD;
using FiresecAPI.GK;
using FiresecAPI.Models;

namespace AutomationModule.ViewModels
{
	public class VariableViewModel : BaseViewModel
	{
		public Variable Variable { get; set; }

		public VariableViewModel(Variable variable)
		{
			Variable = variable;
			Update();
		}

		public VariableViewModel(string name, ValueType valueType)
		{
			Variable = new Variable(name);
			Variable.ValueType = valueType;
			Update();
		}

		public void Update()
		{
			Name = Variable.Name;
			DefaultBoolValue = Variable.DefaultBoolValue;
			DefaultDateTimeValue = Variable.DefaultDateTimeValue;
			DefaultIntValue = Variable.DefaultIntValue;
			ObjectType = Variable.ObjectType;
			DefaultStringValue = Variable.DefaultStringValue;
			ValueType = Variable.ValueType;
			IsList = Variable.IsList;
			IsGlobal = Variable.IsGlobal;
			ObjectTypes = ProcedureHelper.GetEnumObs<ObjectType>();

			OnPropertyChanged(() => Variable);
			OnPropertyChanged(() => Name);
			OnPropertyChanged(() => DefaultBoolValue);
			OnPropertyChanged(() => DefaultDateTimeValue);
			OnPropertyChanged(() => DefaultIntValue);
			OnPropertyChanged(() => ObjectType);
			OnPropertyChanged(() => DefaultStringValue);
			OnPropertyChanged(() => ValueType);
		}

		bool _isGlobal;
		public bool IsGlobal
		{
			get { return _isGlobal; }
			set
			{
				_isGlobal = value;
				OnPropertyChanged(() => IsGlobal);
			}
		}

		bool _isList;
		public bool IsList
		{
			get { return _isList; }
			set
			{
				_isList = value;
				OnPropertyChanged(() => IsList);
			}
		}

		string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged(() => Name);
			}
		}

		bool _defaultBoolValue;
		public bool DefaultBoolValue
		{
			get { return _defaultBoolValue; }
			set
			{
				_defaultBoolValue = value;
				OnPropertyChanged(() => DefaultBoolValue);
			}
		}

		DateTime _defaultDateTimeValue;
		public DateTime DefaultDateTimeValue
		{
			get { return _defaultDateTimeValue; }
			set
			{
				_defaultDateTimeValue = value;
				OnPropertyChanged(() => DefaultDateTimeValue);
			}
		}

		int _defaultIntValue;
		public int DefaultIntValue
		{
			get { return _defaultIntValue; }
			set
			{
				_defaultIntValue = value;
				OnPropertyChanged(() => DefaultIntValue);
			}
		}

		string _defaultStringValue;
		public string DefaultStringValue
		{
			get { return _defaultStringValue; }
			set
			{
				_defaultStringValue = value;
				OnPropertyChanged(() => DefaultStringValue);
			}
		}

		ValueType _valueType;
		public ValueType ValueType
		{
			get { return _valueType; }
			set
			{
				_valueType = value;
				OnPropertyChanged(() => ValueType);
			}
		}

		public ObservableCollection<ObjectType> ObjectTypes { get; private set; }
		ObjectType _objectType;
		public ObjectType ObjectType
		{
			get { return _objectType; }
			set
			{
				_objectType = value;
				OnPropertyChanged(() => ObjectType);
			}
		}
	}

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

		public VariableItemViewModel(VariableItem variableItem)
		{
			VariableItem = new VariableItem();
			Initialize(variableItem);
		}

		public void Initialize(VariableItem variableItem)
		{
			VariableItem = (VariableItem)variableItem.Clone();
			var objectUid = variableItem.ObjectUid;
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

		public bool SelectedBoolValue
		{
			get { return VariableItem.BoolValue; }
			set
			{
				VariableItem.BoolValue = value;
				OnPropertyChanged(() => SelectedBoolValue);
			}
		}

		public DateTime SelectedDateTimeValue
		{
			get { return VariableItem.DateTimeValue; }
			set
			{
				VariableItem.DateTimeValue = value;
				OnPropertyChanged(() => SelectedDateTimeValue);
			}
		}

		public int SelectedIntValue
		{
			get { return VariableItem.IntValue; }
			set
			{
				VariableItem.IntValue = value;
				OnPropertyChanged(() => SelectedIntValue);
			}
		}

		public string SelectedStringValue
		{
			get { return VariableItem.StringValue; }
			set
			{
				VariableItem.StringValue = value;
				OnPropertyChanged(() => SelectedStringValue);
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