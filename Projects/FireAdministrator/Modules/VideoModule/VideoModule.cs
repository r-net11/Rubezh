using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using VideoModule.ViewModels;

namespace VideoModule
{
    public class VideoModule
    {
        static CamerasViewModel _camerasViewModel;

        public VideoModule()
        {
            ServiceFactory.Events.GetEvent<ShowVideoEvent>().Unsubscribe(OnShowVideos);
            ServiceFactory.Events.GetEvent<ShowVideoEvent>().Subscribe(OnShowVideos);

            RegisterResources();
            CreateViewModels();
        }

        void RegisterResources()
        {
            ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));
        }

        void CreateViewModels()
        {
            _camerasViewModel = new CamerasViewModel();
        }

        static void OnShowVideos(object obj)
        {
            ServiceFactory.Layout.Show(_camerasViewModel);
        }
    }
}