using Common;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Infrastructure.Plans.Designer
{
	public class DesignerSurface : Canvas
	{
		private List<CommonDesignerItem> _visuals;
		private IVisualItem _visualItemOver;
		private bool _isDragging;
		private bool _isZIndexValid;
		private Point _previousPosition;
		private CommonDesignerCanvas _designerCanvas;
		public Brush BackgroundBrush { get; set; }
		public Pen Border { get; set; }

		public DesignerSurface(CommonDesignerCanvas designerCanvas)
		{
			_designerCanvas = designerCanvas;
			_isDragging = false;
			_visuals = new List<CommonDesignerItem>();
			_isZIndexValid = true;
			ToolTipService.SetIsEnabled(this, false);
			IsVisibleChanged += (s, e) => Update(IsVisible);
		}

		internal IEnumerable<CommonDesignerItem> Items
		{
			get { return _visuals; }
		}
		internal void AddDesignerItem(CommonDesignerItem visual)
		{
			if (visual.WPFControl != null)
			{
				Children.Add(visual.WPFControl);
			}

			_visuals.Add(visual);
			_isZIndexValid = false;
		}
		internal void DeleteDesignerItem(CommonDesignerItem visual)
		{
			if (visual.IsMouseOver)
				((IVisualItem)visual).SetIsMouseOver(false, new Point());
			_visuals.Remove(visual);
		}
		internal void ClearDesignerItems()
		{
			if (_visualItemOver != null)
				_visualItemOver.SetIsMouseOver(false, new Point());
			_visuals.Clear();
			Children.Clear();
		}
		internal void UpdateZIndex()
		{
			_visuals.Sort((item1, item2) => item1.Element.ZLayer == item2.Element.ZLayer ? item1.Element.ZIndex - item2.Element.ZIndex : item1.Element.ZLayer - item2.Element.ZLayer);
			_isZIndexValid = true;
			InvalidateVisual();
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
			var visualItem = GetVisualItem(point);
			if (visualItem != null)
			{
				visualItem.OnMouseDown(point, e);
				if (e.ClickCount == 2)
					visualItem.OnMouseDoubleClick(point, e);
				else if (!_isDragging)
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
			if (_isDragging)
			{
				e.Handled = true;
				_isDragging = false;
				if (_visualItemOver != null)
					_visualItemOver.DragCompleted(point);
			}
			ReleaseMouseCapture();
			var visualItem = GetVisualItem(point);
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
				var visualItem = GetVisualItem(point);
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
			Update(true);
		}
		protected override void OnMouseLeave(MouseEventArgs e)
		{
			Update(false);
		}
		protected override void OnContextMenuOpening(ContextMenuEventArgs e)
		{
			base.OnContextMenuOpening(e);
			ContextMenu = _visualItemOver == null || !_visualItemOver.IsEnabled ? null : _visualItemOver.ContextMenuOpening();
		}

		private IVisualItem GetVisualItem(Point point)
		{
			for (int i = _visuals.Count - 1; i >= 0; i--)
				if (_visuals[i].IsEnabled)
				{
					var visualItem = _visuals[i].HitTest(point);
					if (visualItem != null)
						return visualItem;
				}
			return null;
		}

		protected override void OnRender(DrawingContext dc)
		{
			using (new TimeCounter("=Surface.Render: {0}"))
			{
				if (!_isZIndexValid)
					UpdateZIndex();
				//base.OnRender(dc);
				_designerCanvas.RenderBackground(dc);
				var thickness = Border == null ? 0 : Border.Thickness;
				dc.DrawRectangle(BackgroundBrush, Border, new Rect(-thickness / 2, -thickness / 2, RenderSize.Width + thickness, RenderSize.Height + thickness));
				foreach (var item in _visuals)
					//if (item.IsVisibleLayout)
					item.Render(dc);
				_designerCanvas.RenderForeground(dc);
			}
		}
	}
}