using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using Infrastructure.Common.Navigation;
using System.Windows;
using System.Windows.Media;
using Infrastructure;
using FiresecAPI.Models;
using FiresecClient;
using System.Linq;
using System.Windows.Input;

namespace FireAdministrator.Views
{
	public partial class NavigationView : UserControl, INotifyPropertyChanged
	{
		public NavigationView()
		{
			InitializeComponent();
			DataContext = this;
		}

		private List<NavigationItem> _navigation;
		public List<NavigationItem> Navigation
		{
			get { return _navigation; }
			set
			{
				_navigation = value;
				CheckPermissions(_navigation);
				OnPropertyChanged("Navigation");
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		private void OnPropertyChanged(string name)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(name));
		}

		private void On_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			if (Navigation != null && Navigation.Count > -1)
				Navigation[0].IsSelected = true;
		}

		//private List<NavigationItem> TransformToList(NavigationItem parent)
		//{
		//    List<NavigationItem> result = new List<NavigationItem>();
		//    foreach (NavigationItem item in parent.Childs)
		//    {
		//        item.Parent = parent;
		//        item.Level = parent.Level + 1;
		//        result.Add(item);
		//        result.AddRange(TransformToList(item));
		//    }
		//    return result;
		//}

		private void TreeViewItem_Selected(object sender, RoutedEventArgs e)
		{
			TreeViewItem tvi = e.OriginalSource as TreeViewItem;
			if (tvi != null)
			{
				//tvi.IsExpanded = !tvi.IsExpanded;
				NavigationItem item = tvi.Header as NavigationItem;
				if (item != null)
					item.Execute();
			}
		}
		private void TreeViewItem_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
		}
		private void TreeView_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			TreeViewItem item = GetTreeViewItemClicked(e);
			if (item != null)
			{
				item.IsExpanded = !item.IsExpanded;
				e.Handled = true;
			}
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
		private bool HavePermission(NavigationItem item)
		{
			return item.Permission == null || FiresecManager.CurrentUser.Permissions.Any(x => x == item.Permission.Value);
		}
	}
}