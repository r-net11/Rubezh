using System.Collections.Generic;
using Infrastructure.Client;
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
			_libraryViewModel = new LibraryViewModel();
		}

		public override void Initialize()
		{
			_libraryViewModel.Initialize();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>()
			{
				new NavigationItem<ShowLibraryEvent>(_libraryViewModel, "Библиотека","/Controls;component/Images/book.png"),
			};
		}
		public override string Name
		{
			get { return "Библиотека устройств"; }
		}
	}
}