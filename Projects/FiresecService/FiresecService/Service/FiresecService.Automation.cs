using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecAPI.Automation;

namespace FiresecService.Service
{
	public partial class FiresecService : IFiresecService
	{
		public OperationResult<bool> RunProcedure(Guid clientUID, Guid procedureUID, List<Argument> args)
		{
			var procedure = ConfigurationCashHelper.SystemConfiguration.AutomationConfiguration.Procedures.FirstOrDefault(x => x.Uid == procedureUID);
			if (procedure != null)
			{
				var user = ConfigurationCashHelper.SecurityConfiguration.Users.FirstOrDefault(x => x.Login == CurrentClientCredentials.UserName);
				var result = ProcedureRunner.Run(procedure, args, null, ConfigurationCashHelper.SystemConfiguration.AutomationConfiguration.GlobalVariables, user, null, clientUID);
				return new OperationResult<bool> { Result = true };
			}
			return new OperationResult<bool>("Процедура не найдена");
		}

		public void ProcedureCallbackResponse(Guid procedureThreadUID, object value)
		{
			var procedure = ProcedureRunner.ProceduresThreads.FirstOrDefault(item => item.UID == procedureThreadUID);
			if (procedure != null)
				procedure.SetCallbackResponse(value);
		}
	}
}