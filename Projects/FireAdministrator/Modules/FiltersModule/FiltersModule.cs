using FiltersModule.ViewModels;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using System.Collections.Generic;
using Infrastructure.Common.Navigation;

namespace FiltersModule
{
	public class FilterModule : ModuleBase
	{
		static FiltersViewModel _filtersViewModel;

		public FilterModule()
		{
			ServiceFactory.Events.GetEvent<ShowJournalEvent>().Unsubscribe(OnShowJournal);
			ServiceFactory.Events.GetEvent<ShowJournalEvent>().Subscribe(OnShowJournal);
		}

		void CreateViewModels()
		{
			_filtersViewModel = new FiltersViewModel();
		}

		static void OnShowJournal(object obj)
		{
			ServiceFactory.Layout.Show(_filtersViewModel);
		}

		public override void Initialize()
		{
			CreateViewModels();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>()
			{
				new NavigationItem<ShowJournalEvent>("Фильтры журнала", "/Controls;component/Images/filter.png"),
			};
		}
	}
}