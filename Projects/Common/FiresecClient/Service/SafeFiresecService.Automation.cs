using System;
using System.Collections.Generic;
using Common;
using StrazhAPI;
using StrazhAPI.Automation;
using StrazhAPI.AutomationCallback;
using StrazhAPI.Integration.OPC;

namespace FiresecClient
{
	public partial class SafeFiresecService
	{
		public OperationResult<bool> RunProcedure(Guid procedureUID, List<Argument> args)
		{
			return SafeContext.Execute(() => FiresecService.RunProcedure(FiresecServiceFactory.UID, procedureUID, args));
		}
		public void ProcedureCallbackResponse(Guid procedureThreadUID, object value)
		{
			SafeContext.Execute(() => FiresecService.ProcedureCallbackResponse(procedureThreadUID, value));
		}
		public ProcedureProperties GetProperties(Guid layoutUID)
		{
			return SafeContext.Execute(() => FiresecService.GetProperties(layoutUID));
		}

		public OperationResult<List<Script>> GetFiresecScripts()
		{
			return SafeContext.Execute(() => FiresecService.GetFiresecScripts());
		}
	}
}