using Infrastructure.Automation;
using Infrastructure.Common.Windows.Windows.ViewModels;
using RubezhAPI.Automation;
using RubezhClient;
using System.Collections.Generic;
using System.Linq;

namespace AutomationModule.ViewModels
{
	public class ScheduleProcedureViewModel : BaseViewModel
	{
		public ScheduleProcedure ScheduleProcedure { get; private set; }
		public List<ArgumentViewModel> Arguments { get; private set; }
		public Procedure Procedure { get; private set; }
		public Procedure CallingProcedure { get; private set; }

		public ScheduleProcedureViewModel(ScheduleProcedure scheduleProcedure, Procedure callingProcedure = null)
		{
			CallingProcedure = callingProcedure;
			ScheduleProcedure = scheduleProcedure;
			Procedure = ClientManager.SystemConfiguration.AutomationConfiguration.Procedures.FirstOrDefault(x => x.Uid == scheduleProcedure.ProcedureUid);
			if (Procedure != null)
				UpdateArguments();
		}

		public void UpdateArguments()
		{
			Arguments = new List<ArgumentViewModel>();
			int i = 0;
			if (ScheduleProcedure.Arguments == null)
				ScheduleProcedure.Arguments = new List<Argument>();
			foreach (var variable in Procedure.Arguments)
			{
				var argument = new Argument();
				if (ScheduleProcedure.Arguments.Count <= i)
				{
					argument = InitializeArgumemt(variable);
				}
				else
				{
					if (!CheckSignature(ScheduleProcedure.Arguments[i], variable))
					{
						argument = InitializeArgumemt(variable);
					}
					else
						argument = ScheduleProcedure.Arguments[i];
				}
				var argumentViewModel = new ArgumentViewModel(argument, null, null, true, CallingProcedure != null);
				argumentViewModel.Name = variable.Name;
				argumentViewModel.IsList = variable.IsList;
				argumentViewModel.Update(GetVariables(argumentViewModel), argumentViewModel.ExplicitType, argumentViewModel.EnumType, argumentViewModel.ObjectType, argumentViewModel.IsList);
				Arguments.Add(argumentViewModel);
				i++;
			}
			ScheduleProcedure.Arguments = new List<Argument>();
			foreach (var argument in Arguments)
				ScheduleProcedure.Arguments.Add(argument.Argument);
			OnPropertyChanged(() => Arguments);
		}

		bool CheckSignature(Argument argument, Variable variable)
		{
			if (argument.ExplicitType != variable.ExplicitType)
				return false;
			if (argument.ExplicitType != ExplicitType.Object && argument.ExplicitType != ExplicitType.Enum)
				return true;
			if (argument.ExplicitType != ExplicitType.Object)
				return (argument.ObjectType == variable.ObjectType);
			if (argument.ExplicitType != ExplicitType.Enum)
				return (argument.EnumType == variable.EnumType);
			return false;
		}

		Argument InitializeArgumemt(Variable variable)
		{
			var argument = new Argument();
			argument.VariableScope = VariableScope.GlobalVariable;
			argument.ExplicitType = variable.ExplicitType;
			argument.EnumType = variable.EnumType;
			argument.ObjectType = variable.ObjectType;
			argument.Value = variable.Value;
			return argument;
		}

		List<Variable> GetVariables(ArgumentViewModel argument)
		{
			List<Variable> allVariables;
			if (CallingProcedure != null)
				allVariables = AutomationHelper.GetAllVariables(CallingProcedure);
			else
				allVariables = new List<Variable>(ClientManager.SystemConfiguration.AutomationConfiguration.GlobalVariables);
			allVariables = allVariables.FindAll(x => x.ExplicitType == argument.ExplicitType && x.IsList == argument.IsList);
			if (argument.ExplicitType == ExplicitType.Object)
				allVariables = allVariables.FindAll(x => x.ObjectType == argument.ObjectType);
			if (argument.ExplicitType == ExplicitType.Enum)
				allVariables = allVariables.FindAll(x => x.EnumType == argument.EnumType);
			return allVariables;
		}

		public string Name
		{
			get
			{
				if (Procedure != null)
					return Procedure.Name;
				return "процедура не найдена";
			}
		}
	}
}
