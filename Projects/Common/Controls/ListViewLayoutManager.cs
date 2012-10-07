using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.ComponentModel;

namespace Controls
{
	public class ListViewLayoutManager
	{
		public static readonly DependencyProperty EnabledProperty = DependencyProperty.RegisterAttached("Enabled", typeof(bool), typeof(ListViewLayoutManager), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnLayoutManagerEnabledChanged)));
		public static void SetEnabled(DependencyObject dependencyObject, bool enabled)
		{
			dependencyObject.SetValue(EnabledProperty, enabled);
		}

		public static readonly DependencyProperty ColumnsProperty = DependencyProperty.RegisterAttached("Columns", typeof(GridViewColumnCollection), typeof(ListViewLayoutManager), new FrameworkPropertyMetadata());
		public static void SetColumns(DependencyObject dependencyObject, GridViewColumnCollection value)
		{
			dependencyObject.SetValue(ColumnsProperty, value);
		}
		public static GridViewColumnCollection GetColumns(DependencyObject dependencyObject)
		{
			return (GridViewColumnCollection)dependencyObject.GetValue(ColumnsProperty);
		}

		public static readonly DependencyProperty MinWidthProperty = DependencyProperty.RegisterAttached("MinWidth", typeof(double), typeof(ListViewLayoutManager), new FrameworkPropertyMetadata());
		public static double GetMinWidth(DependencyObject dependencyObject)
		{
			return (double)dependencyObject.GetValue(MinWidthProperty);
		}
		public static void SetMinWidth(DependencyObject dependencyObject, double minWidth)
		{
			dependencyObject.SetValue(MinWidthProperty, minWidth);
		}

		private static void OnLayoutManagerEnabledChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			ItemsControl view = dependencyObject as ItemsControl;
			bool enabled = (bool)e.NewValue;
			if (view != null && enabled)
				new ListViewLayoutManager(view);
		}

		private readonly ItemsControl _view;
		private bool _loaded;
		private ScrollViewer _scroller;
		private bool _resizing;

		public ListViewLayoutManager(ItemsControl view)
		{
			if (view == null)
				throw new ArgumentNullException("ListViewLayoutManager");
			_loaded = false;
			_resizing = false;
			_scroller = null;
			_view = view;
			_view.Loaded += new RoutedEventHandler(ListViewLoaded);
			_view.Unloaded += new RoutedEventHandler(ListViewUnloaded);
		}

		private GridViewColumnCollection Columns { get; set; }
		private double MinWidth { get; set; }

		private void ListViewLoaded(object sender, RoutedEventArgs e)
		{
			Columns = GetColumns(_view);
			MinWidth = GetMinWidth(_view);
			RegisterEvents();
			ResizeColumns(false);
			_loaded = true;
		}
		private void ListViewUnloaded(object sender, RoutedEventArgs e)
		{
			if (!_loaded)
				return;
			UnregisterEvents();
			_loaded = false;
		}

		private void RegisterEvents()
		{
			_scroller = FindScroller(_view);
			if (_scroller != null)
			{
				DependencyPropertyDescriptor sdpd = DependencyPropertyDescriptor.FromProperty(ScrollViewer.ViewportWidthProperty, typeof(ScrollViewer));
				if (sdpd != null)
					sdpd.AddValueChanged(_scroller, OnResizeColumns);
			}
			if (Columns != null)
			{
				DependencyPropertyDescriptor dpd = DependencyPropertyDescriptor.FromProperty(GridViewColumn.WidthProperty, typeof(GridViewColumn));
				if (dpd != null)
					foreach (var column in Columns)
						dpd.AddValueChanged(column, OnResizeColumns);
			}
		}
		private void UnregisterEvents()
		{
			if (_scroller != null)
			{
				DependencyPropertyDescriptor sdpd = DependencyPropertyDescriptor.FromProperty(ScrollViewer.ViewportWidthProperty, typeof(ScrollViewer));
				if (sdpd != null)
					sdpd.RemoveValueChanged(_scroller, OnResizeColumns);
			}
			if (Columns != null)
			{
				DependencyPropertyDescriptor dpd = DependencyPropertyDescriptor.FromProperty(GridViewColumn.WidthProperty, typeof(GridViewColumn));
				if (dpd != null)
					foreach (var column in Columns)
						dpd.RemoveValueChanged(column, OnResizeColumns);
			}
		}
		private void OnResizeColumns(object sender, EventArgs e)
		{
			ResizeColumns(sender == Columns[Columns.Count - 1]);
		}

		private ScrollViewer FindScroller(DependencyObject start)
		{
			ScrollViewer scroller = null;
			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(start) && scroller == null; i++)
			{
				Visual childVisual = VisualTreeHelper.GetChild(start, i) as Visual;
				if (childVisual is ScrollViewer)
				{
					scroller = childVisual as ScrollViewer;
					break;
				}
				scroller = FindScroller(childVisual);
			}
			return scroller;
		}
		private void ResizeColumns(bool last)
		{
			if (_resizing || Columns == null || Columns.Count == 0)
				return;
			if (_scroller != null)
			{
				ItemsPresenter presenter = _scroller.Content as ItemsPresenter;
				if (presenter != null)
					try
					{
						_resizing = true;
						double columnsWidth = 0;
						for (int i = 0; i < Columns.Count - 1; i++)
						{
							if (Columns[i].ActualWidth < MinWidth)
								Columns[i].Width = MinWidth;
							columnsWidth += Columns[i].ActualWidth;
						}
						double preWidth = presenter.ActualWidth - columnsWidth;
						GridViewColumn fillColumn = Columns[Columns.Count - 1];
						if (preWidth < MinWidth || (last && fillColumn.Width > preWidth))
						{
							if (fillColumn.ActualWidth < MinWidth)
								fillColumn.Width = MinWidth;
							_scroller.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
						}
						else
						{
							_scroller.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
							Columns[Columns.Count - 1].Width = preWidth;
						}
					}
					finally
					{
						_resizing = false;
					}
			}
		}
	}
}
