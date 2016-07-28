using Infrastructure.Common.TreeList;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace Controls
{
	public class TreeListView : TreeView
	{
		public static readonly DependencyProperty IsHeaderVisibleProperty = DependencyProperty
			.Register("IsHeaderVisible", typeof(bool), typeof(TreeListView), new FrameworkPropertyMetadata(true));
		public static readonly DependencyProperty RowHeightProperty = DependencyProperty
			.Register("RowHeight", typeof(double), typeof(TreeListView), new FrameworkPropertyMetadata(double.NaN));
		public static readonly DependencyProperty SelectedObjectProperty = DependencyProperty
			.Register("SelectedObject", typeof(object), typeof(TreeListView), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, SelectedObjectChangedCallback));

		public virtual bool IsHeaderVisible
		{
			get { return (bool)GetValue(IsHeaderVisibleProperty); }
			set { SetValue(IsHeaderVisibleProperty, value); }
		}

		public virtual double RowHeight
		{
			get { return (double)GetValue(RowHeightProperty); }
			set { SetValue(RowHeightProperty, value); }
		}

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

		private GridViewColumnCollection _columns;

		public GridViewColumnCollection Columns
		{
			get { return _columns ?? (_columns = new GridViewColumnCollection()); }
		}

		protected override DependencyObject GetContainerForItemOverride()
		{
			return new TreeListViewItem();
		}

		protected override bool IsItemItsOwnContainerOverride(object item)
		{
			return item is TreeListViewItem;
		}

		protected override void OnSelectedItemChanged(RoutedPropertyChangedEventArgs<object> e)
		{
			var item = e.NewValue as TreeListViewItem;
			if (item == null || item.Header == null || item.Header.GetType().FullName != "MS.Internal.NamedObject")
				SelectedObject = e.NewValue;
			e.Handled = true;
			base.OnSelectedItemChanged(e);
		}

		private static void SelectedObjectChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			var treeView = (TreeListView)obj;

			if (ReferenceEquals(treeView.SelectedItem, e.NewValue)) return;

			var item = e.NewValue as TreeItemViewModel;

			if (item == null) return;

			var treeViewItem = treeView.BringTreeViewItemIntoView(item);

			if (treeViewItem != null)
				treeViewItem.IsSelected = true;
		}

		private TreeViewItem BringTreeViewItemIntoView(TreeItemViewModel item)
		{
			if (item == null)
				return null;

			var parentContainer = (ItemsControl)BringTreeViewItemIntoView(item.TreeParent) ?? this;

			return BringItemIntoView(parentContainer, item);
		}

		private TreeViewItem BringItemIntoView(ItemsControl container, object item)
		{
			var element = container.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;

			if (element != null)
				element.BringIntoView();
			else if (container.Items.Contains(item))
			{
				var vsp = (VirtualizingStackPanel)typeof(ItemsControl).InvokeMember("_itemsHost", BindingFlags.Instance | BindingFlags.GetField | BindingFlags.NonPublic, null, container, null);
				if (vsp == null)
				{
					container.UpdateLayout();
					vsp = (VirtualizingStackPanel)typeof(ItemsControl).InvokeMember("_itemsHost", BindingFlags.Instance | BindingFlags.GetField | BindingFlags.NonPublic, null, container, null);
				}
				if (vsp != null)
				{
					var index = container.Items.IndexOf(item);
					if (index >= 0)
					{
						vsp.GetType().GetMethod("BringIndexIntoView", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(vsp, new object[] { index });
						element = container.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
					}
				}
				else
					Debug.WriteLine("VirtualizingStackPanel NOT FOUND!!!");
			}
			return element;
		}
	}
}