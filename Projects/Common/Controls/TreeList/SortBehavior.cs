using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Infrastructure.Common.TreeList;

namespace Controls.TreeList
{
	public static class SortBehavior
	{
		public static readonly DependencyProperty CanUserSortColumnsProperty = DependencyProperty.RegisterAttached("CanUserSortColumns", typeof(bool), typeof(SortBehavior), new FrameworkPropertyMetadata(OnCanUserSortColumnsChanged));
		public static readonly DependencyProperty CanUseSortProperty = DependencyProperty.RegisterAttached("CanUseSort", typeof(bool), typeof(SortBehavior), new FrameworkPropertyMetadata(true));
		public static readonly DependencyProperty SortDirectionProperty = DependencyProperty.RegisterAttached("SortDirection", typeof(ListSortDirection?), typeof(SortBehavior));
		public static readonly DependencyProperty SortComparerProperty = DependencyProperty.RegisterAttached("SortComparer", typeof(IItemComparer), typeof(SortBehavior));

		[AttachedPropertyBrowsableForType(typeof(ListView))]
		public static bool GetCanUserSortColumns(ListView element)
		{
			return (bool)element.GetValue(CanUserSortColumnsProperty);
		}
		[AttachedPropertyBrowsableForType(typeof(ListView))]
		public static void SetCanUserSortColumns(ListView element, bool value)
		{
			element.SetValue(CanUserSortColumnsProperty, value);
		}

		[AttachedPropertyBrowsableForType(typeof(GridViewColumn))]
		public static bool GetCanUseSort(GridViewColumn element)
		{
			return (bool)element.GetValue(CanUseSortProperty);
		}
		[AttachedPropertyBrowsableForType(typeof(GridViewColumn))]
		public static void SetCanUseSort(GridViewColumn element, bool value)
		{
			element.SetValue(CanUseSortProperty, value);
		}

		[AttachedPropertyBrowsableForType(typeof(GridViewColumn))]
		public static ListSortDirection? GetSortDirection(GridViewColumn element)
		{
			return (ListSortDirection?)element.GetValue(SortDirectionProperty);
		}
		[AttachedPropertyBrowsableForType(typeof(GridViewColumn))]
		public static void SetSortDirection(GridViewColumn element, ListSortDirection? value)
		{
			element.SetValue(SortDirectionProperty, value);
		}

		[AttachedPropertyBrowsableForType(typeof(GridViewColumn))]
		public static IItemComparer GetSortComparer(GridViewColumn element)
		{
			return (IItemComparer)element.GetValue(SortComparerProperty);
		}
		[AttachedPropertyBrowsableForType(typeof(GridViewColumn))]
		public static void SetSortComparer(GridViewColumn element, string value)
		{
			element.SetValue(SortComparerProperty, value);
		}

		private static void OnCanUserSortColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var listView = (ListView)d;
			if ((bool)e.NewValue)
			{
				listView.AddHandler(GridViewColumnHeader.ClickEvent, (RoutedEventHandler)OnColumnHeaderClick);
				if (listView.IsLoaded)
					DoInitialSort(listView);
				else
					listView.Loaded += OnLoaded;
			}
			else
				listView.RemoveHandler(GridViewColumnHeader.ClickEvent, (RoutedEventHandler)OnColumnHeaderClick);
		}

		private static void OnLoaded(object sender, RoutedEventArgs e)
		{
			var listView = (ListView)e.Source;
			listView.Loaded -= OnLoaded;
			DoInitialSort(listView);
		}

		private static void DoInitialSort(ListView listView)
		{
			var gridView = (GridView)listView.View;
			var column = gridView.Columns.FirstOrDefault(c => GetSortDirection(c) != null);
			if (column != null)
				DoSort(listView, column);
		}

		private static void OnColumnHeaderClick(object sender, RoutedEventArgs e)
		{
			var columnHeader = e.OriginalSource as GridViewColumnHeader;
			if (columnHeader != null && GetCanUseSort(columnHeader.Column))
				DoSort((ListView)e.Source, columnHeader.Column);
		}

		private static void DoSort(ListView listView, GridViewColumn column)
		{
			var tree = (TreeList)listView;
			if (tree.Root.SortColumn != null)
				SetSortDirection(tree.Root.SortColumn, null);
			var comparer = GetSortComparer(column);
			if (comparer != null)
			{
				tree.Root.RunSort(column, comparer);
				SetSortDirection(column, tree.Root.SortDirection);
			}
		}
	}
}
