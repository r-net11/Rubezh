using System.Collections.Generic;
using FiresecAPI.Models.Layouts;
using Infrastructure.Client;
using Infrastructure.Client.Layout;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Services.Layout;
using Infrastructure.Events;
using VideoModule.ViewModels;

namespace VideoModule
{
	public class VideoModule : ModuleBase, ILayoutDeclarationModule
	{
		CamerasViewModel CamerasViewModel;

		public override void CreateViewModels()
		{
			CamerasViewModel = new CamerasViewModel();
		}

		public override void Initialize()
		{
			CamerasViewModel.Initialize();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>()
			{
				new NavigationItem<ShowVideoEvent>(CamerasViewModel,"Видео", "/Controls;component/Images/Video1.png"),
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

		public IEnumerable<ILayoutPartDescription> GetLayoutPartDescriptions()
		{
			yield return new LayoutPartDescription(LayoutPartIdentities.Video, 203, "Видео", "Панель видео", "BVideo.png");
			yield return new LayoutPartDescription(LayoutPartIdentities.Camera, 204, "Камера", "Изображение с камеры", "BVideo.png")
			{
				Factory = (p) => new LayoutPartCameraViewModel(p as LayoutPartCameraProperties),
			};
		}
	}
}