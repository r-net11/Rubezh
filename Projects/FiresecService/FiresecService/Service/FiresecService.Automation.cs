using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecAPI.Automation;

namespace FiresecService.Service
{
	public partial class FiresecService : IFiresecService
	{
		public OperationResult<bool> RunProcedure(Guid procedureUID, List<Argument> args)
		{
			ConfigurationCashHelper.SystemConfiguration.AutomationConfiguration.UpdateConfiguration();
			var procedure = ConfigurationCashHelper.SystemConfiguration.AutomationConfiguration.Procedures.FirstOrDefault(x => x.Uid == procedureUID);
			if (procedure != null)
			{
				var result = ProcedureRunner.Run(procedure, args, null, ConfigurationCashHelper.SystemConfiguration.AutomationConfiguration.GlobalVariables);
				return new OperationResult<bool> { Result = true };
			}
			return new OperationResult<bool>("Процедура не найдена");
		}
	}
}