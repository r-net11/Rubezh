using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Controls
{
	public class TreeListViewItem : TreeViewItem
	{
		static TreeListViewItem()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeListViewItem), new FrameworkPropertyMetadata(typeof(TreeListViewItem)));
		}

		public TreeListViewItem()
		{
			PreviewMouseRightButtonDown += new MouseButtonEventHandler(TreeListViewItem_PreviewMouseRightButtonDown);
			MouseRightButtonDown += new MouseButtonEventHandler(TreeListViewItem_MouseRightButtonDown);
		}

		private int _level = -1;
		public int Level
		{
			get
			{
				if (_level == -1)
				{
					TreeListViewItem parent = ItemsControl.ItemsControlFromItemContainer(this) as TreeListViewItem;
					_level = parent != null ? parent.Level + 1 : 0;
				}
				return _level;
			}
		}

		protected override DependencyObject GetContainerForItemOverride()
		{
			return new TreeListViewItem();
		}
		protected override bool IsItemItsOwnContainerOverride(object item)
		{
			return item is TreeListViewItem;
		}

		private bool trace1 = true;
		private bool trace2 = false;
		private bool trace3 = true;
		void TreeListViewItem_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			TreeListViewItem item = sender as TreeListViewItem;
			if (item != null)
			{
				if (trace1)
					item.Focus();
				if (trace2)
					item.IsSelected = true;
				if (trace3)
					e.Handled = true;
			}
		}
		private void TreeListViewItem_PreviewMouseRightButtonDown(object sender, MouseEventArgs e)
		{
			//TreeListViewItem item = sender as TreeListViewItem;
			//if (item != null)
			//{
			//    if (trace1)
			//        item.Focus();
			//    if (trace2)
			//        item.IsSelected = true;
			//    if (trace3)
			//        e.Handled = true;
			//}
		}
	}
}
