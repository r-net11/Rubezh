using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Controls
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

		public static IEnumerable<T> FindVisualChildren<T>(this DependencyObject depObj) where T : DependencyObject
		{
			if (depObj != null)
				for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
				{
					DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
					if (child != null && child is T)
						yield return (T)child;
					foreach (T childOfChild in FindVisualChildren<T>(child))
						yield return childOfChild;
				}
		}
		public static IEnumerable<T> FindLogicalChildren<T>(this DependencyObject depObj) where T : DependencyObject
		{
			if (depObj != null)
				foreach (DependencyObject child in LogicalTreeHelper.GetChildren(depObj).OfType<DependencyObject>())
				{
					if (child != null && child is T)
						yield return (T)child;
					foreach (T childOfChild in FindLogicalChildren<T>(child))
						yield return childOfChild;
				}
		}

		public static DependencyObject FindVisualTreeRoot(this DependencyObject initial)
		{
			DependencyObject current = initial;
			DependencyObject result = initial;
			while (current != null)
			{
				result = current;
				current = current is Visual || current is Visual3D ? VisualTreeHelper.GetParent(current) : LogicalTreeHelper.GetParent(current);
			}
			return result;
		}

		public static T FindVisualAncestor<T>(this DependencyObject dependencyObject) where T : class
		{
			DependencyObject target = dependencyObject;
			do
			{
				target = VisualTreeHelper.GetParent(target);
			}
			while (target != null && !(target is T));
			return target as T;
		}
		public static T FindLogicalAncestor<T>(this DependencyObject dependencyObject) where T : class
		{
			DependencyObject target = dependencyObject;
			do
			{
				var current = target;
				target = LogicalTreeHelper.GetParent(target);
				if (target == null)
					target = VisualTreeHelper.GetParent(current);

			}
			while (target != null && !(target is T));
			return target as T;
		}
	}
}
