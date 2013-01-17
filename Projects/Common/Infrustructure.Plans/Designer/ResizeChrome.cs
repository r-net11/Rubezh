using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.Generic;

namespace Infrustructure.Plans.Designer
{
	public abstract class ResizeChrome : DrawingVisual, IVisualItem
	{
		protected const ToleranceType ToleranceType = System.Windows.Media.ToleranceType.Relative;
		protected double Tolerance { get { return 0.1 / DesignerCanvas.Zoom / DesignerItem.Transform.Value.M11; } }
		protected static Brush TransparentBrush { get; private set; }
		protected static Brush BorderBrush { get; private set; }
		protected static Brush ThumbBrush { get; private set; }
		protected double ResizeThumbSize { get { return 3.5 / DesignerCanvas.Zoom / DesignerItem.Transform.Value.M11; } }
		protected double ResizeMargin { get { return 4 / DesignerCanvas.Zoom / DesignerItem.Transform.Value.M11; } }
		protected double Thickness { get { return 1 / DesignerCanvas.Zoom / DesignerItem.Transform.Value.M11; } }
		private bool _isVisualValid;
		private bool _isVisible;
		private bool _canResize;
		private ResizeDirection _resizeDirection;
		protected bool IsMoved { get; set; }

		static ResizeChrome()
		{
			TransparentBrush = Brushes.Transparent;
			TransparentBrush.Freeze();
			var borderBrush = new LinearGradientBrush()
			{
				Opacity = 0.7,
				StartPoint = new Point(0, 0),
				EndPoint = new Point(1, 0.3),

			};
			borderBrush.GradientStops.Add(new GradientStop(Colors.SlateGray, 0));
			borderBrush.GradientStops.Add(new GradientStop(Colors.LightGray, 0.5));
			borderBrush.GradientStops.Add(new GradientStop(Colors.SlateGray, 1));
			borderBrush.Freeze();
			BorderBrush = borderBrush;
			var thumbBrush = new RadialGradientBrush()
			{
				Center = new Point(0.3, 0.3),
				GradientOrigin = new Point(0.3, 0.3),
				RadiusX = 0.7,
				RadiusY = 0.7,
			};
			thumbBrush.GradientStops.Add(new GradientStop(Colors.White, 0));
			thumbBrush.GradientStops.Add(new GradientStop(Colors.DarkSlateGray, 0.9));
			thumbBrush.Freeze();
			ThumbBrush = thumbBrush;
		}

		protected DesignerItem DesignerItem { get; private set; }
		protected CommonDesignerCanvas DesignerCanvas
		{
			get { return DesignerItem.DesignerCanvas; }
		}
		public bool IsVisible
		{
			get { return _isVisible; }
			set
			{
				_isVisible = value;
				if (IsVisible && !_isVisualValid)
					Redraw();
				Opacity = IsVisible ? 1 : 0;
			}
		}

		public ResizeChrome(DesignerItem designerItem)
		{
			_canResize = false;
			_isVisualValid = false;
			_isVisible = false;
			DesignerItem = designerItem;
		}

		public void InvalidateVisual()
		{
			_isVisualValid = false;
			if (IsVisible)
				Redraw();
		}
		public void Redraw()
		{
			using (DrawingContext drawingContext = RenderOpen())
				Render(drawingContext);
			_isVisualValid = true;
		}
		protected abstract void Render(DrawingContext drawingContext);
		protected void DrawBounds(DrawingContext drawingContext)
		{
			drawingContext.DrawRectangle(null, new Pen(TransparentBrush, 3 * Thickness), GetBounds());
			drawingContext.DrawRectangle(null, new Pen(BorderBrush, Thickness), GetBounds());
			_canResize = false;
		}
		protected void DrawSizableBounds(DrawingContext drawingContext)
		{
			DrawBounds(drawingContext);
			var rect = GetBounds();
			DrawThumb(drawingContext, rect.TopLeft);
			DrawThumb(drawingContext, rect.TopRight);
			DrawThumb(drawingContext, rect.BottomRight);
			DrawThumb(drawingContext, rect.BottomLeft);
			_canResize = true;
		}
		protected void DrawThumb(DrawingContext drawingContext, Point location)
		{
			drawingContext.DrawEllipse(ThumbBrush, null, location, ResizeThumbSize, ResizeThumbSize);
		}

		protected Rect GetBounds()
		{
			var rect = DesignerItem.ContentBounds;
			return new Rect(rect.Left - ResizeMargin, rect.Top - ResizeMargin, rect.Width + 2 * ResizeMargin, rect.Height + 2 * ResizeMargin);
		}
		protected virtual Cursor GetCursor(Point point)
		{
			var cursor = Cursors.Arrow;
			if (_canResize)
			{
				cursor = Cursors.Pen;
				var rect = DesignerItem.Transform.TransformBounds(GetBounds());
				if (IsInsideThumb(rect.TopLeft, point))
					cursor = Cursors.SizeNWSE;
				else if (IsInsideThumb(rect.TopRight, point))
					cursor = Cursors.SizeNESW;
				else if (IsInsideThumb(rect.BottomRight, point))
					cursor = Cursors.SizeNWSE;
				else if (IsInsideThumb(rect.BottomLeft, point))
					cursor = Cursors.SizeNESW;
				else if (IsInsideRect(new Rect(rect.Left, rect.Top - 2 * Thickness, rect.Width, 4 * Thickness), point))
					cursor = Cursors.SizeNS;
				else if (IsInsideRect(new Rect(rect.Left, rect.Bottom - 2 * Thickness, rect.Width, 4 * Thickness), point))
					cursor = Cursors.SizeNS;
				else if (IsInsideRect(new Rect(rect.Left - 2 * Thickness, rect.Top, 4 * Thickness, rect.Height), point))
					cursor = Cursors.SizeWE;
				else if (IsInsideRect(new Rect(rect.Right - 2 * Thickness, rect.Top, 4 * Thickness, rect.Height), point))
					cursor = Cursors.SizeWE;
			}
			return cursor;
		}
		protected bool IsInsideRect(Rect rect, Point point)
		{
			return rect.Contains(point);
			//return IsInsideGeometry(new RectangleGeometry(rect), point);
		}
		protected bool IsInsideThumb(Point location, Point point)
		{
			return IsInsideGeometry(new EllipseGeometry(location, ResizeThumbSize, ResizeThumbSize), point);
		}
		protected bool IsInsideGeometry(Geometry geometry, Point point)
		{
			return geometry.FillContains(point, Tolerance, ToleranceType);
		}

		#region IVisualItem Members

		bool IVisualItem.AllowDrag
		{
			get { return _canResize; }
		}
		bool IVisualItem.IsEnabled
		{
			get { return IsVisible; }
		}
		bool IVisualItem.IsBusy
		{
			get { return false; }
		}

		void IVisualItem.SetIsMouseOver(bool isMouseOver, Point point)
		{
			var cursor = Cursors.Arrow;
			if (isMouseOver)
				cursor = GetCursor(point);
			DesignerCanvas.Cursor = cursor;
		}
		ContextMenu IVisualItem.ContextMenuOpening()
		{
			return null;
		}

		void IVisualItem.OnMouseDown(Point point, MouseButtonEventArgs e)
		{
		}
		void IVisualItem.OnMouseUp(Point point, MouseButtonEventArgs e)
		{
		}
		void IVisualItem.OnMouseMove(Point point, MouseEventArgs e)
		{
			if (!IsMoved)
				DesignerCanvas.Cursor = GetCursor(point);
		}
		void IVisualItem.OnMouseDoubleClick(Point point, MouseButtonEventArgs e)
		{
		}

		void IVisualItem.DragStarted(Point point)
		{
			UpdateResizeDirection(point);
		}
		void IVisualItem.DragCompleted(Point point)
		{
			_resizeDirection = ResizeDirection.None;
			if (IsMoved)
				DesignerCanvas.EndChange();
			IsMoved = false;
			DesignerCanvas.Cursor = GetCursor(point);
		}
		void IVisualItem.DragDelta(Point point, Vector shift)
		{
			if (DesignerItem.IsSelected)
			{
				DesignerCanvas.BeginChange();
				if (_resizeDirection != ResizeDirection.None)
				{
					var vector = CalculateSize(_resizeDirection, shift);
					foreach (DesignerItem designerItem in DesignerCanvas.SelectedItems)
						if (designerItem.ResizeChrome != null)
							designerItem.ResizeChrome.Resize(_resizeDirection, vector);
				}
				else
					foreach (DesignerItem designerItem in DesignerCanvas.SelectedItems)
						if (designerItem.ResizeChrome != null)
							designerItem.ResizeChrome.DragDeltaInner(point, shift);
				IsMoved = true;
			}
		}

		#endregion

		protected virtual void DragDeltaInner(Point point, Vector shift)
		{
		}
		protected abstract void Resize(ResizeDirection direction, Vector vector);
		private Vector CalculateSize(ResizeDirection direction, Vector change)
		{
			double dragDeltaHorizontal = change.X;
			double dragDeltaVertical = change.Y;
			foreach (DesignerItem designerItem in DesignerCanvas.SelectedItems)
			{
				Rect rect = designerItem.GetRectangle();
				if ((direction & ResizeDirection.Top) == ResizeDirection.Top)
				{
					if (rect.Height - dragDeltaVertical < DesignerItem.MinHeight)
						dragDeltaVertical = rect.Height - DesignerItem.MinHeight;
					if (rect.Top + dragDeltaVertical < 0)
						dragDeltaVertical = -rect.Top;
				}
				else if ((direction & ResizeDirection.Bottom) == ResizeDirection.Bottom)
				{
					if (rect.Height + dragDeltaVertical < DesignerItem.MinHeight)
						dragDeltaVertical = DesignerItem.MinHeight - rect.Height;
					if (rect.Bottom + dragDeltaVertical > DesignerCanvas.CanvasHeight)
						dragDeltaVertical = DesignerCanvas.CanvasHeight - rect.Bottom;
				}
				if ((direction & ResizeDirection.Left) == ResizeDirection.Left)
				{
					if (rect.Width - dragDeltaHorizontal < DesignerItem.MinWidth)
						dragDeltaHorizontal = rect.Width - DesignerItem.MinWidth;
					if (rect.Left + dragDeltaHorizontal < 0)
						dragDeltaHorizontal = -rect.Left;
				}
				else if ((direction & ResizeDirection.Right) == ResizeDirection.Right)
				{
					if (rect.Width + dragDeltaHorizontal < DesignerItem.MinWidth)
						dragDeltaHorizontal = DesignerItem.MinWidth - rect.Width;
					if (rect.Right + dragDeltaHorizontal > DesignerCanvas.CanvasWidth)
						dragDeltaHorizontal = DesignerCanvas.CanvasWidth - rect.Right;
				}
			}
			return new Vector(dragDeltaHorizontal, dragDeltaVertical);
		}
		private void UpdateResizeDirection(Point point)
		{
			_resizeDirection = ResizeDirection.None;
			if (_canResize)
			{
				var rect = DesignerItem.Transform.TransformBounds(GetBounds());
				if (IsInsideThumb(rect.TopLeft, point))
					_resizeDirection = ResizeDirection.TopLeft;
				else if (IsInsideThumb(rect.TopRight, point))
					_resizeDirection = ResizeDirection.TopRight;
				else if (IsInsideThumb(rect.BottomRight, point))
					_resizeDirection = ResizeDirection.BottomRight;
				else if (IsInsideThumb(rect.BottomLeft, point))
					_resizeDirection = ResizeDirection.BottomLeft;
				else if (IsInsideRect(new Rect(rect.Left, rect.Top - 2 * Thickness, rect.Width, 4 * Thickness), point))
					_resizeDirection = ResizeDirection.Top;
				else if (IsInsideRect(new Rect(rect.Left, rect.Bottom - 2 * Thickness, rect.Width, 4 * Thickness), point))
					_resizeDirection = ResizeDirection.Bottom;
				else if (IsInsideRect(new Rect(rect.Left - 2 * Thickness, rect.Top, 4 * Thickness, rect.Height), point))
					_resizeDirection = ResizeDirection.Left;
				else if (IsInsideRect(new Rect(rect.Right - 2 * Thickness, rect.Top, 4 * Thickness, rect.Height), point))
					_resizeDirection = ResizeDirection.Right;
			}
		}
	}
}
