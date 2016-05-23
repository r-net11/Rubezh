using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Common;
using Integration.Service.Entities;
using Integration.Service.OPCIntegration;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using StrazhAPI.Enums;
using StrazhAPI.Integration.OPC;
using StrazhAPI.SKD;

namespace Integration.Service
{
	public sealed class IntegrationFacade : IIntegrationService
	{
//		private const int DefaultTimeoutMilliseconds = 15000;
		private OPCIntegrationService opcIntegrationService;
	//	#region Singleton implementation
		//private static volatile IntegrationFacade _instance;
	//	private static readonly object SyncRoot = new object();

		public IntegrationFacade()
		{
			opcIntegrationService = new OPCIntegrationService();
			if(!opcIntegrationService.StartOPCIntegrationService(SKDManager.SKDConfiguration.OPCSettings))
				throw new Exception("Can not start OPC integration service.");
		}

		//public static IntegrationFacade Instance
		//{
		//	get
		//	{
		//		if (_instance != null) return _instance;

		//		lock (SyncRoot)
		//		{
		//			if(_instance == null)
		//				_instance = new IntegrationFacade();
		//		}

		//		return _instance;
		//	}
		//}
		//#endregion

		public bool PingOPCServer()
		{
			return opcIntegrationService.PingOPCServer();
		}

		public List<OPCZone> GetOPCZones()
		{
			return opcIntegrationService.GetOPCZones();
		}

		public void SetNewConfig()
		{
			opcIntegrationService.StopOPCIntegrationService();
			opcIntegrationService.StartOPCIntegrationService(SKDManager.SKDConfiguration.OPCSettings);
		}

		//private static OPCZone GetOPCZoneType(string zoneType, OPCZone zone)
		//{
		//	OPCZoneType resulType;
		//	if (!Enum.TryParse(zoneType, true, out resulType)) return null;
		//	zone.Type = resulType;
		//	return zone;
		//}

		//private static OPCZone GetGuardZoneType(string zoneType, OPCZone zone)
		//{
		//	GuardZoneType resulType;
		//	if (!Enum.TryParse(zoneType, true, out resulType)) return null;
		//	zone.GuardZoneType = resulType;
		//	return zone;
		//}
	}
}
