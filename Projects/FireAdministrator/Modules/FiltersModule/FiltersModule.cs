using System.Collections.Generic;
using FiltersModule.ViewModels;
using Infrastructure.Client;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Events;
using Infrastructure.Common.Validation;
using System;
using FiltersModule.Events;
using FilterModule.Validation;

namespace FiltersModule
{
	public class FilterModule : ModuleBase, IValidationModule
	{
		FiltersViewModel FiltersViewModel;

		public override void CreateViewModels()
		{
			FiltersViewModel = new FiltersViewModel();
		}

		public override void Initialize()
		{
			FiltersViewModel.Initialize();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>()
			{
				new NavigationItem<ShowFiltersEvent, Guid>(FiltersViewModel, "Фильтры", "/Controls;component/Images/Filter.png"),
			};
		}
		public override string Name
		{
			get { return "Фильтры журнала событий"; }
		}

		public IEnumerable<IValidationError> Validate()
		{
			var validator = new Validator();
			return validator.Validate();
		}
	}
}