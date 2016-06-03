using System.Collections.Generic;
using StrazhAPI.Automation.Enums;
using StrazhAPI.Enums;
using StrazhAPI.Integration.OPC;

namespace Integration.Service
{
	public interface IIntegrationService
	{
		bool PingOPCServer();

		IEnumerable<OPCZone> GetOPCZones();

		void SetNewConfig();

		void SetGuard(int no);

		void UnsetGuard(int no);

		IEnumerable<Script> GetFiresecScripts();

		bool ExecuteFiresecScript(Script script, FiresecCommandType type);

		bool SendOPCCommandType(OPCCommandType type);

		IEnumerable<OPCZone> GetGuardZones();
	}
}
