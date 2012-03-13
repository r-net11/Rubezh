using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using GroupControllerModule.ViewModels;

namespace GroupControllerModule
{
    public class GroupControllerModule
    {
        static DevicesViewModel _groupControllerViewModel;

        public GroupControllerModule()
        {
            ServiceFactory.Events.GetEvent<ShowGroupControllerEvent>().Unsubscribe(OnShowGroupController);
            ServiceFactory.Events.GetEvent<ShowGroupControllerEvent>().Subscribe(OnShowGroupController);

            RegisterResources();
            CreateViewModels();
        }

        void RegisterResources()
        {
            ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));
        }

        void CreateViewModels()
        {
            _groupControllerViewModel = new DevicesViewModel();
        }

        static void OnShowGroupController(object obj)
        {
            ServiceFactory.Layout.Show(_groupControllerViewModel);
        }
    }
}