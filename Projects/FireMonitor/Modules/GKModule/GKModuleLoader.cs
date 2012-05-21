using GKModule.ViewModels;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using System.Collections.Generic;
using Infrastructure.Common.Navigation;

namespace GKModule
{
	public class GKModuleLoader : ModuleBase
	{
		GKViewModel GKViewModel;

		public GKModuleLoader()
		{
			ServiceFactory.Events.GetEvent<ShowGKEvent>().Subscribe(OnShowCall);
		}

		void CreateViewModels()
		{
			GKViewModel = new GKViewModel();
		}
		void OnShowCall(object obj)
		{
			ServiceFactory.Layout.Show(GKViewModel);
		}

		public override void Initialize()
		{
			CreateViewModels();
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