using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;

namespace AutomationModule.ViewModels
{
	public class ArgumentViewModel : BaseViewModel
	{
		public Argument Argument { get; private set; }
		Procedure Procedure { get; set; }
		public VariableItemViewModel CurrentVariableItem { get; private set; }

		public ArgumentViewModel(Argument argument, Procedure procedure)
		{
			Argument = argument;
			Procedure = procedure;
			CurrentVariableItem = new VariableItemViewModel(argument.VariableItem);
			ObjectTypes = ProcedureHelper.GetEnumObs<ObjectType>();
			ChangeItemCommand = new RelayCommand(OnChangeItem);
		}

		public RelayCommand ChangeItemCommand { get; private set; }
		public void OnChangeItem()
		{
			CurrentVariableItem = ProcedureHelper.SelectObject(Argument.ObjectType, CurrentVariableItem);
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

		public ExplicitType ExplicitType
		{
			get { return Argument.ExplicitType; }
			set
			{
				Argument.ExplicitType = value;
				OnPropertyChanged(() => ExplicitType);
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}
	}
}
