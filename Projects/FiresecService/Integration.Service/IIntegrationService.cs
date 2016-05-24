using System.Collections.Generic;
using StrazhAPI.Integration.OPC;
using StrazhAPI.Models;

namespace Integration.Service
{
	public interface IIntegrationService
	{
		bool PingOPCServer();

		List<OPCZone> GetOPCZones();

		void SetNewConfig();

		void SetGuard(int no);

		void UnsetGuard(int no);
	}
}
