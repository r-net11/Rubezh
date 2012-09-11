using System.Collections.Generic;
using Infrastructure.Client;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Validation;
using Infrastructure.Events;
using InstructionsModule.Validation;
using InstructionsModule.ViewModels;

namespace InstructionsModule
{
	public class InstructionsModule : ModuleBase, IValidationModule
	{
		InstructionsViewModel _instructionsViewModel;

		public InstructionsModule()
		{
			_instructionsViewModel = new InstructionsViewModel();
		}

		public override void Initialize()
		{
			_instructionsViewModel.Initialize();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>()
			{
				new NavigationItem<ShowInstructionsEvent, int?>(_instructionsViewModel, "Инструкции", "/Controls;component/Images/information.png"),
			};
		}
		public override string Name
		{
			get { return "Инструкции"; }
		}

		#region IValidationModule Members

		public IEnumerable<IValidationError> Validate()
		{
			return Validator.Validate();
		}

		#endregion
	}
}