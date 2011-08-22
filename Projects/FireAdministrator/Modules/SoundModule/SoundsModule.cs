using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using Microsoft.Practices.Prism.Modularity;
using SoundsModule.ViewModels;
using SoundsModule.Views;
using System.ComponentModel;
using System.Windows;

namespace SoundsModule
{
    public class SoundsModule : IModule
    {
        public SoundsModule()
        {
            HasChanged = false;
            ServiceFactory.Events.GetEvent<ShowSoundsEvent>().Subscribe(OnShowSounds);
        }

        public void Initialize()
        {
            RegisterResources();
            CreateViewModels();
        }

        static SoundsViewModel _soundsViewModel;
        public static bool HasChanged { get; set; }

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

        static void OnShowSounds(string obj)
        {
            ServiceFactory.Layout.Show(_soundsViewModel);
        }

        public static void Save()
        {
            _soundsViewModel.Save();
        }
    }
}