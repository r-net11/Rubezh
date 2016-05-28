using System;
using System.Collections.Generic;
using System.ServiceModel;
using StrazhAPI.Automation;
using StrazhAPI.AutomationCallback;
using StrazhAPI.Integration.OPC;

namespace StrazhAPI
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

		[OperationContract]
		OperationResult<List<Script>> GetFiresecScripts();
	}
}