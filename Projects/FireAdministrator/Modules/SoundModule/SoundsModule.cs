using System.Collections.Generic;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Events;
using SoundsModule.ViewModels;
using Infrastructure.Client;

namespace SoundsModule
{
	public class SoundsModule : ModuleBase
	{
		SoundsViewModel _soundsViewModel;

		public SoundsModule()
		{
			_soundsViewModel = new SoundsViewModel();
		}

		public override void Initialize()
		{
			_soundsViewModel.Initialize();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>()
			{
				new NavigationItem<ShowSoundsEvent>(_soundsViewModel, "Звуки", "/Controls;component/Images/music.png"),
			};
		}
		public override string Name
		{
			get { return "Звуки"; }
		}
	}
}