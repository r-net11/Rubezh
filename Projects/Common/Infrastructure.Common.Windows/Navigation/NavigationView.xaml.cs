using Infrastructure.Common.Windows;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Infrastructure.Common.Navigation
{
	public partial class NavigationView : UserControl, INotifyPropertyChanged
	{
		TreeViewItem _selectedItem;
		public NavigationView()
		{
			InitializeComponent();
		}

		public event PropertyChangedEventHandler PropertyChanged;

		void OnPropertyChanged(string name)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(name));
		}

		void TreeView_TargetUpdated(object sender, System.Windows.Data.DataTransferEventArgs e)
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

		void TreeViewItem_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
		}

		void TreeView_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			TreeViewItem item = GetTreeViewItemClicked(e);
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

		TreeViewItem GetTreeViewItemClicked(RoutedEventArgs e)
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

		void CheckPermissions(IList<NavigationItem> items)
		{
			for (int i = items.Count - 1; i >= 0; i--)
				if (!items[0].CheckPermission() || !HavePermission(items[i]))
					items.Remove(items[i]);
				else
					CheckPermissions(items[i].Childs);
		}

		void UpdateParent(NavigationItem parent, IList<NavigationItem> items)
		{
			foreach (NavigationItem item in items)
			{
				item.Parent = parent;
				UpdateParent(item, item.Childs);
			}
		}

		bool HavePermission(NavigationItem item)
		{
			return item.Permission == null || ApplicationService.User == null || ApplicationService.User.HasPermission(item.Permission.Value);
		}

		bool SelectFirst(IList<NavigationItem> items)
		{
			for (int index = 0; index < items.Count; index++)
				if (items[index].IsVisible)
				{
					if (items[index].IsSelectionAllowed)
					{
						items[index].Execute();
						return true;
					}
					if (SelectFirst(items[index].Childs))
						return true;
				}
			return false;
		}
	}
}