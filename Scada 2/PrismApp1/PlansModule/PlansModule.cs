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
        }

        public void Initialize()
        {
            RegisterResources();
            PlanLoader.Load();
        }

        void RegisterResources()
        {
            var resourceService = ServiceFactory.Get<IResourceService>();
            resourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));
        }

        static void OnShowPlan(object obj)
        {
            PlansViewModel fullPlanViewModel = new PlansViewModel();
            fullPlanViewModel.Initialize();
            ServiceFactory.Layout.Show(fullPlanViewModel);
            //fullPlanViewModel.MainCanvas = FullPlanView._MainCanvas;
            //fullPlanViewModel.Initialize();
        }
    }
}
