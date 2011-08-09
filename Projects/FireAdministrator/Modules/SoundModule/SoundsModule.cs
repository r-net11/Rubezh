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
            soundsViewModel = new SoundsViewModel();
            soundsViewModel.Initialize();
        }

        static SoundsViewModel soundsViewModel;

        static void OnShowSounds(string obj)
        {
            ServiceFactory.Layout.Show(soundsViewModel);
        }

        public void Save()
        {
            soundsViewModel.Save();
        }
    }
}
