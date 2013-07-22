using System.Collections.Generic;
using FiresecAPI.Models;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Microsoft.Practices.Prism.Events;

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

		public NavigationItem(ViewPartViewModel viewPartViewModel, string title, string icon = null, IList<NavigationItem> childs = null, PermissionType? permission = null, W arg = default(W), bool subscribe = true)
			: base(title, icon, childs, permission)
		{
			ViewPartViewModel = viewPartViewModel;
			Arg = arg;
			if (subscribe)
				Subscribe();
			IsSelectionAllowed = true;
		}
		public override void Execute()
		{
			ServiceFactoryBase.Events.GetEvent<T>().Publish(Arg);
		}
		public void Execute(W arg)
		{
			ServiceFactoryBase.Events.GetEvent<T>().Publish(arg);
		}

		private void Subscribe()
		{
			ServiceFactoryBase.Events.GetEvent<T>().Subscribe(ShowViewPart);
		}
		public void ShowViewPart(W arg)
		{
			IsSelected = true;
			if (ViewPartViewModel != null)
			{
				var selectable = ViewPartViewModel as ISelectable<W>;
				if (selectable != null)
					selectable.Select(arg);
				ApplicationService.Layout.Show(ViewPartViewModel);
			}
		}
	}
}
