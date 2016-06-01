using Infrastructure.Client;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Integration.OPC.Events;
using Integration.OPC.Models;
using Integration.OPC.ViewModels;
using StrazhAPI.Enums;
using StrazhAPI.SKD;
using System.Collections.Generic;
using System.Linq;

namespace Integration.OPC
{
	public class IntegrationOPCModuleLoader : ModuleBase
	{
		private OPCZonesViewModel _opcZonesViewModel;
		public override ModuleType ModuleType
		{
			get { return ModuleType.IntegrationOPC; }
		}

		public override void CreateViewModels()
		{
			_opcZonesViewModel = new OPCZonesViewModel();
		}

		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>
			{
				new NavigationItem<ShowOPCIntegrationEvent>(_opcZonesViewModel, "Зоны ОПС", "Kartoteka2W")
			};
		}

		public override void Initialize()
		{
			_opcZonesViewModel.Initialize(SKDManager.SKDConfiguration.OPCZones.Select(x => new OPCZone(x)));
		}
	}
}
