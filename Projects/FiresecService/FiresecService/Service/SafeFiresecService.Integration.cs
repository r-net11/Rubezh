using System.Collections.Generic;
using Common;
using StrazhAPI;
using StrazhAPI.Automation.Enums;
using StrazhAPI.Enums;
using StrazhAPI.Integration.OPC;

namespace FiresecService.Service
{
	public partial class SafeFiresecService
	{
		public OperationResult<bool> PingOPCServer()
		{
			return SafeContext.Execute(() => FiresecService.PingOPCServer());
		}

		public OperationResult<IEnumerable<OPCZone>> GetOPCZones()
		{
			return SafeContext.Execute(() => FiresecService.GetOPCZones());
		}

		public OperationResult<IEnumerable<OPCZone>> GetGuardZones()
		{
			return SafeContext.Execute(() => FiresecService.GetGuardZones());
		}

		public OperationResult SetGuard(int no)
		{
			return SafeContext.Execute(() => FiresecService.SetGuard(no));
		}

		public OperationResult UnsetGuard(int no)
		{
			return SafeContext.Execute(() => FiresecService.UnsetGuard(no));
		}

		public OperationResult<bool> ExecuteFiresecScript(Script script, FiresecCommandType type)
		{
			return SafeContext.Execute(() => FiresecService.ExecuteFiresecScript(script, type));
		}

		public OperationResult<IEnumerable<Script>> GetFiresecScripts()
		{
			return SafeContext.Execute(() => FiresecService.GetFiresecScripts());
		}

		public OperationResult<bool> SendOPCScript(OPCCommandType type)
		{
			return SafeContext.Execute(() => FiresecService.SendOPCScript(type));
		}
	}
}
