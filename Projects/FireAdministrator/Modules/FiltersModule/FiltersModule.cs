using System;
using System.Collections.Generic;
using FilterModule.Validation;
using FiltersModule.Events;
using FiltersModule.ViewModels;
using Infrastructure.Client;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Validation;
using Infrastructure.Common.Services.Layout;
using Infrastructure.Client.Layout;
using FiresecAPI.Models.Layouts;

namespace FiltersModule
{
	public class FilterModule : ModuleBase, IValidationModule, ILayoutDeclarationModule
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

		#region ILayoutDeclarationModule Members

		public IEnumerable<ILayoutPartDescription> GetLayoutPartDescriptions()
		{
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.Common, LayoutPartIdentities.Journal, 120, "Журнал", "Панель журнал", "BLevels.png") { Factory = (p) => new LayoutPartJournalViewModel(p as LayoutPartJournalProperties), };
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.Common, LayoutPartIdentities.Archive, 121, "Архив", "Панель архив", "BLevels.png");
		}

		#endregion
	}
}