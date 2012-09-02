using System.Collections.Generic;
using FiresecAPI.Models;
using Infrastructure.Common.Navigation;
using Microsoft.Practices.Prism.Events;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;
using Infrastructure.Common.Windows;

namespace Infrastructure.Client
{
	public class NavigationItem<T> : NavigationItem<T, object>
	where T : CompositePresentationEvent<object>, new()
	{
		public NavigationItem(ViewPartViewModel viewPartViewModel, string title, string icon = null, IList<NavigationItem> childs = null, PermissionType? permission = null)
			: base(viewPartViewModel, title, icon, childs, permission)
		{
		}
	}
	public class NavigationItem<T, W> : NavigationItem
		where T : CompositePresentationEvent<W>, new()
	{
		public W Arg { get; private set; }
		public ViewPartViewModel ViewPartViewModel { get; set; }

		public NavigationItem(ViewPartViewModel viewPartViewModel, string title, string icon = null, IList<NavigationItem> childs = null, PermissionType? permission = null, W arg = default(W))
			: base(title, icon, childs, permission)
		{
			ViewPartViewModel = viewPartViewModel;
			Arg = arg;
			ServiceFactoryBase.Events.GetEvent<T>().Subscribe(x => { IsSelected = true; });
			IsSelectionAllowed = true;
		}
		public override void Execute()
		{
			if (ViewPartViewModel != null)
			{
				var selectable = ViewPartViewModel as ISelectable<W>;
				if (selectable != null)
					selectable.Select(Arg);
				ApplicationService.Layout.Show(ViewPartViewModel);
			}
			ServiceFactoryBase.Events.GetEvent<T>().Publish(Arg);
		}
		public void Execute(W arg)
		{
			ServiceFactoryBase.Events.GetEvent<T>().Publish(arg);
		}
	}
}
