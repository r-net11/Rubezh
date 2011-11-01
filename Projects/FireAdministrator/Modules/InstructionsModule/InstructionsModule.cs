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
            ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));
        }

        void CreateViewModels()
        {
            _instructionsViewModel = new InstructionsViewModel();
            _instructionsViewModel.Initialize();
        }

        static void OnShowInstructions(ulong? instructionNo)
        {
            if (instructionNo != null)
            {
                _instructionsViewModel.SelectedInstruction = _instructionsViewModel.Instructions.FirstOrDefault(x => x.InstructionNo == instructionNo.Value);
            }
            ServiceFactory.Layout.Show(_instructionsViewModel);
        }
    }
}