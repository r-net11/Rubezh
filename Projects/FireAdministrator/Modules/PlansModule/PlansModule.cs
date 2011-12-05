using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using PlansModule.ViewModels;
using PlansModule.Events;
using System;

namespace PlansModule
{
    public class PlansModule
    {
        static bool _hasChanges;
        public static bool HasChanges
        {
            get { return _hasChanges; }
            set
            {
                _hasChanges = value;
                if (value)
                    ServiceFactory.Events.GetEvent<PlanChangedEvent>().Publish(Guid.Empty);
            }
        }

        static PlansViewModel _plansViewModel;

        public PlansModule()
        {
            ServiceFactory.Events.GetEvent<ShowPlansEvent>().Subscribe(OnShowPlans);

            RegisterResources();
            CreateViewModels();
        }

        void RegisterResources()
        {
            ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));
            ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "Designer/Designer/DesignerCanvas.xaml"));
            ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "Designer/Designer/DesignerItem.xaml"));
            ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "Designer/Rectangle/ResizeChrome.xaml"));
            ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "Designer/Polygon/PolygonResizeChrome.xaml"));
        }

        static void CreateViewModels()
        {
            _plansViewModel = new PlansViewModel();
        }

        static void OnShowPlans(string obj)
        {
            ServiceFactory.Layout.Show(_plansViewModel);
        }

        public static void Save()
        {
            _plansViewModel.PlanDesignerViewModel.Save();
        }
    }
}