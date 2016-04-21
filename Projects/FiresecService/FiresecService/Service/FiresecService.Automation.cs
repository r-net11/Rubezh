using System.Windows.Forms.VisualStyles;
using FiresecAPI;
using FiresecAPI.Automation;
using FiresecAPI.AutomationCallback;
using FiresecAPI.Models.Automation;
using FiresecService.Automation;
using System;
using System.Collections.Generic;
using System.Linq;
using SKDDriver;

namespace FiresecService.Service
{
	public partial class FiresecService : IFiresecService
	{
		public OperationResult<bool> RunProcedure(Guid clientUID, Guid procedureUID, List<Argument> args)
		{
			var procedure = ConfigurationCashHelper.SystemConfiguration.AutomationConfiguration.Procedures.FirstOrDefault(x => x.Uid == procedureUID);
			if (procedure != null)
			{
				var user = ConfigurationCashHelper.SecurityConfiguration.Users.FirstOrDefault(x => x.Login == CurrentClientCredentials.UserName);
				var result = ProcedureRunner.Run(procedure, args, null, user, null, clientUID);
				return new OperationResult<bool>(true);
			}
			return OperationResult<bool>.FromError("Процедура не найдена");
		}

		public void ProcedureCallbackResponse(Guid callbackUID, object value)
		{
			ProcedureThread.CallbackResponse(callbackUID, value);
		}

		public ProcedureProperties GetProperties(Guid layoutUID)
		{
			return ProcedurePropertyCache.GetProcedureProperties(layoutUID);
		}

		public OperationResult<bool> SaveGlobalVariable(GlobalVariable variable)
		{
			using (var db = new SKDDatabaseService())
			{
				return db.GlobalVariablesTranslator.Save(variable);
			}
		}

		public OperationResult<bool> ResetGlobalVariables()
		{
			using (var db = new SKDDatabaseService())
			{
				return db.GlobalVariablesTranslator.ResetGlobalVariables();
			}
		}

		public OperationResult<bool> SaveGlobalVariables(List<IVariable> variables)
		{
			using (var db = new SKDDatabaseService())
			{
				return db.GlobalVariablesTranslator.Save(variables);
			}
		}

		public OperationResult<bool> RemoveGlobalVariable(GlobalVariable variable)
		{
			using (var db = new SKDDatabaseService())
			{
				return db.GlobalVariablesTranslator.Remove(variable);
			}
		}

		public OperationResult<List<GlobalVariable>> GetInitialGlobalVariables()
		{
			using (var db = new SKDDatabaseService())
			{
				return db.GlobalVariablesTranslator.GetInitialGlobalVariables();
			}
		}

		public OperationResult<List<GlobalVariable>> GetCurrentGlobalVariables()
		{
			using (var db = new SKDDatabaseService())
			{
				return db.GlobalVariablesTranslator.GetCurrentGlobalVariables();
			}
		}
	}
}