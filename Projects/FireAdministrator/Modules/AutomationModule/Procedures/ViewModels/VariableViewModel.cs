using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FiresecAPI.Automation;
using FiresecAPI.Models;
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
			BoolValue = Variable.BoolValue;
			DateTimeValue = Variable.DateTimeValue;
			IntValue = Variable.IntValue;
			ObjectType = Variable.ObjectType;
			StringValue = Variable.StringValue;
			VariableType = Variable.VariableType;
			IsList = Variable.IsList;

			ObjectTypes = new ObservableCollection<ObjectType>
			{
				ObjectType.Card, ObjectType.Device, ObjectType.Direction, ObjectType.GuardZone, ObjectType.Person, ObjectType.Plan,
				ObjectType.SKDDevice, ObjectType.SKDZone, ObjectType.VideoDevice, ObjectType.Zone
			};
			OnPropertyChanged(() => Variable);
			OnPropertyChanged(() => Name);
			OnPropertyChanged(() => BoolValue);
			OnPropertyChanged(() => DateTimeValue);
			OnPropertyChanged(() => IntValue);
			OnPropertyChanged(() => ObjectType);
			OnPropertyChanged(() => StringValue);
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

		bool _boolValue;
		public bool BoolValue
		{
			get { return _boolValue; }
			set
			{
				_boolValue = value;
				OnPropertyChanged(() => BoolValue);
			}
		}

		DateTime _dateTimeValue;
		public DateTime DateTimeValue
		{
			get { return _dateTimeValue; }
			set
			{
				_dateTimeValue = value;
				OnPropertyChanged(() => DateTimeValue);
			}
		}

		int _intValue;
		public int IntValue
		{
			get { return _intValue; }
			set
			{
				_intValue = value;
				OnPropertyChanged(() => IntValue);
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

		string _stringValue;
		public string StringValue
		{
			get { return _stringValue; }
			set
			{
				_stringValue = value;
				OnPropertyChanged(() => StringValue);
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

		public List<Device> Devices { get; private set; }
	}
}