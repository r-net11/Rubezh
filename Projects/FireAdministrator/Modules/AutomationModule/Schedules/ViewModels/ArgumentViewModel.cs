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
		public Variable Variable { get; private set; }
		public VariableItemViewModel CurrentVariableItem { get; private set; }

		public ArgumentViewModel(Argument argument, Procedure procedure)
		{
			Variable = procedure.Arguments.FirstOrDefault(x => x.Uid == argument.VariableUid);
			CurrentVariableItem = new VariableItemViewModel(argument.VariableItem);
			ChangeItemCommand = new RelayCommand(OnChangeItem);
		}

		public RelayCommand ChangeItemCommand { get; private set; }
		public void OnChangeItem()
		{
			CurrentVariableItem = ProcedureHelper.SelectObject(ObjectType, CurrentVariableItem);
		}


		public bool IsList
		{
			get { return Variable.IsList; }
		}

		public string Name
		{
			get { return Variable.Name; }
		}

		public ObjectType ObjectType
		{
			get { return Variable.ObjectType; }
		}

		public ExplicitType ExplicitType
		{
			get { return Variable.ExplicitType; }
		}
	}
}
