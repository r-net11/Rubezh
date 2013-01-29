using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Painters;

namespace Infrustructure.Plans.Presenter
{
	public class PresenterItem : CommonDesignerItem
	{
		public event EventHandler DoubleClickEvent;

		public bool IsPoint { get; set; }

		public Func<ContextMenu> ContextMenuProvider { get; set; }

		public bool ShowBorderOnMouseOver { get; set; }

		public PresenterItem(ElementBase element)
			: base(element)
		{
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
			Painter = PainterFactory.Create(Element);
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

		protected override ContextMenu ContextMenuOpening()
		{
			return ContextMenuProvider == null ? null : ContextMenuProvider();
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