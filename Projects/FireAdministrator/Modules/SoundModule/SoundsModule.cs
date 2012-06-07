using System.Collections.Generic;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Events;
using SoundsModule.ViewModels;

namespace SoundsModule
{
	public class SoundsModule : ModuleBase
	{
		SoundsViewModel _soundsViewModel;

		public SoundsModule()
		{
			ServiceFactory.Events.GetEvent<ShowSoundsEvent>().Subscribe(OnShowSounds);
			_soundsViewModel = new SoundsViewModel();
		}

		void OnShowSounds(object obj)
		{
			ServiceFactory.Layout.Show(_soundsViewModel);
		}

		public override void Initialize()
		{
			_soundsViewModel.Initialize();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>()
			{
				new NavigationItem<ShowSoundsEvent>("Звуки", "/Controls;component/Images/music.png"),
			};
		}
	}
}