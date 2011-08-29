using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Modularity;
using InstructionsModule.ViewModels;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using InstructionsModule.Validation.ViewModels;

namespace InstructionsModule
{
    public class InstructionsModule : IModule
    {
        public InstructionsModule()
        {
            ServiceFactory.Events.GetEvent<ShowInstructionsEvent>().Subscribe(OnShowInstructions);
            HasChanges = false;
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
            _instructionsViewModel = new InstructionsViewModel();
            _instructionsViewModel.Initialize();
        }

        static InstructionsViewModel _instructionsViewModel;
        public static bool HasChanges { get; set; }

        public static void Validate()
        {
            var validationErrorsViewModel = new ValidationErrorsViewModel();
            ServiceFactory.Layout.ShowValidationArea(validationErrorsViewModel);
        }

        static void OnShowInstructions(string obj)
        {
            ServiceFactory.Layout.Show(_instructionsViewModel);
        }
    }
}
