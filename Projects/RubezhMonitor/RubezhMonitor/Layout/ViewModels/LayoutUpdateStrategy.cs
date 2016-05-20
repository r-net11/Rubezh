using Xceed.Wpf.AvalonDock.Layout;

namespace RubezhMonitor.Layout.ViewModels
{
	class LayoutUpdateStrategy : ILayoutUpdateStrategy
	{
		#region ILayoutUpdateStrategy Members

		public void AfterInsertDocument(LayoutRoot layout, LayoutDocument anchorableShown)
		{
		}
		public bool BeforeInsertDocument(LayoutRoot layout, LayoutDocument anchorableToShow, ILayoutContainer destinationContainer)
		{
			return false;
		}

		public void AfterInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableShown)
		{
		}
		public bool BeforeInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableToShow, ILayoutContainer destinationContainer)
		{
			return false;
		}

		#endregion
	}
}