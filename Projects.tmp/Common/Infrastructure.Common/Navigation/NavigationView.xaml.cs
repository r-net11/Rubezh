using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Infrastructure.Common.Windows;

namespace Infrastructure.Common.Navigation
{
	public partial class NavigationView : UserControl, INotifyPropertyChanged
	{
		private TreeViewItem _selectedItem;
		public NavigationView()
		{
			InitializeComponent();
		}

		public event PropertyChangedEventHandler PropertyChanged;
		private void OnPropertyChanged(string name)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(name));
		}

		private void TreeView_TargetUpdated(object sender, System.Windows.Data.DataTransferEventArgs e)
		{
			TreeView tv = e.TargetObject as TreeView;
			if (e.Property == TreeView.ItemsSourceProperty && tv != null)
			{
				var items = tv.ItemsSource as IList<NavigationItem>;
				if (items != null)
				{
					CheckPermissions(items);
					UpdateParent(null, items);
					SelectFirst(items);
				}
			}
		}

		private void TreeViewItem_Selected(object sender, RoutedEventArgs e)
		{
			TreeViewItem tvi = e.OriginalSource as TreeViewItem;
			if (tvi != null)
			{
				tvi.BringIntoView();
				var scope = FocusManager.GetFocusScope(tvi);
				FocusManager.SetFocusedElement(scope, tvi);
				//tvi.IsExpanded = !tvi.IsExpanded;
				NavigationItem navigation = tvi.Header as NavigationItem;
				if (navigation != null)
					navigation.Execute();
				e.Handled = true;
			}
		}
		private void TreeViewItem_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
		}
		private void TreeView_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			var item = GetTreeViewItemClicked(e);
			if (item != null)
			{
				item.MouseLeave += TreeViewItem_MouseLeave;
				var tree = (TreeView)sender;
				_selectedItem = tree.ItemContainerGenerator.ContainerFromItem(tree.SelectedItem) as TreeViewItem;
				if (_selectedItem != null)
					_selectedItem.RequestBringIntoView += TreeViewItem_RequestBringIntoView;
			}
		}
		private void TreeView_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			TreeViewItem item = GetTreeViewItemClicked(e);
			ResetSelectedItem();
			if (item != null)
			{
				NavigationItem navigation = item.Header as NavigationItem;
				if (navigation != null && navigation.IsSelectionAllowed && navigation.IsSelected && navigation.SupportMultipleSelect)
					navigation.Execute();
				item.IsExpanded = !item.IsExpanded;
				item.BringIntoView();
				e.Handled = true;
			}
		}
		private void TreeViewItem_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
		{
			e.Handled = true;
		}
		private void TreeViewItem_MouseLeave(object sender, MouseEventArgs e)
		{
			((TreeViewItem)sender).MouseLeave -= TreeViewItem_MouseLeave;
			ResetSelectedItem();
		}

		private TreeViewItem GetTreeViewItemClicked(RoutedEventArgs e)
		{
			TreeView tv = e.Source as TreeView;
			FrameworkElement item = (FrameworkElement)e.OriginalSource;
			FrameworkElement parent = item.Parent as FrameworkElement;
			TreeViewItem result = parent == null ? null : parent.TemplatedParent as TreeViewItem;
			if (result == null)
			{
				Point p = item.TranslatePoint(new Point(0, 0), tv);
				DependencyObject obj = tv.InputHitTest(p) as DependencyObject;
				while (obj != null && !(obj is TreeViewItem))
					obj = VisualTreeHelper.GetParent(obj);
				result = obj as TreeViewItem;
			}
			return result;
		}

		private void CheckPermissions(IList<NavigationItem> items)
		{
			for (int i = items.Count - 1; i >= 0; i--)
				if (!items[0].CheckPermission() || !HavePermission(items[i]))
					items.Remove(items[i]);
				else
					CheckPermissions(items[i].Childs);
		}
		private void UpdateParent(NavigationItem parent, IList<NavigationItem> items)
		{
			foreach (NavigationItem item in items)
			{
				item.Parent = parent;
				UpdateParent(item, item.Childs);
			}
		}
		private bool HavePermission(NavigationItem item)
		{
			return item.Permission == null || ApplicationService.User == null || ApplicationService.User.HasPermission(item.Permission.Value);
		}
		private bool SelectFirst(IList<NavigationItem> items)
		{
			for (int index = 0; index < items.Count; index++)
				if (items[index].IsVisible)
				{
					if (items[index].IsSelectionAllowed)
					{
						items[index].Execute();
						return true;
					}
					else if (SelectFirst(items[index].Childs))
						return true;
				}
			return false;
		}
		private void ResetSelectedItem()
		{
			if (_selectedItem != null)
			{
				_selectedItem.RequestBringIntoView -= TreeViewItem_RequestBringIntoView;
				_selectedItem = null;
			}
		}
	}
}