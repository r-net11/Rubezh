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
		LibraryViewModel LibraryViewModel;

		public override void CreateViewModels()
		{
			LibraryViewModel = new LibraryViewModel();
		}

		public override void Initialize()
		{
			LibraryViewModel.Initialize();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>()
			{
				new NavigationItem<ShowLibraryEvent>(LibraryViewModel, "Библиотека", "/Controls;component/Images/book.png"),
			};
		}
		public override string Name
		{
			get { return "Библиотека устройств"; }
		}
	}
}