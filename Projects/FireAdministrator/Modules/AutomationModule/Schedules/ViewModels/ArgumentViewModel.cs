using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using ValueType = FiresecAPI.Automation.ValueType;
using Infrastructure.Common;

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
			SelectedVariableItem = new VariableItemViewModel(new VariableItem { ObjectUid = Argument.UidValue });
			ObjectTypes = ProcedureHelper.GetEnumObs<ObjectType>();
			ChangeItemCommand = new RelayCommand(OnChangeItem);
		}

		public RelayCommand ChangeItemCommand { get; private set; }
		public void OnChangeItem()
		{
			SelectedVariableItem = ProcedureHelper.SelectObject(Argument.ObjectType, SelectedVariableItem);
		}

		VariableItemViewModel _selectedVariableItem;
		public VariableItemViewModel SelectedVariableItem
		{
			get { return _selectedVariableItem; }
			set
			{
				_selectedVariableItem = value;
				if (value != null)
					Argument.UidValue = _selectedVariableItem.VariableItem.ObjectUid;
				OnPropertyChanged(() => SelectedVariableItem);
			}
		}

		public bool IsList
		{
			get { return Argument.IsList; }
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

		public ValueType ValueType
		{
			get { return Argument.ValueType; }
			set
			{
				Argument.ValueType = value;
				OnPropertyChanged(() => ValueType);
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
