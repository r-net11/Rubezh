using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Infrastructure.Common;
using System.Windows.Media;
using System.Windows.Threading;
using System;
using System.Reflection;

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
			if (item == null || item.Header == null || item.Header.GetType().FullName != "MS.Internal.NamedObject")
				SelectedObject = e.NewValue;
			e.Handled = true;
			base.OnSelectedItemChanged(e);
		}

		private static void SelectedObjectChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			TreeListView treeView = (TreeListView)obj;
			if (!ReferenceEquals(treeView.SelectedItem, e.NewValue))
			{
				var item = e.NewValue as TreeItemViewModel;
				if (item != null)
				{
					treeView.BringTreeViewItemIntoView(item);
					item.IsSelected = true;
				}
			}
		}

		public void Virt(int itemIndex_)
		{
			VirtualizingStackPanel vsp = (VirtualizingStackPanel)typeof(ItemsControl).InvokeMember("_itemsHost", BindingFlags.Instance | BindingFlags.GetField | BindingFlags.NonPublic, null, this, null);
			double scrollHeight = vsp.ScrollOwner.ScrollableHeight;
			double offset = scrollHeight * itemIndex_ / this.Items.Count; // itemIndex_ is index of the item which we want to show in the middle of the view
			vsp.SetVerticalOffset(offset);
		}

	}
	public class MyVirtualizingStackPanel : VirtualizingStackPanel
	{
		public void BringIntoView(int index) { BringIndexIntoView(index); }
	}
	public static class TreeViewitemFinder
	{
		public static TreeViewItem BringTreeViewItemIntoView(this TreeView treeView, TreeItemViewModel item)
		{
			if (item == null)
				return null;
			ItemsControl parentContainer = (ItemsControl)treeView.BringTreeViewItemIntoView(item.TreeParent) ?? treeView;
			return parentContainer.BringItemIntoView(item);
		}

		private static TreeViewItem BringItemIntoView(this ItemsControl container, object item)
		{
			var vsp = container.FindVisualChild<VirtualizingStackPanel>();
			if (vsp == null)
			{
				var treeViewItem = (TreeViewItem)container.ItemContainerGenerator.ContainerFromItem(item);
				treeViewItem.BringIntoView();
				return treeViewItem;
			}
			//Use exposed BringIntoView method to render each of the items in order
			for (int i = 0; i < container.Items.Count; i++)
			{
				vsp.Dispatcher.Invoke(DispatcherPriority.ContextIdle, (Action<VirtualizingStackPanel, int>)BringIntoView, vsp, i);
				var nextitem = (TreeViewItem)container.ItemContainerGenerator.ContainerFromIndex(i);
				if (nextitem.DataContext == item)
				{
					nextitem.Dispatcher.Invoke(DispatcherPriority.ContextIdle, (Action)nextitem.BringIntoView);
					return nextitem;
				}
			}
			return null;
		}
		private static void BringIntoView(VirtualizingStackPanel vsp, int index)
		{
			vsp.GetType().GetMethod("BringIndexIntoView", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(vsp, new object[] { index });
		}

		private static T FindVisualChild<T>(this Visual visual) where T : Visual
		{
			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(visual); i++)
			{
				var child = (Visual)VisualTreeHelper.GetChild(visual, i);
				if (child != null)
				{
					var correctlyTyped = child as T;
					if (correctlyTyped != null)
						return correctlyTyped;
					var descendent = FindVisualChild<T>(child);
					if (descendent != null)
						return descendent;
				}
			}
			return null;
		}
	}

}
