using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class ArgumentViewModel : BaseViewModel
	{
		public Argument Argument { get; private set; }
		Procedure Procedure { get; set; }
		public ArgumentViewModel(Argument argument, Procedure procedure)
		{
			Argument = argument;
			Procedure = procedure;
			ObjectTypes = new ObservableCollection<ObjectType>
			{
				ObjectType.Card, ObjectType.Device, ObjectType.Direction, ObjectType.GuardZone, ObjectType.Person, ObjectType.Plan,
				ObjectType.SKDDevice, ObjectType.SKDZone, ObjectType.VideoDevice, ObjectType.Zone
			};
		}

		public bool IsList
		{
			get { return Argument.IsList; }
		}

		public string Name
		{
			get
			{
				var variable = Procedure.Arguments.FirstOrDefault(x => x.Uid == Argument.ArgumentUid);
				if (variable != null)
					return variable.Name;
				return "";
			}
		}

		public bool BoolValue
		{
			get { return Argument.BoolValue; }
			set
			{
				Argument.BoolValue = value;
				OnPropertyChanged(() => BoolValue);
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}

		public DateTime DateTimeValue
		{
			get { return Argument.DateTimeValue; }
			set
			{
				var dateTimeValue = DateTimeValue.AddMilliseconds(-DateTimeValue.Millisecond);
				value = value.AddMilliseconds(-DateTimeValue.Millisecond);
				if (dateTimeValue != value)
					ServiceFactory.SaveService.AutomationChanged = true;
				Argument.DateTimeValue = value;
				OnPropertyChanged(() => DateTimeValue);

			}
		}

		public int IntValue
		{
			get { return Argument.IntValue; }
			set
			{
				Argument.IntValue = value;
				OnPropertyChanged(() => IntValue);
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}

		public ObservableCollection<ObjectType> ObjectTypes { get; private set; }
		public ObjectType ObjectType
		{
			get { return Argument.ObjectType; }
			set
			{
				Argument.ObjectType = value;
				OnPropertyChanged(() => ObjectType);
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}

		public string StringValue
		{
			get { return Argument.StringValue; }
			set
			{
				Argument.StringValue = value;
				OnPropertyChanged(() => StringValue);
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}

		public VariableType VariableType
		{
			get { return Argument.VariableType; }
			set
			{
				Argument.VariableType = value;
				OnPropertyChanged(() => VariableType);
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}
	}
}
