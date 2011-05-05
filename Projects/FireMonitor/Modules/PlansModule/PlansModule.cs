using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Modularity;
using Infrastructure;
using Infrastructure.Events;
using PlansModule.ViewModels;
using System.Diagnostics;
using PlansModule.Views;

namespace PlansModule
{
    public class PlansModule : IModule
    {
        public PlansModule()
        {
            ServiceFactory.Events.GetEvent<ShowPlanEvent>().Subscribe(OnShowPlan);
            ServiceFactory.Events.GetEvent<ShowDeviceOnPlanEvent>().Subscribe(OnShowDeviceOnPlan);
        }

        public void Initialize()
        {
            RegisterResources();
            PlanLoader.Load();
            CreateViewModels();
        }

        void RegisterResources()
        {
            var resourceService = ServiceFactory.Get<IResourceService>();
            resourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));
            resourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/TreeExpanderStyle.xaml"));
        }

        void CreateViewModels()
        {
            plansViewModel = new PlansViewModel();
            plansViewModel.Initialize();
        }

        static PlansViewModel plansViewModel;

        static void OnShowPlan(object obj)
        {
            ServiceFactory.Layout.Show(plansViewModel);
        }

        static void OnShowDeviceOnPlan(string path)
        {
            plansViewModel.ShowDevice(path);
            ServiceFactory.Layout.Show(plansViewModel);
        }
    }
}
