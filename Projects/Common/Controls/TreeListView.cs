using System.Windows;
using System.Windows.Controls;

namespace Controls
{
	public class TreeListView : TreeView
	{
		public static readonly DependencyProperty IsHeaderVisibleProperty = DependencyProperty.Register("IsHeaderVisible", typeof(bool), typeof(TreeListView), new FrameworkPropertyMetadata(true));
		public virtual bool IsHeaderVisible
		{
			get { return (bool)GetValue(IsHeaderVisibleProperty); }
			set { SetValue(IsHeaderVisibleProperty, value); }
		}

		public static readonly DependencyProperty RowHeightProperty = DependencyProperty.Register("RowHeight", typeof(double), typeof(TreeListView), new FrameworkPropertyMetadata(double.NaN));
		public virtual double RowHeight
		{
			get { return (double)GetValue(RowHeightProperty); }
			set { SetValue(RowHeightProperty, value); }
		}

		static TreeListView()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeListView), new FrameworkPropertyMetadata(typeof(TreeListView)));
		}

		private GridViewColumnCollection _columns;
		public GridViewColumnCollection Columns
		{
			get
			{
				if (_columns == null)
					_columns = new GridViewColumnCollection();
				return _columns;
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
		protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			base.OnItemsChanged(e);
		}
	}
}
