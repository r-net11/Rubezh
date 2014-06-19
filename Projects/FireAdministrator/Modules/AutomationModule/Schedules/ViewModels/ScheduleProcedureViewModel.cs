using System;
using System.Linq;
using FiresecAPI.Automation;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class ScheduleProcedureViewModel : BaseViewModel
	{
		public ScheduleProcedure ScheduleProcedure { get; private set; }

		public ScheduleProcedureViewModel(Guid procedureUid)
		{
			ScheduleProcedure = new ScheduleProcedure(procedureUid);
		}

		Procedure Procedure
		{
			get
			{
				return FiresecManager.SystemConfiguration.AutomationConfiguration.Procedures.FirstOrDefault(x => x.Uid == ScheduleProcedure.ProcedureUid);
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
