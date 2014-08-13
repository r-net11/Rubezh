using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FiresecAPI.Automation;
using Infrastructure.Common.Windows.ViewModels;

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

		public VariableViewModel(string name, VariableType variableType)
		{
			Variable = new Variable(name);
			Variable.VariableType = variableType;
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
			VariableType = Variable.VariableType;
			IsList = Variable.IsList;

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
			OnPropertyChanged(() => VariableType);
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

		VariableType _variableType;
		public VariableType VariableType
		{
			get { return _variableType; }
			set
			{
				_variableType = value;
				OnPropertyChanged(() => VariableType);
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