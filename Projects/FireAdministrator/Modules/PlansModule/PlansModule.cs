using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using PlansModule.ViewModels;

namespace PlansModule
{
    public class PlansModule
    {
        static PlansViewModel _plansViewModel;

        public PlansModule()
        {
            ServiceFactory.Events.GetEvent<ShowPlansEvent>().Unsubscribe(OnShowPlans);
            ServiceFactory.Events.GetEvent<ShowPlansEvent>().Subscribe(OnShowPlans);

            RegisterResources();
            CreateViewModels();
        }

        void RegisterResources()
        {
            ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));
            ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "Designer/Designer/DesignerCanvas.xaml"));
            ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "Designer/Designer/DesignerItem.xaml"));
            ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "Designer/Rectangle/ResizeChromeRectangle.xaml"));
            ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "Designer/Polygon/ResizeChromePolygon.xaml"));
        }

        public static void CreateViewModels()
        {
            _plansViewModel = new PlansViewModel();
            
        }

        static void OnShowPlans(object obj)
        {
            ServiceFactory.Layout.Show(_plansViewModel);
        }

        public static void Save()
        {
            _plansViewModel.PlanDesignerViewModel.Save();
        }

        void Test()
        {
            var plans = FiresecManager.PlansConfiguration.Plans;

            var subPlan1 = new Plan()
            {
                Caption = "Левое крыло",
                Width = 500,
                Height = 500
            };
            subPlan1.Children.Add(plans[0]);
            subPlan1.Children.Add(plans[1]);
            subPlan1.Children.Add(plans[2]);
            subPlan1.Children.Add(plans[3]);

            var subPlan2 = new Plan()
            {
                Caption = "Правое крыло",
                Width = 500,
                Height = 500
            };
            subPlan2.Children.Add(plans[4]);
            subPlan2.Children.Add(plans[5]);
            subPlan2.Children.Add(plans[6]);
            subPlan2.Children.Add(plans[7]);

            var rootPlan = new Plan()
            {
                Caption = "Объект 1",
                Width = 500,
                Height = 500
            };
            rootPlan.Children.Add(subPlan1);
            rootPlan.Children.Add(subPlan2);

            plans.Clear();
            plans.Add(rootPlan);

            FiresecManager.PlansConfiguration.Update();
        }
    }
}