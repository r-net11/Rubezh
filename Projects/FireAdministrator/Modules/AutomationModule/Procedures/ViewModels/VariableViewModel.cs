using System;
using System.Collections.ObjectModel;
using FiresecAPI.Automation;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class VariableViewModel : BaseViewModel
	{
		public Variable Variable { get; set; }

		public VariableViewModel(Variable variable)
		{
			Variable = variable;
			Initialize();
		}

		public VariableViewModel(string name)
		{
			Variable = new Variable(name);
			Initialize();
		}

		void Initialize()
		{
			ObjectValues = new ObservableCollection<ObjectType>
			{
				ObjectType.Card, ObjectType.Device, ObjectType.Direction, ObjectType.GuardZone, ObjectType.Person, ObjectType.Plan,
				ObjectType.SKDDevice, ObjectType.SKDZone, ObjectType.VideoDevice, ObjectType.Zone
			};
		}

		public string Name
		{
			get { return Variable.Name; }
			set
			{
				Variable.Name = value;
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => Name);
			}
		}

		public bool BoolValue
		{
			get { return Variable.BoolValue; }
			set
			{
				Variable.BoolValue = value;
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged("Value");
			}
		}

		public DateTime DateTimeValue
		{
			get { return Variable.DateTimeValue; }
			set
			{
				Variable.DateTimeValue = value;
				OnPropertyChanged(() => DateTimeValue);
			}
		}

		public int IntValue
		{
			get { return Variable.IntValue; }
			set
			{
				Variable.IntValue = value;
				OnPropertyChanged(() => IntValue);
			}
		}

		public ObservableCollection<ObjectType> ObjectValues { get; private set; }
		public ObjectType ObjectValue
		{
			get { return Variable.ObjectValue; }
			set
			{
				Variable.ObjectValue = value;
				OnPropertyChanged(() => ObjectValue);
			}
		}

		public string StringValue
		{
			get { return Variable.StringValue; }
			set
			{
				Variable.StringValue = value;
				OnPropertyChanged(() => StringValue);
			}
		}

		public VariableType VariableType
		{
			get { return Variable.VariableType; }
			set
			{
				Variable.VariableType = value;
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged("VariableType");
			}
		}

		public void Update()
		{
			OnPropertyChanged(() => Variable);
			OnPropertyChanged(() => Name);
			OnPropertyChanged(() => BoolValue);
			OnPropertyChanged(() => DateTimeValue);
			OnPropertyChanged(() => IntValue);
			OnPropertyChanged(() => ObjectValue);
			OnPropertyChanged(() => StringValue);
			OnPropertyChanged(() => VariableType);
		}
	}
}
