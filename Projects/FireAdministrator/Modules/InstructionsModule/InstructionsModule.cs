using System;
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
		InstructionsViewModel InstructionsViewModel;

		public override void CreateViewModels()
		{
			InstructionsViewModel = new InstructionsViewModel();
		}

		public override void Initialize()
		{
			InstructionsViewModel.Initialize();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>()
			{
				new NavigationItem<ShowInstructionsEvent, Guid>(InstructionsViewModel, "Инструкции", "/Controls;component/Images/information.png", null, null, Guid.Empty),
			};
		}
		public override string Name
		{
			get { return "Инструкции"; }
		}

		#region IValidationModule Members
		public IEnumerable<IValidationError> Validate()
		{
			var validator = new Validator();
			return validator.Validate();
		}
		#endregion
	}
}