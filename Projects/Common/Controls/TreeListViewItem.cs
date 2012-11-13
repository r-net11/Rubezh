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

		void TreeListViewItem_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			TreeListViewItem item = sender as TreeListViewItem;
			if (item != null)
			{
				item.Focus();
				e.Handled = true;
			}
		}

	}
}
