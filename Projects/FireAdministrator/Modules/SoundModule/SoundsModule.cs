using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using SoundsModule.ViewModels;
using System.Collections.Generic;
using Infrastructure.Common.Navigation;

namespace SoundsModule
{
    public class SoundsModule : ModuleBase
    {
        static SoundsViewModel _soundsViewModel;

        public SoundsModule()
        {
            ServiceFactory.Events.GetEvent<ShowSoundsEvent>().Unsubscribe(OnShowSounds);
            ServiceFactory.Events.GetEvent<ShowSoundsEvent>().Subscribe(OnShowSounds);
        }

        static void CreateViewModels()
        {
            _soundsViewModel = new SoundsViewModel();
            _soundsViewModel.Initialize();
        }

        static void OnShowSounds(object obj)
        {
            ServiceFactory.Layout.Show(_soundsViewModel);
        }

		public override void Initialize()
		{
			CreateViewModels();
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