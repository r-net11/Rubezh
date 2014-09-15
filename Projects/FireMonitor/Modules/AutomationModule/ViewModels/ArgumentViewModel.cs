using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;

namespace AutomationModule.ViewModels
{
	public class ArgumentViewModel : BaseViewModel
	{
		public Variable Variable { get; private set; }
		public VariableItemViewModel CurrentVariableItem { get; private set; }
		public Argument Argument { get; private set; }
		public ArgumentViewModel(Argument argument, Procedure procedure)
		{
			Argument = argument;
			Variable = procedure.Arguments.FirstOrDefault(x => x.Uid == argument.VariableUid);
			CurrentVariableItem = new VariableItemViewModel(argument.VariableItem);
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