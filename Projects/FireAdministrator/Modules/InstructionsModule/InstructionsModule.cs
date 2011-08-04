using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Modularity;
using InstructionsModule.ViewModels;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;

namespace InstructionsModule
{
    public class InstructionsModule : IModule
    {
        static InstructionsViewModel instructionsViewModel;

        public InstructionsModule()
        {
            ServiceFactory.Events.GetEvent<ShowInstructionsEvent>().Subscribe(OnShowInstructions);
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
            instructionsViewModel = new InstructionsViewModel();
        }

        static void OnShowInstructions(string obj)
        {
            instructionsViewModel.Initialize();
            ServiceFactory.Layout.Show(instructionsViewModel);
        }
    }
}
