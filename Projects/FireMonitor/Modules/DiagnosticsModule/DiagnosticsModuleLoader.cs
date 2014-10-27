using System.Collections.Generic;
using DiagnosticsModule.ViewModels;
using FiresecAPI;
using Infrastructure.Client;
using Infrastructure.Client.Layout;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Services.Layout;
using Infrastructure.Events;
using DiagnosticsModule.Events;

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
				new NavigationItem("Диагностика", "/Controls;component/Images/Bug.png",
					new List<NavigationItem>()
					{
						new NavigationItem<ShowDiagnosticsEvent, object>(DiagnosticsViewModel, "Диагностика", "/Controls;component/Images/Bug.png"),
						new NavigationItem<ShowServerEvent, object>(ServerViewModel, "Очередь операций", "/Controls;component/Images/Bug.png")
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