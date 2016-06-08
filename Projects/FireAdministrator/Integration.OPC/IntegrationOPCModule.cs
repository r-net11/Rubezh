using System;
using Infrastructure.Client;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Services;
using Infrastructure.Events;
using Infrustructure.Plans.Events;
using Integration.OPC.Models;
using Integration.OPC.Plans;
using Integration.OPC.ViewModels;
using StrazhAPI;
using StrazhAPI.Enums;
using StrazhAPI.Models;
using StrazhAPI.SKD;
using System.Collections.Generic;
using System.Linq;

namespace Integration.OPC
{
	public class IntegrationOPCModule : ModuleBase
	{
		private ZonesOPCViewModel _integrationOPCViewModel;
		private OPCPlanExtension _planExtension;
		public override ModuleType ModuleType
		{
			get { return ModuleType.IntegrationOPC; }
		}

		public override void CreateViewModels()
		{
			_integrationOPCViewModel = new ZonesOPCViewModel();
			_planExtension = new OPCPlanExtension(_integrationOPCViewModel);
		}

		public override void Initialize()
		{
			_integrationOPCViewModel.Initialize(SKDManager.SKDConfiguration.OPCZones.Select(x => new OPCZone(x)));
			_planExtension.Initialize();
			ServiceFactoryBase.Events.GetEvent<RegisterPlanExtensionEvent<Plan>>().Publish(_planExtension);
		}

		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>
			{
				new NavigationItemEx<ShowZonesOPCEvent, Guid>(_integrationOPCViewModel, ModuleType.ToDescription(), "OPCIntegrationMenu", null, null, Guid.Empty)
			};
		}
	}
}
