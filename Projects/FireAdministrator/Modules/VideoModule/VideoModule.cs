using System.Collections.Generic;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Events;
using VideoModule.ViewModels;

namespace VideoModule
{
    public class VideoModule : ModuleBase
    {
        CamerasViewModel _camerasViewModel;

        public VideoModule()
        {
            ServiceFactory.Events.GetEvent<ShowVideoEvent>().Subscribe(OnShowVideos);
            _camerasViewModel = new CamerasViewModel();
            VideoService.Initialize(ServiceFactory.AppSettings.LibVlcDllsPath);
        }

        void OnShowVideos(object obj)
        {
            ServiceFactory.Layout.Show(_camerasViewModel);
        }

        public override void Initialize()
        {
            _camerasViewModel.Initialize();
        }
        public override IEnumerable<NavigationItem> CreateNavigation()
        {
            return new List<NavigationItem>()
			{
				new NavigationItem<ShowVideoEvent>("Видео", "/Controls;component/Images/Video1.png"),
			};
        }
        public override string Name
        {
            get { return "Видео"; }
        }
    }
}