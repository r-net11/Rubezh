using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FiresecAPI.Automation;
using Infrastructure.Common.Windows.ViewModels;
using ValueType = FiresecAPI.Automation.ValueType;

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
			ObjectTypes = new ObservableCollection<ObjectType>
			{
				ObjectType.Card, ObjectType.Device, ObjectType.Direction, ObjectType.GuardZone, ObjectType.Person, ObjectType.Plan,
				ObjectType.SKDDevice, ObjectType.SKDZone, ObjectType.VideoDevice, ObjectType.Zone
			};
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
}