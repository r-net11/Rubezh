using System.Collections.Generic;
using System.Diagnostics;
using DiagnosticsModule.ViewModels;
using RubezhAPI;
using Infrastructure.Client;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Events;

namespace DiagnosticsModule
{
	public class DiagnosticsModule : ModuleBase
	{
		DiagnosticsViewModel DiagnosticsViewModel;

		public override void CreateViewModels()
		{
			DiagnosticsViewModel = new DiagnosticsViewModel();
		}

		public override void Initialize()
		{
		}

		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>()
			{
				new NavigationItem<ShowDiagnosticsEvent>(DiagnosticsViewModel, ModuleType.ToDescription(), "Star"),
			};
		}

		public override ModuleType ModuleType
		{
			get { return ModuleType.Diagnostics; }
		}

		public override void Dispose()
		{
		}
	}
}