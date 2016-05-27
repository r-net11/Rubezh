using Infrustructure.Plans.Designer;
using StrazhAPI.Plans.Elements;
using Infrustructure.Plans.Painters;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Infrustructure.Plans.Presenter
{
	public class PresenterItem : CommonDesignerItem
	{
		public event EventHandler DoubleClickEvent;

		public event EventHandler ClickEvent;

		public bool IsPoint { get; set; }

		public Func<ContextMenu> ContextMenuProvider { get; set; }

		public bool ShowBorderOnMouseOver { get; set; }

		public Cursor Cursor { get; set; }

		public PresenterItem(ElementBase element)
			: base(element)
		{
			Cursor = null;
			ShowBorderOnMouseOver = false;
			IsEnabled = true;
			IsPoint = false;
			IsVisibleLayout = true;
		}

		public void InvalidatePainter()
		{
			Painter.Invalidate();
		}

		public void OverridePainter(IPainter painter)
		{
			Painter = painter;
			Painter.Invalidate();
		}

		public void CreatePainter()
		{
			Painter = PainterFactory.Create(DesignerCanvas, Element);
			Painter.Invalidate();
		}

		public void Navigate()
		{
			//UpdateLayout();
			//RaiseEvent(new RoutedEventArgs(PresenterItem.FlushEvent));
		}

		protected override void MouseDoubleClick(Point point, MouseButtonEventArgs e)
		{
			base.MouseDoubleClick(point, e);
			if (DoubleClickEvent != null)
				DoubleClickEvent(this, e);
		}

		protected override void MouseUp(Point point, MouseButtonEventArgs e)
		{
			base.MouseUp(point, e);
			if (ClickEvent != null)
				ClickEvent(this, e);
		}

		protected override ContextMenu ContextMenuOpening()
		{
			return ContextMenuProvider == null ? null : ContextMenuProvider();
		}

		protected override void SetIsMouseOver(bool value)
		{
			base.SetIsMouseOver(value);
			DesignerCanvas.Cursor = value && Cursor != null ? Cursor : Cursors.Arrow;
		}

		public override Rect GetRectangle()
		{
			var rect = base.GetRectangle();
			if (DesignerCanvas == null || !IsPoint)
				return rect;
			return new Rect(rect.Left - DesignerCanvas.PointZoom / 2, rect.Top - DesignerCanvas.PointZoom / 2, DesignerCanvas.PointZoom, DesignerCanvas.PointZoom);
		}
	}
}