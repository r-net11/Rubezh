using System.Linq;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using InstructionsModule.ViewModels;

namespace InstructionsModule
{
    public class InstructionsModule
    {
        static InstructionsViewModel _instructionsViewModel;

        public InstructionsModule()
        {
            ServiceFactory.Events.GetEvent<ShowInstructionsEvent>().Subscribe(OnShowInstructions);

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
                _instructionsViewModel.SelectedInstruction = _instructionsViewModel.Instructions.FirstOrDefault(x => x.Instruction.No == instructionNo.Value);
            }
            ServiceFactory.Layout.Show(_instructionsViewModel);
        }
    }
}