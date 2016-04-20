using System.Collections.Generic;
using DiagnosticsModule.Events;
using DiagnosticsModule.ViewModels;
using Infrastructure.Client;
using Infrastructure.Client.Layout;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Navigation;
using Infrastructure.Common.Windows.Services.Layout;
using Infrastructure.Events;

namespace DiagnosticsModule
{
	public class DiagnosticsModuleLoader : ModuleBase, ILayoutProviderModule
	{
		DiagnosticsViewModel DiagnosticsViewModel;
		ServerViewModel ServerViewModel;

		public override void CreateViewModels()
		{
			DiagnosticsViewModel = new DiagnosticsViewModel();
			ServerViewModel = new ServerViewModel();
		}

		public override void Initialize()
		{
		}
		public override void AfterInitialize()
		{
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>
				{
				new NavigationItem("Диагностика", "Bug",
					new List<NavigationItem>()
					{
						new NavigationItem<ShowDiagnosticsEvent, object>(DiagnosticsViewModel, "Диагностика", "Bug"),
						new NavigationItem<ShowServerEvent, object>(ServerViewModel, "Очередь операций", "Bug")
					})
				};
		}

		public override ModuleType ModuleType
		{
			get { return ModuleType.Diagnostics; }
		}
		public override void Dispose()
		{
			if (DiagnosticsViewModel != null)
				DiagnosticsViewModel.StopThreads();
		}

		#region ILayoutProviderModule Members
		public IEnumerable<ILayoutPartPresenter> GetLayoutParts()
		{
			yield return new LayoutPartPresenter(LayoutPartIdentities.Diagnostics, "Диагностика", "Bug.png", (p) => DiagnosticsViewModel);
		}
		#endregion
	}
}