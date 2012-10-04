using System.Collections.Generic;
using DiagnosticsModule.ViewModels;
using Infrastructure;
using Infrastructure.Client;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Events;

namespace DiagnosticsModule
{
    public class DiagnosticsModuleLoader : ModuleBase
    {
        DiagnosticsViewModel DiagnosticsViewModel;

        public DiagnosticsModuleLoader()
        {
            ServiceFactory.Layout.AddToolbarItem(new ImitatorViewModel());
            ServiceFactory.Events.GetEvent<ShowDiagnosticsEvent>().Subscribe(OnShowDiagnostics);
            DiagnosticsViewModel = new DiagnosticsViewModel();
        }

        void OnShowDiagnostics(object obj)
        {
            ServiceFactory.Layout.Show(DiagnosticsViewModel);
        }

		public override void Initialize()
		{
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>()
			{
			    new NavigationItem<ShowDiagnosticsEvent, object>(DiagnosticsViewModel, "Диагностика", "/Controls;component/Images/Bug.png")
			};
		}
		public override string Name
		{
            get { return "Диагностика"; }
		}
	}
}