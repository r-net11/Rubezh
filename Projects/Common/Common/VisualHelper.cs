using System.Windows;
using System.Windows.Media;

namespace Common
{
	public static class VisualHelper
	{
		public static TChildItem FindVisualChild<TChildItem>(DependencyObject obj) 
			where TChildItem : DependencyObject
		{
			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
			{
				var child = VisualTreeHelper.GetChild(obj, i);
				if (child != null && child is TChildItem)
					return (TChildItem)child;
				var childOfChild = FindVisualChild<TChildItem>(child);
				if (childOfChild != null)
					return childOfChild;
			}
			return null;
		}
	}
}
