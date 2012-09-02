using System.Collections.Generic;
using FiresecAPI.Models;
using Infrastructure.Common.Navigation;
using Microsoft.Practices.Prism.Events;
using Infrastructure.Common.Windows.ViewModels;

namespace Infrastructure
{
	public class NavigationItem<T> : NavigationItem<T, object>
	where T : CompositePresentationEvent<object>, new()
	{
		public NavigationItem(string title, string icon = null, IList<NavigationItem> childs = null, PermissionType? permission = null, ViewPartViewModel viewPartViewModel = null)
			: base(title, icon, childs, permission, viewPartViewModel)
		{
		}
	}
	public class NavigationItem<T, W> : NavigationItem
		where T : CompositePresentationEvent<W>, new()
	{
		public W Arg { get; private set; }
		public ViewPartViewModel ViewPartViewModel { get; set; }

		public NavigationItem(string title, string icon = null, IList<NavigationItem> childs = null, PermissionType? permission = null, ViewPartViewModel viewPartViewModel = null, W arg = default(W))
			: base(title, icon, childs, permission)
		{
			ViewPartViewModel = viewPartViewModel;
			Arg = arg;
			ServiceFactory.Events.GetEvent<T>().Subscribe(x => { IsSelected = true; });
			IsSelectionAllowed = true;
		}
		public override void Execute()
		{
			if (ViewPartViewModel != null)
				ServiceFactory.Layout.Show(ViewPartViewModel);
			ServiceFactory.Events.GetEvent<T>().Publish(Arg);
		}
		public void Execute(W arg)
		{
			ServiceFactory.Events.GetEvent<T>().Publish(arg);
		}
	}
}
