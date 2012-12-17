using System.Collections.Generic;
using FiltersModule.ViewModels;
using Infrastructure.Client;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Events;

namespace FiltersModule
{
	public class FilterModule : ModuleBase
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
				new NavigationItem<ShowJournalEvent>(FiltersViewModel, "Фильтры журнала", "/Controls;component/Images/filter.png"),
			};
		}
		public override string Name
		{
			get { return "Фильтры журнала событий"; }
		}
	}
}