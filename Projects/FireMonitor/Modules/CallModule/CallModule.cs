using CallModule.ViewModels;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;

namespace CallModule
{
    public class CallModule
    {
        static CallViewModel CallViewModel;

        public CallModule()
        {
            ServiceFactory.Events.GetEvent<ShowCallEvent>().Subscribe(OnShowCall);

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
        }

        static void OnShowCall(object obj)
        {
            ServiceFactory.Layout.Show(CallViewModel);
        }
    }
}