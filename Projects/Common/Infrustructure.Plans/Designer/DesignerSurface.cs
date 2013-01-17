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
		private IVisualItem _visualItemOver;
		private bool _isDragging;
		private Point _previousPosition;

		public DesignerSurface()
		{
			_isDragging = false;
			_visuals = new List<CommonDesignerItem>();
			ToolTipService.SetIsEnabled(this, false);
			IsVisibleChanged += (s, e) => Update(IsVisible);
		}

		internal IEnumerable<CommonDesignerItem> Items
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

		internal void AddDesignerItem(CommonDesignerItem visual)
		{
			_visuals.Add(visual);
			AddVisualChild(visual);
			AddLogicalChild(visual);
		}
		internal void DeleteDesignerItem(CommonDesignerItem visual)
		{
			_visuals.Remove(visual);
			RemoveVisualChild(visual);
			RemoveLogicalChild(visual);
		}
		internal void UpdateZIndex()
		{
			_visuals.ForEach(item => RemoveVisualChild(item));
			_visuals.Sort((item1, item2) => item1.Element.ZLayer == item2.Element.ZLayer ? item1.Element.ZIndex - item2.Element.ZIndex : item1.Element.ZLayer - item2.Element.ZLayer);
			_visuals.ForEach(item => AddVisualChild(item));
		}
		internal void Update(bool isActive)
		{
			if (_visualItemOver != null)
			{
				var point = Mouse.PrimaryDevice.GetPosition(this);
				_visualItemOver.SetIsMouseOver(false, point);
				_visualItemOver = null;
			}
			if (isActive)
				OnMouseMove(new MouseEventArgs(Mouse.PrimaryDevice, 0));
		}

		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			var point = e.GetPosition(this);
			var visualItem = GetDesignerItem(point);
			if (visualItem != null)
			{
				visualItem.OnMouseDown(point, e);
				if (e.ClickCount == 2)
					visualItem.OnMouseDoubleClick(point, e);
				if (!_isDragging)
				{
					CaptureMouse();
					_previousPosition = point;
					_isDragging = true;
					visualItem.DragStarted(point);
				}
				e.Handled = true;
			}
		}
		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
		{
			var point = e.GetPosition(this);
			ReleaseMouseCapture();
			if (_isDragging)
			{
				e.Handled = true;
				_isDragging = false;
				if (_visualItemOver != null && _visualItemOver.IsEnabled)
					_visualItemOver.DragCompleted(point);
			}
			var visualItem = GetDesignerItem(point);
			if (visualItem != null)
				visualItem.OnMouseUp(point, e);
		}
		protected override void OnMouseMove(MouseEventArgs e)
		{
			Point point = e.GetPosition(this);
			if (_visualItemOver != null && _visualItemOver.IsEnabled && _isDragging)
			{
				if (e.MouseDevice.LeftButton == MouseButtonState.Pressed)
				{
					if (_previousPosition != point)
					{
						_visualItemOver.DragDelta(point, new Vector(point.X - _previousPosition.X, point.Y - _previousPosition.Y));
						_previousPosition = point;
					}
				}
				else
				{
					_isDragging = false;
					_visualItemOver.DragCompleted(point);
				}
			}
			else if (_visualItemOver == null || !_visualItemOver.IsBusy)
			{
				var visualItem = GetDesignerItem(point);
				if (_visualItemOver != null && visualItem != _visualItemOver)
					_visualItemOver.SetIsMouseOver(false, point);
				if (_visualItemOver != visualItem)
				{
					_visualItemOver = visualItem;
					if (_visualItemOver != null)
						_visualItemOver.SetIsMouseOver(true, point);
				}
			}
			if (_visualItemOver != null)
				_visualItemOver.OnMouseMove(point, e);
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
			ContextMenu = _visualItemOver == null || !_visualItemOver.IsEnabled ? null : _visualItemOver.ContextMenuOpening();
		}

		private IVisualItem _visualItem;
		private IVisualItem GetDesignerItem(Point point)
		{
			_visualItem = null;
			PointHitTestParameters parameters = new PointHitTestParameters(point);
			VisualTreeHelper.HitTest(this, HitTestFilter, HitTestCallback, parameters);
			return _visualItem;
		}
		private HitTestResultBehavior HitTestCallback(HitTestResult result)
		{
			_visualItem = result.VisualHit as IVisualItem;
			return _visualItem == null ? HitTestResultBehavior.Continue : HitTestResultBehavior.Stop;
		}
		private HitTestFilterBehavior HitTestFilter(DependencyObject d)
		{
			if (d == this)
				return HitTestFilterBehavior.ContinueSkipSelf;
			var visualItem = d as IVisualItem;
			return visualItem != null && visualItem.IsEnabled ? HitTestFilterBehavior.Continue : HitTestFilterBehavior.ContinueSkipSelfAndChildren;
		}

		protected override void OnRender(DrawingContext dc)
		{
			//base.OnRender(dc);
			dc.DrawRectangle(Background, null, new Rect(0, 0, RenderSize.Width, RenderSize.Height));
			//foreach (var item in _visuals)
			//    item.Render(dc);
		}
	}
}
