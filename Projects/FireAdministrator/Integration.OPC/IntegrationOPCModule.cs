using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Client;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Events;
using Integration.OPC.Models;
using Integration.OPC.ViewModels;
using StrazhAPI;
using StrazhAPI.Enums;
using StrazhAPI.SKD;

namespace Integration.OPC
{
	public class IntegrationOPCModule : ModuleBase
	{
		private ZonesOPCViewModel _integrationOPCViewModel;
		public override ModuleType ModuleType
		{
			get { return ModuleType.IntegrationOPC; }
		}

		public override void CreateViewModels()
		{
			_integrationOPCViewModel = new ZonesOPCViewModel();
		}

		public override void Initialize()
		{
			_integrationOPCViewModel.Initialize(SKDManager.SKDConfiguration.OPCZones.Select(x => new OPCZone(x)));
		}

		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>
			{
				new NavigationItem<ShowZonesOPCEvent>(_integrationOPCViewModel, ModuleType.ToDescription(), "music")
			};
		}
	}
}
