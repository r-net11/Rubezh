using System.Collections.Generic;
using FiltersModule.ViewModels;
using Infrastructure;
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
			ServiceFactory.Events.GetEvent<ShowJournalEvent>().Subscribe(OnShowJournal);
			_filtersViewModel = new FiltersViewModel();
		}

		void OnShowJournal(object obj)
		{
			ServiceFactory.Layout.Show(_filtersViewModel);
		}

		public override void Initialize()
		{
			_filtersViewModel.Initialize();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>()
			{
				new NavigationItem<ShowJournalEvent>("Фильтры журнала", "/Controls;component/Images/filter.png"),
			};
		}
		public override string Name
		{
			get { return "Фильтры журнала событий"; }
		}
	}
}