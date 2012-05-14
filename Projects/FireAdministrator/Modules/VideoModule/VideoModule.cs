using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using VideoModule.ViewModels;
using System.Collections.Generic;
using Infrastructure.Common.Navigation;

namespace VideoModule
{
	public class VideoModule : ModuleBase
	{
		static CamerasViewModel _camerasViewModel;

		public VideoModule()
		{
			ServiceFactory.Events.GetEvent<ShowVideoEvent>().Unsubscribe(OnShowVideos);
			ServiceFactory.Events.GetEvent<ShowVideoEvent>().Subscribe(OnShowVideos);
		}

		void CreateViewModels()
		{
			_camerasViewModel = new CamerasViewModel();
		}

		static void OnShowVideos(object obj)
		{
			ServiceFactory.Layout.Show(_camerasViewModel);
		}

		public override void Initialize()
		{
			CreateViewModels();
			VideoService.Initialize(ServiceFactory.AppSettings.LibVlcDllsPath);
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>()
			{
				new NavigationItem<ShowVideoEvent>("Видео", "/Controls;component/Images/Video1.png"),
			};
		}
	}
}