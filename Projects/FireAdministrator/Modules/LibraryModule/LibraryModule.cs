using System.Collections.Generic;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Events;
using LibraryModule.ViewModels;

namespace LibraryModule
{
	public class LibraryModule : ModuleBase
	{
		LibraryViewModel _libraryViewModel;

		public LibraryModule()
		{
			ServiceFactory.Events.GetEvent<ShowLibraryEvent>().Subscribe(OnShowLibrary);
			_libraryViewModel = new LibraryViewModel();
		}

		void OnShowLibrary(object obj)
		{
			ServiceFactory.Layout.Show(_libraryViewModel);
		}

		public override void Initialize()
		{
			_libraryViewModel.Initialize();
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