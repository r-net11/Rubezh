using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Windows.Media;
using Infrastructure.Common;

namespace Controls
{
	public class TreeListView : TreeView
	{
		private const int MinColumnWidth = 20;
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

		public static readonly DependencyProperty SelectedObjectProperty = DependencyProperty.Register("SelectedObject", typeof(object), typeof(TreeListView), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, SelectedObjectChangedCallback));
		[Bindable(true)]
		public object SelectedObject
		{
			get { return GetValue(SelectedObjectProperty); }
			set { SetValue(SelectedObjectProperty, value); }
		}

		static TreeListView()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeListView), new FrameworkPropertyMetadata(typeof(TreeListView)));
		}
		public TreeListView()
		{
			//PreviewMouseDoubleClick += (sender, e) => e.Handled = true;
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
		protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
		{
			base.OnItemsChanged(e);
		}


		protected override void OnSelectedItemChanged(RoutedPropertyChangedEventArgs<object> e)
		{
			var item = e.NewValue as TreeListViewItem;
			SelectedObject = item != null && item.Header != null && item.Header.GetType().FullName == "MS.Internal.NamedObject" ? null : e.NewValue;
			base.OnSelectedItemChanged(e);
		}

		private static void SelectedObjectChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			TreeListView treeView = (TreeListView)obj;
			if (!ReferenceEquals(treeView.SelectedItem, e.NewValue))
			{
				var item = e.NewValue as TreeItemViewModel;
				if (item != null)
					item.IsSelected = true;
			}
		}
	}
}
