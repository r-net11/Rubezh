using System.Collections.Generic;
using DiagnosticsModule.ViewModels;
using Infrastructure.Client;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Events;

namespace DiagnosticsModule
{
	public class DiagnosticsModule : ModuleBase
	{
        DiagnosticsViewModel DiagnosticsViewModel;

        public DiagnosticsModule()
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
				new NavigationItem<ShowDiagnosticsEvent>(DiagnosticsViewModel, "Диагностика", "/Controls;component/Images/settings.png"),
			};
		}
		public override string Name
		{
            get { return "Диагностика"; }
		}
	}
}