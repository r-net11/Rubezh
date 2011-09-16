using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using Microsoft.Practices.Prism.Modularity;
using SoundsModule.ViewModels;

namespace SoundsModule
{
    public class SoundsModule : IModule
    {
        public SoundsModule()
        {
            HasChanges = false;
            ServiceFactory.Events.GetEvent<ShowSoundsEvent>().Subscribe(OnShowSounds);
        }

        public void Initialize()
        {
            RegisterResources();
            CreateViewModels();
        }

        void RegisterResources()
        {
            var resourceService = ServiceFactory.Get<IResourceService>();
            resourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));
        }

        static void CreateViewModels()
        {
            _soundsViewModel = new SoundsViewModel();
            _soundsViewModel.Inicialize();
        }

        static SoundsViewModel _soundsViewModel;
        public static bool HasChanges { get; set; }

        static void OnShowSounds(string obj)
        {
            ServiceFactory.Layout.Show(_soundsViewModel);
        }
    }
}