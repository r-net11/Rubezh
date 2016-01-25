using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Controls
{
	public class GridSplitterFix
	{
		public static readonly DependencyProperty AttachProperty = DependencyProperty.RegisterAttached("Attach", typeof(bool), typeof(GridSplitterFix), new PropertyMetadata(false, OnAttachPropertyChanged));

		[AttachedPropertyBrowsableForChildrenAttribute(IncludeDescendants = false)]
		[AttachedPropertyBrowsableForType(typeof(GridSplitter))]
		public static bool GetAttach(DependencyObject obj)
		{
			return (bool)obj.GetValue(AttachProperty);
		}
		public static void SetAttach(DependencyObject obj, bool value)
		{
			obj.SetValue(AttachProperty, value);
		}

		private static void OnAttachPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			var splitter = dependencyObject as GridSplitter;
			if (splitter != null)
			{
				var grid = splitter.Parent as Grid;
				if (grid != null)
				{
					if ((bool)e.NewValue)
						grid.SizeChanged += grid_SizeChanged;
					else
						grid.SizeChanged -= grid_SizeChanged;
				}
			}
		}

		private static void grid_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			var grid = (Grid)sender;
			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(grid); i++)
			{
				var splitter = VisualTreeHelper.GetChild(grid, i) as GridSplitter;
				if (splitter != null && GetAttach(splitter))
					Fix(grid, splitter);
			}
		}
		private static void Fix(Grid grid, GridSplitter splitter)
		{
			var isColumn = splitter.HorizontalAlignment != HorizontalAlignment.Stretch;
			if (!isColumn && splitter.VerticalAlignment == VerticalAlignment.Stretch)
				isColumn = splitter.ActualWidth < splitter.ActualHeight;
			if (isColumn)
				FixColumn(grid, splitter);
			else
				FixRow(grid, splitter);
		}
		private static void FixColumn(Grid grid, GridSplitter splitter)
		{
			var index = Grid.GetColumn(splitter);
			if (index > 0 && index < grid.ColumnDefinitions.Count)
			{
				var space = grid.ColumnDefinitions[index - 1].ActualWidth + grid.ColumnDefinitions[index + 1].ActualWidth;
				if (grid.ColumnDefinitions[index - 1].Width.IsStar && grid.ColumnDefinitions[index + 1].Width.IsAbsolute)
				{
					space -= grid.ColumnDefinitions[index - 1].MinWidth;
					grid.ColumnDefinitions[index + 1].MaxWidth = space > 1 ? space - 1 : 0;
				}
				else if (grid.ColumnDefinitions[index - 1].Width.IsAbsolute && grid.ColumnDefinitions[index + 1].Width.IsStar)
				{
					space -= grid.ColumnDefinitions[index + 1].MinWidth;
					grid.ColumnDefinitions[index - 1].MaxWidth = space > 1 ? space - 1 : 0;
				}
			}
		}
		private static void FixRow(Grid grid, GridSplitter splitter)
		{
			var index = Grid.GetRow(splitter);
			if (index > 0 && index < grid.RowDefinitions.Count)
			{
				var space = grid.RowDefinitions[index - 1].ActualHeight + grid.RowDefinitions[index + 1].ActualHeight;
				if (grid.RowDefinitions[index - 1].Height.IsStar && grid.RowDefinitions[index + 1].Height.IsAbsolute)
				{
					space -= grid.RowDefinitions[index - 1].MinHeight;
					grid.RowDefinitions[index + 1].MaxHeight = space > 1 ? space - 1 : 0;
				}
				else if (grid.RowDefinitions[index - 1].Height.IsAbsolute && grid.RowDefinitions[index + 1].Height.IsStar)
				{
					space -= grid.RowDefinitions[index + 1].MinHeight;
					grid.RowDefinitions[index - 1].MaxHeight = space > 1 ? space - 1 : 0;
				}
			}
		}
	}
}
