using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Windows.Media;
using StrazhAPI.Automation;
using StrazhAPI.AutomationCallback;
using StrazhAPI.Integration.OPC;
using StrazhAPI.Models.Automation;

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
		OperationResult<bool> SaveGlobalVariable(GlobalVariable variable);

		[OperationContract]
		OperationResult<bool> ResetGlobalVariables();

		[OperationContract]
		OperationResult<bool> RemoveGlobalVariable(GlobalVariable variable);

		[OperationContract]
		OperationResult<List<GlobalVariable>> GetInitialGlobalVariables();

		[OperationContract]
		OperationResult<bool> SaveGlobalVariables(List<IVariable> variables);

		[OperationContract]
		OperationResult<List<GlobalVariable>> GetCurrentGlobalVariables();

		[OperationContract]
		OperationResult<bool> SaveEditedGlobalVariables(IEnumerable<GlobalVariable> variables);

		[OperationContract]
		OperationResult<List<Script>> GetFiresecScripts();
	}
}