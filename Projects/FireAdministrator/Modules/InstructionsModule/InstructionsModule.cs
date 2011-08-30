using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using InstructionsModule.Validation.ViewModels;
using Microsoft.Practices.Prism.Modularity;
using InstructionsModule.ViewModels;

namespace InstructionsModule
{
    public class InstructionsModule : IModule
    {
        static InstructionsViewModel _instructionsViewModel;
        public static bool HasChanges { get; set; }

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
