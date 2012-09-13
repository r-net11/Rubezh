using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using DiagnosticsModule.ViewModels;
using Infrastructure;
using Infrastructure.Events;
using Infrastructure.Common.Navigation;

namespace DiagnosticsModule
{
    public class DiagnosticsModuleLoader : ModuleBase
    {
        DiagnosticsViewModel DiagnosticsViewModel;

        public DiagnosticsModuleLoader()
        {
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
                //new NavigationItem<ShowDiagnosticsEvent, object>(DiagnosticsViewModel, "Диагностика", "/Controls;component/Images/Settings.png")
            };
        }
		public override string Name
		{
            get { return "Диагностика"; }
		}
	}
}