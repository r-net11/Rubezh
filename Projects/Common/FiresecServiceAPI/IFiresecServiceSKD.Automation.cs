using System.Collections.Generic;
using System.ServiceModel;
using FiresecAPI.Automation;
using TimeIntervals = FiresecAPI.EmployeeTimeIntervals;
using System;

namespace FiresecAPI
{
	public partial interface IFiresecServiceSKD
	{
		[OperationContract]
		OperationResult<bool> RunProcedure(Guid procedureUID, List<Argument> args);
	}
}