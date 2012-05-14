using System.Linq;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using InstructionsModule.ViewModels;
using System.Collections.Generic;
using Infrastructure.Common.Navigation;

namespace InstructionsModule
{
    public class InstructionsModule : ModuleBase
    {
        static InstructionsViewModel _instructionsViewModel;

        public InstructionsModule()
        {
            ServiceFactory.Events.GetEvent<ShowInstructionsEvent>().Unsubscribe(OnShowInstructions);
            ServiceFactory.Events.GetEvent<ShowInstructionsEvent>().Subscribe(OnShowInstructions);
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

		public override void Initialize()
		{
			CreateViewModels();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>()
			{
				new NavigationItem<ShowInstructionsEvent, ulong?>("Инструкции", "/Controls;component/Images/information.png"),
			};
		}
	}
}