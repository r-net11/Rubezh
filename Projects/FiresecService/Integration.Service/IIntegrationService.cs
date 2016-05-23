using System.Collections.Generic;
using StrazhAPI.Integration.OPC;

namespace Integration.Service
{
	public interface IIntegrationService
	{
		bool PingOPCServer();

		List<OPCZone> GetOPCZones();

		void SetNewConfig();
	}
}
