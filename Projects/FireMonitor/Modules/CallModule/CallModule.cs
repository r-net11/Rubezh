using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Modularity;
using Infrastructure.Common;
using Infrastructure;
using Infrastructure.Events;
using CallModule.ViewModels;

namespace CallModule
{
    public class CallModule : IModule
    {
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
            var resourceService = ServiceFactory.Get<IResourceService>();
            resourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));
        }

        void CreateViewModels()
        {
            callViewModel = new CallViewModel();
            callViewModel.Initialize();
        }

        static CallViewModel callViewModel;

        static void OnShowCall(object obj)
        {
            ServiceFactory.Layout.Show(callViewModel);
        }
    }
}
