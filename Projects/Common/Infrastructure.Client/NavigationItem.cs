using System.Collections.Generic;
using RubezhAPI.Models;
using RubezhClient;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans.Events;
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

			if(permission.HasValue)
				IsVisible = ClientManager.CheckPermission(permission.Value);
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
		public virtual void ShowViewPart(W arg)
		{
			IsSelected = true;
			if (ViewPartViewModel != null)
			{
				SelectItem(arg);
				ApplicationService.Layout.Show(ViewPartViewModel);
			}
		}
		protected virtual void SelectItem(W arg)
		{
			TrySelectItem(arg);
		}
		protected bool TrySelectItem<TT>(TT arg)
		{
			var selectable = ViewPartViewModel as ISelectable<TT>;
			if (selectable == null)
				return false;
			selectable.Select(arg);
			return true;
		}
	}
	public class NavigationItemEx<T, W> : NavigationItem<T, ShowOnPlanArgs<W>>
		where T : CompositePresentationEvent<ShowOnPlanArgs<W>>, new()
	{
		public NavigationItemEx(ViewPartViewModel viewPartViewModel, string title, string icon = null, IList<NavigationItem> childs = null, PermissionType? permission = null, W arg = default(W), bool subscribe = true)
			: base(viewPartViewModel, title, icon, childs, permission, arg, subscribe)
		{
		}
		public NavigationItemEx(ViewPartViewModel viewPartViewModel, string title, string icon = null, IList<NavigationItem> childs = null, PermissionType? permission = null, ShowOnPlanArgs<W> arg = default(ShowOnPlanArgs<W>), bool subscribe = true)
			: base(viewPartViewModel, title, icon, childs, permission, arg, subscribe)
		{
		}
		public override void ShowViewPart(ShowOnPlanArgs<W> arg)
		{
			if (ViewPartViewModel != null && arg.RightPanelVisible.HasValue)
			{
				ViewPartViewModel.IsRightPanelVisible = arg.RightPanelVisible.Value;
				if (ViewPartViewModel.IsActive)
					ApplicationService.Shell.RightPanelVisible = arg.RightPanelVisible.Value;
				if (arg.PlanUID.HasValue)
					ServiceFactoryBase.Events.GetEvent<SelectPlanEvent>().Publish(arg.PlanUID.Value);
			}
			base.ShowViewPart(arg);
		}
		protected override void SelectItem(ShowOnPlanArgs<W> arg)
		{
			if (!TrySelectItem(arg))
				TrySelectItem(arg.Value);
		}
	}
}