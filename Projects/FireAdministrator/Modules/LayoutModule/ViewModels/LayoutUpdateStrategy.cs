using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock;
using System.Reflection;
using System.Windows.Input;
using System.Windows.Controls;

namespace LayoutModule.ViewModels
{
	class LayoutUpdateStrategy : ILayoutUpdateStrategy
	{
		#region ILayoutUpdateStrategy Members

		public void AfterInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableShown)
		{
		}

		public void AfterInsertDocument(LayoutRoot layout, LayoutDocument anchorableShown)
		{
			//anchorableShown.Float();
			//var point = Mouse.GetPosition(layout.Manager);
			//point = layout.Manager.PointToScreen(point);
			//anchorableShown.FloatingWidth = 100;
			//anchorableShown.FloatingHeight = 100;
			//var method = typeof(DockingManager).GetMethod("StartDraggingFloatingWindowForContent", BindingFlags.NonPublic | BindingFlags.Instance);
			//method.Invoke(layout.Manager, new object[] { anchorableShown, true });
			//anchorableShown.FloatingLeft = point.X - 20;
			//anchorableShown.FloatingTop = point.Y - 20;
		}

		public bool BeforeInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableToShow, ILayoutContainer destinationContainer)
		{
			return false;
		}

		public bool BeforeInsertDocument(LayoutRoot layout, LayoutDocument anchorableToShow, ILayoutContainer destinationContainer)
		{
			return false;
		}

		#endregion
	}
}
