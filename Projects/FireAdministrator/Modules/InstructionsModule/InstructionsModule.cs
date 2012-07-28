using System.Collections.Generic;
using System.Linq;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Events;
using InstructionsModule.ViewModels;

namespace InstructionsModule
{
	public class InstructionsModule : ModuleBase
	{
		InstructionsViewModel _instructionsViewModel;

		public InstructionsModule()
		{
			ServiceFactory.Events.GetEvent<ShowInstructionsEvent>().Subscribe(OnShowInstructions);
			_instructionsViewModel = new InstructionsViewModel();
		}

		void OnShowInstructions(int? instructionNo)
		{
			if (instructionNo != null)
			{
				_instructionsViewModel.SelectedInstruction = _instructionsViewModel.Instructions.FirstOrDefault(x => x.Instruction.No == instructionNo.Value);
			}
			ServiceFactory.Layout.Show(_instructionsViewModel);
		}

		public override void Initialize()
		{
			_instructionsViewModel.Initialize();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>()
			{
				new NavigationItem<ShowInstructionsEvent, int?>("Инструкции", "/Controls;component/Images/information.png"),
			};
		}
		public override string Name
		{
			get { return "Инструкции"; }
		}
	}
}