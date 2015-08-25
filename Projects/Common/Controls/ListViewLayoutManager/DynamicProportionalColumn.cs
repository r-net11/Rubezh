using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Controls.ListViewLayoutManager
{
	public sealed class DynamicProportionalColumn : LayoutColumn
	{
		public static readonly DependencyProperty WidthProperty =
			DependencyProperty.RegisterAttached(
				"Width",
				typeof(double),
				typeof(DynamicProportionalColumn));

		public static readonly DependencyProperty MinWidthProperty =
			DependencyProperty.RegisterAttached(
				"MinWidth",
				typeof(double),
				typeof(DynamicProportionalColumn));

		private DynamicProportionalColumn()
		{
		} // DynamicProportionalColumn

		// ----------------------------------------------------------------------
		public static double GetWidth(DependencyObject obj)
		{
			return (double)obj.GetValue(WidthProperty);
		} // GetWidth

		// ----------------------------------------------------------------------
		public static void SetWidth(DependencyObject obj, double width)
		{
			obj.SetValue(WidthProperty, width);
		} // SetWidth

		// ----------------------------------------------------------------------
		public static double GetMinWidth(DependencyObject obj)
		{
			return (double)obj.GetValue(MinWidthProperty);
		} // GetMinWidth

		// ----------------------------------------------------------------------
		public static void SetMinWidth(DependencyObject obj, double minWidth)
		{
			obj.SetValue(MinWidthProperty, minWidth);
		} // SetMinWidth


		// ----------------------------------------------------------------------
		public static bool IsDynamicProportionalColumn(GridViewColumn column)
		{
			if (column == null)
			{
				return false;
			}
			return HasPropertyValue(column, WidthProperty);
		} // IsProportionalColumn

		// ----------------------------------------------------------------------
		public static double? GetProportionalWidth(GridViewColumn column)
		{

			return GetColumnWidth(column, WidthProperty);
		} // GetProportionalWidth

		// ----------------------------------------------------------------------
		public static GridViewColumn ApplyWidth(GridViewColumn gridViewColumn, double width)
		{
			SetWidth(gridViewColumn, width);
			return gridViewColumn;
		} // ApplyWidth

	} // class DynamicProportionalColumn
}
