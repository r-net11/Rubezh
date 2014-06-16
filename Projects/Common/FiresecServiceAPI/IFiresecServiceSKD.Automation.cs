using System.Collections.Generic;
using System.ServiceModel;
using TimeIntervals = FiresecAPI.EmployeeTimeIntervals;
using System;

namespace FiresecAPI
{
	public partial interface IFiresecServiceSKD
	{
		[OperationContract]
		OperationResult<bool> RunProcedure(Guid procedureUID, List<string> args);
	}
}