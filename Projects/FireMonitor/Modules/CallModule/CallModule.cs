using CallModule.ViewModels;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using Microsoft.Practices.Prism.Modularity;

namespace CallModule
{
    public class CallModule : IModule
    {
        static CallViewModel CallViewModel;

        public CallModule()
        {
            ServiceFactory.Events.GetEvent<ShowCallEvent>().Subscribe(OnShowCall);
        }

        public void Initialize()
        {
            RegisterResources();
            CreateViewModels();
        }

        void RegisterResources()
        {
            ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));
        }

        static void CreateViewModels()
        {
            CallViewModel = new CallViewModel();
            CallViewModel.Initialize();
        }

        static void OnShowCall(object obj)
        {
            ServiceFactory.Layout.Show(CallViewModel);
        }
    }
}