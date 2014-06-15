using System;
using System.Collections.Generic;
using Common;
using FiresecAPI;
using FiresecAPI.SKD;

namespace FiresecClient
{
	public partial class SafeFiresecService
	{
		public OperationResult<bool> RunProcedure(Guid procedureUID, List<string> args)
		{
			return SafeContext.Execute<OperationResult<bool>>(() => FiresecService.RunProcedure(procedureUID, args));
		}
	}
}