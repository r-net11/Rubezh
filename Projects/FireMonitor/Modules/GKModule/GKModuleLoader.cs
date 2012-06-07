using System.Collections.Generic;
using GKModule.ViewModels;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Events;

namespace GKModule
{
	public class GKModuleLoader : ModuleBase
	{
		GKViewModel GKViewModel;

		public GKModuleLoader()
		{
			ServiceFactory.Events.GetEvent<ShowGKEvent>().Subscribe(OnShowCall);
			GKViewModel = new GKViewModel();
		}

		void OnShowCall(object obj)
		{
			ServiceFactory.Layout.Show(GKViewModel);
		}

		public override void Initialize()
		{
			GKViewModel.Initialize();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>()
			{
				new NavigationItem<ShowGKEvent>("ГК", "/Controls;component/Images/Settings.png"),
			};
		}
	}
}