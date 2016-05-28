using System.Collections.Generic;
using StrazhAPI.Automation.Enums;
using StrazhAPI.Integration.OPC;

namespace Integration.Service
{
	public interface IIntegrationService
	{
		bool PingOPCServer();

		List<OPCZone> GetOPCZones();

		void SetNewConfig();

		void SetGuard(int no);

		void UnsetGuard(int no);

		List<Script> GetFiresecScripts();

		bool ExecuteFiresecScript(Script script, FiresecCommandType type);
	}
}
