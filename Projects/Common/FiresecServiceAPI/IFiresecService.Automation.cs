using System;
using System.Collections.Generic;
using System.ServiceModel;
using FiresecAPI.Automation;

namespace FiresecAPI
{
	public partial interface IFiresecServiceSKD
	{
		[OperationContract]
		OperationResult<bool> RunProcedure(Guid procedureUID, List<Variable> args);
	}
}