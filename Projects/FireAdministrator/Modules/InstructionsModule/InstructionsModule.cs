using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using InstructionsModule.ViewModels;
using Microsoft.Practices.Prism.Modularity;
using System.Linq;

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

        static void OnShowInstructions(string instructionNo)
        {
            if (string.IsNullOrEmpty(instructionNo) == false)
            {
                _instructionsViewModel.SelectedInstruction = _instructionsViewModel.Instructions.FirstOrDefault(x => x.InstructionNo == instructionNo);
            }
            ServiceFactory.Layout.Show(_instructionsViewModel);
        }
    }
}
