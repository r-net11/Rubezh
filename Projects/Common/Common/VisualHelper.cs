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

		public static T GetParent<T>(DependencyObject obj, int level = 1)
			where T : DependencyObject
		{
			while (obj != null)
			{
				T parent = obj as T;
				if (parent != null)
				{
					if (level == 1)
						return parent;
					else
						level--;
				}
				obj = VisualTreeHelper.GetParent(obj);
			}
			return null;
		}
		public static DependencyObject GetRoot(DependencyObject obj)
		{
			var parent = VisualTreeHelper.GetParent(obj);
			while (parent != null)
			{
				obj = parent;
				parent = VisualTreeHelper.GetParent(obj);
			}
			return obj;
		}
	}
}
