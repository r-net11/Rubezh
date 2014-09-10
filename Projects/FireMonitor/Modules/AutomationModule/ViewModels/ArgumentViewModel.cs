using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
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
				ObjectType.Device, ObjectType.Direction, ObjectType.GuardZone,ObjectType.SKDDevice,
				ObjectType.SKDZone, ObjectType.VideoDevice, ObjectType.Zone
			};
		}

		public string Name
		{
			get
			{
				var variable = Procedure.Arguments.FirstOrDefault(x => x.Uid == Argument.VariableUid);
				if (variable != null)
				    return variable.Name;
				return "";
			}
		}

		public bool IsList
		{
			get { return Argument.IsList; }
			set
			{
				Argument.IsList = value;
				OnPropertyChanged(() => IsList);
			}
		}

		public bool BoolValue
		{
			get { return Argument.VariableItem.BoolValue; }
			set
			{
				Argument.VariableItem.BoolValue = value;
				OnPropertyChanged(() => BoolValue);
			}
		}

		public DateTime DateTimeValue
		{
			get { return Argument.VariableItem.DateTimeValue; }
			set
			{
				Argument.VariableItem.DateTimeValue = value;
				OnPropertyChanged(() => DateTimeValue);
			}
		}

		public int IntValue
		{
			get { return Argument.VariableItem.IntValue; }
			set
			{
				Argument.VariableItem.IntValue = value;
				OnPropertyChanged(() => IntValue);
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
			}
		}

		public string StringValue
		{
			get { return Argument.VariableItem.StringValue; }
			set
			{
				Argument.VariableItem.StringValue = value;
				OnPropertyChanged(() => StringValue);
			}
		}

		public ExplicitType ExplicitType
		{
			get { return Argument.ExplicitType; }
			set
			{
				Argument.ExplicitType = value;
				OnPropertyChanged(() => ExplicitType);
			}
		}
	}
}