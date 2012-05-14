using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using LibraryModule.ViewModels;
using System.Collections.Generic;
using Infrastructure.Common.Navigation;

namespace LibraryModule
{
    public class LibraryModule : ModuleBase
    {
        static LibraryViewModel _libraryViewModel;

        public LibraryModule()
        {
            ServiceFactory.Events.GetEvent<ShowLibraryEvent>().Unsubscribe(OnShowLibrary);
            ServiceFactory.Events.GetEvent<ShowLibraryEvent>().Subscribe(OnShowLibrary);
        }

        static void CreateViewModels()
        {
            _libraryViewModel = new LibraryViewModel();
        }

        static void OnShowLibrary(object obj)
        {
            ServiceFactory.Layout.Show(_libraryViewModel);
        }

		public override void Initialize()
		{
			CreateViewModels();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>()
			{
				new NavigationItem<ShowLibraryEvent>("Библиотека","/Controls;component/Images/book.png"),
			};
		}
	}
}