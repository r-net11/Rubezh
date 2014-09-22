using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Automation;
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
			var tempArguments = new List<Variable>();
			foreach (var variable in Procedure.Arguments)
			{
				var argument = new Variable();
				argument.ArgumentUid = variable.Uid;
				argument.ExplicitType = variable.ExplicitType;
				argument.EnumType = variable.EnumType;
				argument.ObjectType = variable.ObjectType;
				PropertyCopy.Copy<ExplicitValue, ExplicitValue>(variable.DefaultExplicitValue, argument.ExplicitValue);
				argument.ExplicitValues = new List<ExplicitValue>();
				foreach (var defaultExplicitValues in variable.DefaultExplicitValues)
				{
					var explicitValue = new ExplicitValue();
					PropertyCopy.Copy<ExplicitValue, ExplicitValue>(defaultExplicitValues, explicitValue);
					argument.ExplicitValues.Add(explicitValue);
				}
				var scheduleProcedure = ScheduleProcedure.Arguments.FirstOrDefault(x => x.ArgumentUid == variable.Uid && x.IsList == variable.IsList);
				if (scheduleProcedure != null)
					argument = scheduleProcedure;
				else if (CallingProcedure == null)
					argument.VariableScope = VariableScope.GlobalVariable;
				argument.Name = variable.Name;
				argument.IsList = variable.IsList;
				tempArguments.Add(argument);
				var argumentViewModel = new ArgumentViewModel(argument, null, true, false);
				var allVariables = GetVariables(argument);
				if (CallingProcedure != null)
					argumentViewModel = new ArgumentViewModel(argument, null);
				argumentViewModel.Update(allVariables);
				Arguments.Add(argumentViewModel);				
			}
			ScheduleProcedure.Arguments = new List<Variable>(tempArguments);
			OnPropertyChanged(() => Arguments);
		}

		List<Variable> GetVariables(Variable argument)
		{
			var allVariables = new List<Variable>();
			if (CallingProcedure != null)
				allVariables = ProcedureHelper.GetAllVariables(CallingProcedure);
			else
				allVariables = new List<Variable>(FiresecManager.SystemConfiguration.AutomationConfiguration.GlobalVariables);
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
