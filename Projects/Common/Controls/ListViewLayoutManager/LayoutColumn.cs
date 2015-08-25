using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Controls.ListViewLayoutManager
{
	public abstract class LayoutColumn
	{
		protected static bool HasPropertyValue(GridViewColumn column, DependencyProperty dp)
		{
			if (column == null)
			{
				throw new ArgumentNullException("column");
			}
			object value = column.ReadLocalValue(dp);
			if (value != null && value.GetType() == dp.PropertyType)
			{
				return true;
			}

			return false;
		}

		protected static double? GetColumnWidth(GridViewColumn column, DependencyProperty dp)
		{
			if (column == null)
			{
				throw new ArgumentNullException("column");
			}
			object value = column.ReadLocalValue(dp);
			if (value != null && value.GetType() == dp.PropertyType)
			{
				return (double)value;
			}

			return null;
		}
	}
}
