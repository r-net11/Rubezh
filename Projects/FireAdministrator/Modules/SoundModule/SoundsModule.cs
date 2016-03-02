using System.Collections.Generic;
using FiresecAPI;
using FiresecAPI.Enums;
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
				new NavigationItem<ShowSoundsEvent>(SoundsViewModel, ModuleType.ToDescription(), "music"),
			};
		}
		public override ModuleType ModuleType
		{
			get { return ModuleType.Sounds; }
		}
	}
}