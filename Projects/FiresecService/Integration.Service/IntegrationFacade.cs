using Integration.Service.OPCIntegration;
using StrazhAPI.Automation.Enums;
using StrazhAPI.Integration.OPC;
using StrazhAPI.SKD;
using System;
using System.Collections.Generic;

namespace Integration.Service
{
	public sealed class IntegrationFacade : IIntegrationService
	{
		private readonly OPCIntegrationService _opcIntegrationService;

		public IntegrationFacade()
		{
			_opcIntegrationService = new OPCIntegrationService();
			if(!_opcIntegrationService.StartOPCIntegrationService(SKDManager.SKDConfiguration.OPCSettings))
				throw new Exception("Can not start OPC integration service.");
		}

		public bool PingOPCServer()
		{
			return _opcIntegrationService.PingOPCServer();
		}

		public List<OPCZone> GetOPCZones()
		{
			return _opcIntegrationService.GetOPCZones();
		}

		public void SetNewConfig()
		{
			_opcIntegrationService.StopOPCIntegrationService();
			_opcIntegrationService.StartOPCIntegrationService(SKDManager.SKDConfiguration.OPCSettings);
		}

		public void SetGuard(int no)
		{
			_opcIntegrationService.SetGuard(no);
		}

		public void UnsetGuard(int no)
		{
			_opcIntegrationService.UnsetGuard(no);
		}

		public List<Script> GetFiresecScripts()
		{
			return _opcIntegrationService.GetFiresecScripts();
		}

		public bool ExecuteFiresecScript(Script script, FiresecCommandType type)
		{
			return _opcIntegrationService.ExecuteFiresecScript(script, type);
		}
	}
}
