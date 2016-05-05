using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Automation;
using FiresecAPI.Models.Automation;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;

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
			Procedure = FiresecManager.SystemConfiguration.AutomationConfiguration.Procedures.FirstOrDefault(x => x.Uid == scheduleProcedure.ProcedureUid);
			if (Procedure != null)
				UpdateArguments();
		}

		public void UpdateArguments()
		{
			Arguments = new List<ArgumentViewModel>();
			var i = 0;

			if (ScheduleProcedure.Arguments == null)
				ScheduleProcedure.Arguments = new List<Argument>();

			foreach (var variable in Procedure.Arguments)
			{
				Argument argument;

				if (ScheduleProcedure.Arguments.Count <= i)
					argument = InitializeArgumemt(variable);
				else
					argument = !CheckSignature(ScheduleProcedure.Arguments[i], variable)
								? InitializeArgumemt(variable)
								: ScheduleProcedure.Arguments[i];

				var argumentViewModel = new ArgumentViewModel(argument, null, null, true, CallingProcedure != null) { Name = variable.Name };
				argumentViewModel.Update(GetVariables(argumentViewModel), argumentViewModel.ExplicitType, argumentViewModel.EnumType, argumentViewModel.ObjectType);
				Arguments.Add(argumentViewModel);
				i++;
			}

			ScheduleProcedure.Arguments = new List<Argument>();

			foreach (var argument in Arguments)
				ScheduleProcedure.Arguments.Add(argument.Argument);

			OnPropertyChanged(() => Arguments);
		}

		private static bool CheckSignature(Argument argument, IVariable variable)
		{
			if (argument.ExplicitType != variable.VariableValue.ExplicitType)
				return false;
			if (argument.ExplicitType != ExplicitType.Object && argument.ExplicitType != ExplicitType.Enum)
				return true;
			if (argument.ExplicitType != ExplicitType.Object)
				return (argument.ObjectType == variable.VariableValue.ObjectType);
			if (argument.ExplicitType != ExplicitType.Enum)
				return (argument.EnumType == variable.VariableValue.EnumType);
			return false;
		}

		private static Argument InitializeArgumemt(IVariable variable)
		{
			var argument = new Argument
			{
				VariableScope = VariableScope.GlobalVariable,
				ExplicitType = variable.VariableValue.ExplicitType,
				EnumType = variable.VariableValue.EnumType,
				ObjectType = variable.VariableValue.ObjectType
			};

			PropertyCopy.Copy(variable.VariableValue.ExplicitValue, argument.ExplicitValue);
			argument.ExplicitValues = new List<ExplicitValue>();

			foreach (var explicitValue in variable.VariableValue.ExplicitValues)
			{
				var newExplicitValue = new ExplicitValue();
				PropertyCopy.Copy(explicitValue, newExplicitValue);
				argument.ExplicitValues.Add(newExplicitValue);
			}

			return argument;
		}

		List<IVariable> GetVariables(ArgumentViewModel argument)
		{
			var allVariables = CallingProcedure != null
				? ProcedureHelper.GetAllVariables(CallingProcedure)
				: FiresecManager.FiresecService.GetInitialGlobalVariables().Result.ToList<IVariable>();
			//	: new List<IVariable>();//(FiresecManager.SystemConfiguration.AutomationConfiguration.GlobalVariables); TODO: Get global variables from the server

			allVariables = allVariables.FindAll(x => x.VariableValue.ExplicitType == argument.ExplicitType);

			if (argument.ExplicitType == ExplicitType.Object)
				allVariables = allVariables.FindAll(x => x.VariableValue.ObjectType == argument.ObjectType);

			if (argument.ExplicitType == ExplicitType.Enum)
				allVariables = allVariables.FindAll(x => x.VariableValue.EnumType == argument.EnumType);

			return allVariables;
		}

		public string Name
		{
			get { return Procedure != null ? Procedure.Name : Resources.Language.Schedules.ViewModels.ScheduleProcedureViewModel.ProcedureNotFound; }
		}
	}
}
