using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Infrustructure.Plans.Designer
{
	public class DesignerSurface : Panel
	{
		private List<CommonDesignerItem> _visuals;
		private CommonDesignerItem _designerItemOver;

		public DesignerSurface()
		{
			_visuals = new List<CommonDesignerItem>();
			ToolTipService.SetIsEnabled(this, false);
		}

		public IEnumerable<CommonDesignerItem> Items
		{
			get { return _visuals; }
		}
		protected override int VisualChildrenCount
		{
			get { return _visuals.Count; }
		}
		protected override Visual GetVisualChild(int index)
		{
			return _visuals[index];
		}

		public void AddDesignerItem(CommonDesignerItem visual)
		{
			_visuals.Add(visual);
			AddVisualChild(visual);
			AddLogicalChild(visual);
		}
		public void DeleteDesignerItem(CommonDesignerItem visual)
		{
			_visuals.Remove(visual);
			RemoveVisualChild(visual);
			RemoveLogicalChild(visual);
		}
		public void UpdateZIndex()
		{
			_visuals.ForEach(item => RemoveVisualChild(item));
			_visuals.Sort((item1, item2) => item1.Element.ZLayer == item2.Element.ZLayer ? item1.Element.ZIndex - item2.Element.ZIndex : item1.Element.ZLayer - item2.Element.ZLayer);
			_visuals.ForEach(item => AddVisualChild(item));
		}

		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			var commonDesignerItem = GetDesignerItem(e.GetPosition(this));
			if (commonDesignerItem != null)
			{
				commonDesignerItem.OnMouseDown(e);
				if (e.ClickCount == 2)
					commonDesignerItem.OnMouseDoubleClick(e);
				e.Handled = true;
			}
		}
		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
		{
			var commonDesignerItem = GetDesignerItem(e.GetPosition(this));
			if (commonDesignerItem != null)
				commonDesignerItem.OnMouseUp(e);
		}
		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (_designerItemOver == null || !_designerItemOver.IsBusy)
			{
				var commonDesignerItem = GetDesignerItem(e.GetPosition(this));
				if (_designerItemOver != null && commonDesignerItem != _designerItemOver)
					_designerItemOver.SetIsMouseOver(false);
				if (_designerItemOver != commonDesignerItem)
				{
					_designerItemOver = commonDesignerItem;
					if (_designerItemOver != null)
						_designerItemOver.SetIsMouseOver(true);
				}
			}
			if (_designerItemOver != null)
				_designerItemOver.OnMouseMove(e);
		}
		protected override void OnMouseEnter(MouseEventArgs e)
		{
			OnMouseMove(e);
		}
		protected override void OnMouseLeave(MouseEventArgs e)
		{
			OnMouseMove(e);
		}
		protected override void OnContextMenuOpening(ContextMenuEventArgs e)
		{
			base.OnContextMenuOpening(e);
			ContextMenu = _designerItemOver == null || !_designerItemOver.IsVisibleLayout ? null : _designerItemOver.ContextMenuOpening();
		}

		private CommonDesignerItem _hitCommonDesignerItem;
		private CommonDesignerItem GetDesignerItem(Point point)
		{
			_hitCommonDesignerItem = null;
			PointHitTestParameters parameters = new PointHitTestParameters(point);
			VisualTreeHelper.HitTest(this, HitTestFilter, HitTestCallback, parameters);
			return _hitCommonDesignerItem;
		}
		private HitTestResultBehavior HitTestCallback(HitTestResult result)
		{
			_hitCommonDesignerItem = result.VisualHit as CommonDesignerItem;
			return _hitCommonDesignerItem == null ? HitTestResultBehavior.Continue : HitTestResultBehavior.Stop;
		}
		private HitTestFilterBehavior HitTestFilter(DependencyObject d)
		{
			if (d == this)
				return HitTestFilterBehavior.ContinueSkipSelf;
			var designerItem = d as CommonDesignerItem;
			return designerItem == null || !designerItem.IsEnabled ? HitTestFilterBehavior.ContinueSkipSelfAndChildren : HitTestFilterBehavior.ContinueSkipChildren;
		}
	}
}
