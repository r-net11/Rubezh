using FiresecAPI.Automation;
using FiresecAPI.AutomationCallback;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Windows.Media;

namespace FiresecAPI
{
	public partial interface IFiresecServiceSKD
	{
		[OperationContract]
		OperationResult<bool> RunProcedure(Guid clientUID, Guid procedureUID, List<Argument> args);

		[OperationContract(IsOneWay = true)]
		[ServiceKnownType(typeof(Color))]
		void ProcedureCallbackResponse(Guid callbackUID, object value);

		[OperationContract]
		ProcedureProperties GetProperties(Guid layoutUID);
	}
}