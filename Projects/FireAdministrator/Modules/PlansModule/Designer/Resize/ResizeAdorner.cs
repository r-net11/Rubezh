using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using Infrustructure.Plans;
using Infrustructure.Plans.Designer;

namespace PlansModule.Designer.Resize
{
	public class ResizeAdorner : Adorner
	{
		private VisualCollection _visuals;
		private FrameworkElement _chrome;

		public ResizeAdorner(DesignerItem designerItem)
			: base(designerItem)
		{
			_chrome = designerItem.ResizeChrome;
			_visuals = new VisualCollection(this);
			if (_chrome != null)
			{
				_chrome.DataContext = designerItem;
				_visuals.Add(_chrome);
			}
		}

		protected override int VisualChildrenCount
		{
			get { return _visuals.Count; }
		}
		protected override Visual GetVisualChild(int index)
		{
			return _visuals[index];
		}
		protected override Size ArrangeOverride(Size arrangeBounds)
		{
			if (_chrome != null)
				_chrome.Arrange(new Rect(arrangeBounds));
			return arrangeBounds;
		}
	}
}
