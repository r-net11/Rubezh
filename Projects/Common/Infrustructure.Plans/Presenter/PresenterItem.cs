using System.Windows;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Painters;
using System.Windows.Input;
using System;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Infrustructure.Plans.Presenter
{
	public class PresenterItem : CommonDesignerItem
	{
		public event EventHandler DoubleClickEvent;
		public bool IsPoint { get; set; }
		public Func<ContextMenu> ContextMenuProvider { get; set; }
		public PresenterBorder Border { get; private set; }

		public PresenterItem(ElementBase element)
			: base(element)
		{
			IsEnabled = true;
			IsPoint = false;
			IsVisibleLayout = true;
		}

		public void OverridePainter(IPainter painter)
		{
			Painter = painter;
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

		public override void UpdateZoom()
		{
			base.UpdateZoom();
			if (Border != null)
				Border.InvalidateVisual();
		}
		public override void UpdateZoomPoint()
		{
			if (IsPoint)
			{
				//Translate();
				RefreshPainter();
				if (Border != null)
					Border.InvalidateVisual();
			}
			else
				base.UpdateZoomPoint();
		}
		public override void RefreshPainter()
		{
			if (IsPoint)
			{
				//var rect = Element.GetRectangle();
				//Offset = new Vector(rect.Left, rect.Top);
			}
			else
				base.RefreshPainter();
		}

		public override Rect GetRectangle()
		{
			var rect = base.GetRectangle();
			return DesignerCanvas != null && !IsPoint ? rect : new Rect(rect.Left - DesignerCanvas.PointZoom / 2, rect.Top - DesignerCanvas.PointZoom / 2, DesignerCanvas.PointZoom, DesignerCanvas.PointZoom);
		}

		public void SetBorder(PresenterBorder border)
		{
			Border = border;
			//Children.Add(Border);
			Border.IsVisible = IsMouseOver;
		}
		protected override void SetIsMouseOver(bool value)
		{
			base.SetIsMouseOver(value);
			if (Border != null)
				Border.IsVisible = value;
		}
	}
}
