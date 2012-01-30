using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using SoundsModule.ViewModels;

namespace SoundsModule
{
    public class SoundsModule
    {
        static SoundsViewModel _soundsViewModel;

        public SoundsModule()
        {
            ServiceFactory.Events.GetEvent<ShowSoundsEvent>().Subscribe(OnShowSounds);

            RegisterResources();
            CreateViewModels();
        }

        void RegisterResources()
        {
            ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));
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
    }
}