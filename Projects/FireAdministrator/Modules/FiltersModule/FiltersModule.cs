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
		FiltersViewModel _filtersViewModel;

		public FilterModule()
		{
			_filtersViewModel = new FiltersViewModel();
		}

		public override void Initialize()
		{
			_filtersViewModel.Initialize();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>()
			{
				new NavigationItem<ShowJournalEvent>(_filtersViewModel, "Фильтры журнала", "/Controls;component/Images/filter.png"),
			};
		}
		public override string Name
		{
			get { return "Фильтры журнала событий"; }
		}
	}
}