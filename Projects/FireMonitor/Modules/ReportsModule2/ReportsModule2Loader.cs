using System.Collections.Generic;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Events;
using ReportsModule2.ViewModels;

namespace ReportsModule2
{
	public class ReportsModule2Loader : ModuleBase
	{
		Reports2ViewModel Reports2ViewModel;

		public ReportsModule2Loader()
		{
		    ServiceFactory.Events.GetEvent<ShowReports2Event>().Subscribe(OnShowReports2);
		    Reports2ViewModel = new Reports2ViewModel();
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
		        new NavigationItem<ShowReports2Event>("Отчеты(2)", "/Controls;component/Images/levels.png"),
		    };
		}

		public override string Name
		{
		    get { return "Отчеты(2)"; }
		}
	}
}