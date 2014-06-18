using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FiresecAPI;
using FiresecAPI.SKD;
using SKDDriver;
using FiresecService.Processor;

namespace FiresecService.Service
{
	public partial class FiresecService : IFiresecService
	{
		public OperationResult<bool> RunProcedure(Guid procedureUID, List<string> args)
		{
			var procedure = AutomationProcessor.SystemConfiguration.AutomationConfiguration.Procedures.FirstOrDefault(x => x.Uid == procedureUID);
			if (procedure != null)
			{
				var result = AutomationProcessorRunner.Run(procedure, args);
				return new OperationResult<bool>() { Result = result };
			}
			else
			{
				return new OperationResult<bool>("Процедура не найдена");
			}
		}
	}
}