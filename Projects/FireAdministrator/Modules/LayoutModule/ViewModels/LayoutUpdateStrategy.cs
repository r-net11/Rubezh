using Xceed.Wpf.AvalonDock.Layout;

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
			var parent = anchorableShown.Parent as LayoutDocumentPane;
			if (parent == null)
				parent = anchorableShown.FindParent<LayoutDocumentPane>();
			if (parent != null)
			{
			}
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