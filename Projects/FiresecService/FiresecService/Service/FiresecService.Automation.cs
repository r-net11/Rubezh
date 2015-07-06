﻿using FiresecAPI;
using FiresecAPI.Automation;
using FiresecAPI.AutomationCallback;
using FiresecService.Automation;
using System;
using System.Collections.Generic;
using System.Linq;

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
				var result = ProcedureRunner.Run(procedure, args, null, user, null, clientUID);
				return new OperationResult<bool>(true);
			}
			return OperationResult<bool>.FromError("Процедура не найдена");
		}

		public void ProcedureCallbackResponse(Guid callbackUID, object value)
		{
			ProcedureThread.CallbackResponse(callbackUID, value);
		}

		public ProcedureProperties GetProperties(Guid layoutUID)
		{
			return ProcedurePropertyCache.GetProcedureProperties(layoutUID);
		}
	}
}