using GKModule.ViewModels;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;

namespace GKModule
{
    public class GKModuleLoader
    {
        static GKViewModel GKViewModel;

        public GKModuleLoader()
        {
            ServiceFactory.Events.GetEvent<ShowGKEvent>().Subscribe(OnShowCall);

            RegisterResources();
            CreateViewModels();
        }

        void RegisterResources()
        {
            ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));
        }

        public static void Initialize()
        {
            //GKViewModel.Initialize();
        }

        static void CreateViewModels()
        {
            GKViewModel = new GKViewModel();
        }

        static void OnShowCall(object obj)
        {
            ServiceFactory.Layout.Show(GKViewModel);
        }
    }
}