using System;
using System.Collections.Generic;
using Infrastructure.Common.Validation;
using SoundsModule.Validation;
using StrazhAPI;
using StrazhAPI.Enums;
using Infrastructure.Client;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Events;
using SoundsModule.ViewModels;

namespace SoundsModule
{
	public class SoundsModule : ModuleBase, IValidationModule
	{
		SoundsViewModel SoundsViewModel;

		public override void CreateViewModels()
		{
			SoundsViewModel = new SoundsViewModel();
		}

		public override void Initialize()
		{
		}

		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>()
			{
				new NavigationItem<ShowSoundsEvent, Guid>(SoundsViewModel, ModuleType.ToDescription(), "music"),
			};
		}

		public override ModuleType ModuleType
		{
			get { return ModuleType.Sounds; }
		}

		#region IValidationModule

		public IEnumerable<IValidationError> Validate()
		{
			return new Validator().Validate();
		}

		#endregion
	}
}