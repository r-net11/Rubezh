using System.Collections.Generic;
using Infrastructure;
using Infrastructure.Client;
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
			_camerasViewModel = new CamerasViewModel();
			VideoService.Initialize(ServiceFactory.AppSettings.LibVlcDllsPath);
		}

		public override void Initialize()
		{
			_camerasViewModel.Initialize();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>()
			{
				new NavigationItem<ShowVideoEvent>(_camerasViewModel,"Видео", "/Controls;component/Images/Video1.png"),
			};
		}
		public override string Name
		{
			get { return "Видео"; }
		}
        public override void Dispose()
        {
            VideoService.Close();
        }
	}
}