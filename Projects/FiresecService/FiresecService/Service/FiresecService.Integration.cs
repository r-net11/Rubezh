using System;
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
			try
			{
				return new OperationResult<List<OPCZone>>(_integrationService.GetOPCZones());
			}
			catch (Exception e)
			{
				return new OperationResult<List<OPCZone>>
				{
					Errors = new List<string> {e.ToString()}
				};
			}
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
			try
			{
				return new OperationResult<List<Script>>(_integrationService.GetFiresecScripts());
			}
			catch (Exception e)
			{
				return new OperationResult<List<Script>>
				{
					Errors = new List<string> {e.ToString()}
				};
			}
		}

		public OperationResult<bool> SendOPCScript(OPCCommandType type)
		{
			return new OperationResult<bool>(_integrationService.SendOPCCommandType(type));
		}
	}
}
