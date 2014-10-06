using System;
using System.Collections.Generic;
using FiresecAPI;
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
				new NavigationItem<ShowInstructionsEvent, Guid>(InstructionsViewModel, ModuleType.ToDescription(), "/Controls;component/Images/information.png", null, null, Guid.Empty),
			};
		}
		protected override ModuleType ModuleType
		{
			get { return ModuleType.Instructions; }
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