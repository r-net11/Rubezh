using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Common;
using Infrastructure.Common.Windows.Ribbon;

namespace Controls.Ribbon.Views
{
	public class RibbonPopup : Popup
	{
		public RibbonPopup()
		{
		}

		protected override void OnOpened(EventArgs e)
		{
			base.OnOpened(e);
			Child.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
		}
		protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
		{
			base.OnLostKeyboardFocus(e);
			if (IsKeyboardFocusWithin)
				return;
			if (IsOpen)
				Child.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			if (e.Handled)
				return;
			if (e.Key == Key.Escape)
			{
				Close();
				e.Handled = true;
			}
			else
			{
				var ribbonMenuItem = e.OriginalSource as RibbonMenuItemView;
				if (ribbonMenuItem != null)
					switch (e.Key)
					{
						case Key.Left:
							if (ribbonMenuItem != null)
							{
								if (ribbonMenuItem.IsSelected)
									ribbonMenuItem.IsSelected = false;
								else
								{
									var ribbonMenu = VisualHelper.GetParent<RibbonMenuView>(ribbonMenuItem, 2);
									if (ribbonMenu != null)
									{
										ribbonMenuItem = (RibbonMenuItemView)ribbonMenu.ItemContainerGenerator.ContainerFromItem(ribbonMenu.SelectedItem);
										if (ribbonMenuItem != null)
											ribbonMenuItem.Focus();
									}
									else
										Close();
								}
								e.Handled = true;
							}
							break;
						case Key.Right:
							if (ribbonMenuItem != null && !ribbonMenuItem.IsSelected)
							{
								ribbonMenuItem.IsSelected = true;
								e.Handled = true;
							}
							break;
						case Key.Up:
							if (MoveFocus(ribbonMenuItem, false))
								e.Handled = true;
							break;
						case Key.Down:
							if (MoveFocus(ribbonMenuItem, true))
								e.Handled = false;
							break;
						case Key.Space:
							ribbonMenuItem.IsSelected = !ribbonMenuItem.IsSelected;
							if (!ribbonMenuItem.IsSelected)
							{
								var ribbonMenu = VisualHelper.GetParent<RibbonMenuView>(ribbonMenuItem);
								if (ribbonMenu != null)
								{
									var viewModel = (RibbonMenuItemViewModel)ribbonMenu.ItemContainerGenerator.ItemFromContainer(ribbonMenuItem);
									if (viewModel != null && viewModel.Command != null && viewModel.Command.CanExecute(viewModel.CommandParameter))
									{
										viewModel.Command.Execute(viewModel.CommandParameter);
										Close();
									}
								}
							}
							e.Handled = true;
							break;
					}
			}
		}
		private static bool MoveFocus(RibbonMenuItemView menuItem, bool down)
		{
			var ribbonMenu = VisualHelper.GetParent<RibbonMenuView>(menuItem);
			var index = ribbonMenu.ItemContainerGenerator.IndexFromContainer(menuItem);
			var newIndex = -1;
			while (newIndex == -1)
			{
				index = (index + (down ? 1 : -1) + ribbonMenu.Items.Count) % ribbonMenu.Items.Count;
				var container = (RibbonMenuItemView)ribbonMenu.ItemContainerGenerator.ContainerFromIndex(index);
				if (container.Visibility == Visibility.Visible)
					newIndex = index;
			}
			if (newIndex != -1)
			{
				var container = (RibbonMenuItemView)ribbonMenu.ItemContainerGenerator.ContainerFromIndex(newIndex);
				if (container != null)
				{
					container.Focus();
					ribbonMenu.SelectedItem = null;
					return true;
				}
			}
			return false;
		}

		public void Close()
		{
			SetValue(IsOpenProperty, false);
		}
	}
}
