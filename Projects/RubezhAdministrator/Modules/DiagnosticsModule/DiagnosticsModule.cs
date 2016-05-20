using System.Collections.Generic;
using System.Diagnostics;
using DiagnosticsModule.ViewModels;
using RubezhAPI;
using Infrastructure.Client;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Events;
using RubezhClient;

namespace DiagnosticsModule
{
	public class DiagnosticsModule : ModuleBase
	{
		DiagnosticsViewModel DiagnosticsViewModel;
		ZonesViewModel ZonesViewModel;

		public override void CreateViewModels()
		{
			DiagnosticsViewModel = new DiagnosticsViewModel();
			ZonesViewModel = new ZonesViewModel();
		}

		public override void Initialize()
		{
			ZonesViewModel.Initialize(ClientManager.SystemConfiguration.Zones);
		}

		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>()
			{
				new NavigationItem<ShowDiagnosticsEvent>(DiagnosticsViewModel, "Диагностика", "Star"),
				new NavigationItem<ShowDiagnosticsZonesEvent>(ZonesViewModel, "Диагностика Дерево", "Star"),
			};
		}

		public override ModuleType ModuleType
		{
			get { return ModuleType.Diagnostics; }
		}
	}
}