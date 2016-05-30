using System.Collections.Generic;
using StrazhAPI;
using StrazhAPI.Automation.Enums;
using StrazhAPI.Enums;
using StrazhAPI.Integration.OPC;

namespace FiresecService.Service
{
	public partial class FiresecService
	{
		public OperationResult<bool> PingOPCServer()
		{
			var result = _integrationService.PingOPCServer();

			return new OperationResult<bool>(result);
		}

		public OperationResult<List<OPCZone>> GetOPCZones()
		{
			var result = _integrationService.GetOPCZones();

			return new OperationResult<List<OPCZone>>(result);
		}

		public OperationResult SetGuard(int no)
		{
			_integrationService.SetGuard(no);
			return new OperationResult();
		}

		public OperationResult UnsetGuard(int no)
		{
			_integrationService.UnsetGuard(no);
			return new OperationResult();
		}

		public OperationResult<bool> ExecuteFiresecScript(Script script, FiresecCommandType type)
		{
			return new OperationResult<bool>(_integrationService.ExecuteFiresecScript(script, type));
		}

		public OperationResult<List<Script>> GetFiresecScripts()
		{
			var result = _integrationService.GetFiresecScripts();

			if (result == null)
				return new OperationResult<List<Script>>
				{
					Errors = new List<string> {"Connection problems."}
				};

			return new OperationResult<List<Script>>(result);
		}

		public OperationResult<bool> SendOPCScript(OPCCommandType type)
		{
			return new OperationResult<bool>(_integrationService.SendOPCCommandType(type));
		}
	}
}
