using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Automation;
using FiresecClient;
using Infrastructure;
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

			Arguments = new List<ArgumentViewModel>();
			foreach (var argument in scheduleProcedure.Arguments)
			{
				var argumentViewModel = new ArgumentViewModel(argument);
				Arguments.Add(argumentViewModel);
			}
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
