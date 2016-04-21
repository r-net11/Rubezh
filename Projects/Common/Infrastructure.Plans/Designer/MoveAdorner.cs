using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Infrastructure.Plans.Designer
{
	public class MoveAdorner : Adorner
	{
		public bool IsMoved { get; private set; }
		private CommonDesignerCanvas _designerCanvas;
		private DrawingVisual _drawingVisual;
		private Point? _startPoint;
		private TranslateTransform _transform;
		private List<DesignerItem> _selectedItems;

		public MoveAdorner(CommonDesignerCanvas designerCanvas)
			: base(designerCanvas)
		{
			_designerCanvas = designerCanvas;
			Cursor = Cursors.SizeAll;
			_transform = new TranslateTransform();
			_drawingVisual = new DrawingVisual();
			_drawingVisual.Transform = _transform;
			AddVisualChild(_drawingVisual);
		}

		public void Show(Point point)
		{
			IsMoved = false;
			_startPoint = point;
			if (Parent == null)
			{
				AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(_designerCanvas);
				if (adornerLayer != null)
					adornerLayer.Add(this);
			}
			_selectedItems = _designerCanvas.SelectedItems.ToList();
			DrawVisual();
			if (!IsMouseCaptured)
				CaptureMouse();
		}
		public void Hide()
		{
			Completed();
			if (IsMouseCaptured)
				ReleaseMouseCapture();

			AdornerLayer adornerLayer = Parent as AdornerLayer;
			if (adornerLayer != null)
				adornerLayer.Remove(this);
		}

		protected override int VisualChildrenCount
		{
			get { return 1; }
		}
		protected override Visual GetVisualChild(int index)
		{
			return _drawingVisual;
		}

		private void DrawVisual()
		{
			_transform.X = 0;
			_transform.Y = 0;
			using (var context = _drawingVisual.RenderOpen())
				foreach (var designerItem in _selectedItems)
				{
					designerItem.IsVisibleLayout = false;
					designerItem.Painter.Draw(context);
					if (designerItem.ResizeChrome != null)
						designerItem.ResizeChrome.Render(context);
				}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				if (!IsMoved)
					_designerCanvas.BeginChange();
				IsMoved = true;
				var shift = CalculateShift(e.GetPosition(_designerCanvas));
				_transform.X = shift.X;
				_transform.Y = shift.Y;
				e.Handled = true;
			}
			else
				Hide();
		}
		protected override void OnMouseUp(MouseButtonEventArgs e)
		{
			Hide();
		}
		protected override void OnMouseLeave(MouseEventArgs e)
		{
			Hide();
		}

		private void Completed()
		{
			if (_startPoint.HasValue && IsMoved)
			{
				var shift = CalculateShift(Mouse.GetPosition(_designerCanvas));
				IsMoved = shift.X != 0 || shift.Y != 0;
				if (IsMoved)
					foreach (var designerItem in _selectedItems)
					{
						designerItem.Element.SetPosition(designerItem.Element.GetPosition() + shift);
						designerItem.IsVisibleLayout = true;
						designerItem.IsSelected = true;
						designerItem.RefreshPainter();
					}
				_designerCanvas.Refresh();
				_startPoint = null;
				_designerCanvas.EndChange();
			}
		}
		private Vector CalculateShift(Point point)
		{
			var shift = point - _startPoint.Value;
			foreach (DesignerItem designerItem in _selectedItems)
			{
				var rect = designerItem.ContentBounds;
				if (rect.Right + shift.X > _designerCanvas.CanvasWidth)
					shift.X = _designerCanvas.CanvasWidth - rect.Right;
				if (rect.Left + shift.X < 0)
					shift.X = -rect.Left;
				if (rect.Bottom + shift.Y > _designerCanvas.CanvasHeight)
					shift.Y = _designerCanvas.CanvasHeight - rect.Bottom;
				if (rect.Top + shift.Y < 0)
					shift.Y = -rect.Top;
			}
			return shift;
		}
	}
}