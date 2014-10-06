using System.Collections.Generic;
using FiresecAPI;
using Infrastructure.Client;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Events;
using SoundsModule.ViewModels;

namespace SoundsModule
{
	public class SoundsModule : ModuleBase
	{
		SoundsViewModel SoundsViewModel;

		public override void CreateViewModels()
		{
			SoundsViewModel = new SoundsViewModel();
		}

		public override void Initialize()
		{
			SoundsViewModel.Initialize();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>()
			{
				new NavigationItem<ShowSoundsEvent>(SoundsViewModel, ModuleType.ToDescription(), "/Controls;component/Images/music.png"),
			};
		}
		protected override ModuleType ModuleType
		{
			get { return ModuleType.Sounds; }
		}
	}
}