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
		public ScheduleProcedureViewModel(ScheduleProcedure scheduleProcedure)
		{
			ScheduleProcedure = scheduleProcedure;
			Procedure = FiresecManager.SystemConfiguration.AutomationConfiguration.Procedures.FirstOrDefault(x => x.Uid == scheduleProcedure.ProcedureUid);
			ScheduleProcedure.ProcedureUid = scheduleProcedure.ProcedureUid;
			UpdateArguments(Procedure);

		}

		public void UpdateArguments(Procedure procedure)
		{
			Arguments = new List<ArgumentViewModel>();
			var tempArguments = new List<Argument>();
			foreach (var variable in procedure.Arguments)
			{
				var argument = new Argument(variable);
				if (ScheduleProcedure.Arguments.Any(x => x.VariableUid == variable.Uid))
					argument = ScheduleProcedure.Arguments.FirstOrDefault(x => x.VariableUid == variable.Uid);
				var argumentViewModel = new ArgumentViewModel(argument, procedure);
				tempArguments.Add(argument);
				Arguments.Add(argumentViewModel);
			}
			ScheduleProcedure.Arguments = new List<Argument>(tempArguments);
			OnPropertyChanged(() => Arguments);
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
