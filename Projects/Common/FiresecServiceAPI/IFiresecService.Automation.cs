using System;
using System.Collections.Generic;
using System.ServiceModel;
using FiresecAPI.Automation;

namespace FiresecAPI
{
	public partial interface IFiresecServiceSKD
	{
		[OperationContract]
		OperationResult<bool> RunProcedure(Guid clientUID, Guid procedureUID, List<Argument> args);
		//[OperationContract(IsOneWay=true)]
		//void ProcedureCallback(Guid procedureThreadUID, object value);
	}
}