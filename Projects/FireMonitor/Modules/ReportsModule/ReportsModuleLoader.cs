using System.Collections.Generic;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Events;
using ReportsModule.ViewModels;

namespace ReportsModule
{
	public class ReportsModuleLoader : ModuleBase
	{
		ReportsViewModel Reports2ViewModel;

		public ReportsModuleLoader()
		{
			ServiceFactory.Layout.AddToolbarItem(new IndicatorViewModel());
			ServiceFactory.Events.GetEvent<ShowReports2Event>().Subscribe(OnShowReports2);
			Reports2ViewModel = new ReportsViewModel();
		}

		void OnShowReports2(object obj)
		{
			ServiceFactory.Layout.Show(Reports2ViewModel);
		}

		public override void Initialize()
		{
		}

		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>()
		    {
#if DEBUG
		        new NavigationItem<ShowReports2Event>("Отчеты", "/Controls;component/Images/levels.png"),
#endif
		    };
		}

		public override string Name
		{
			get { return "Отчеты"; }
		}
	}
}