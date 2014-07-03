using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecAPI.Automation;
using FiresecService.Processor;

namespace FiresecService.Service
{
	public partial class FiresecService : IFiresecService
	{
		public OperationResult<bool> RunProcedure(Guid procedureUID, List<Argument> args)
		{
			var procedure = ConfigurationCashHelper.SystemConfiguration.AutomationConfiguration.Procedures.FirstOrDefault(x => x.Uid == procedureUID);
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