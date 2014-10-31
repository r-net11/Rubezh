using System;
using System.Collections.Generic;
using Common;
using FiresecAPI;
using FiresecAPI.Automation;

namespace FiresecClient
{
	public partial class SafeFiresecService
	{
		public OperationResult<bool> RunProcedure(Guid procedureUID, List<Argument> args)
		{
			return SafeContext.Execute<OperationResult<bool>>(() => FiresecService.RunProcedure(FiresecServiceFactory.UID, procedureUID, args));
		}
	}
}