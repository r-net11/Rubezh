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
			//if (!Painter.RedrawOnZoom && Border != null)
			//    Border.InvalidateVisual();
		}
		public override void UpdateZoomPoint()
		{
			if (IsPoint)
			{
				Translate();
				if (Border != null)
					Border.InvalidateVisual();
			}
			else
				base.UpdateZoomPoint();
		}
		public override Rect GetRectangle()
		{
			var rect = base.GetRectangle();
			return DesignerCanvas != null && !IsPoint ? rect : new Rect(rect.Left - DesignerCanvas.PointZoom / 2, rect.Top - DesignerCanvas.PointZoom / 2, DesignerCanvas.PointZoom, DesignerCanvas.PointZoom);
		}

		public void SetBorder(PresenterBorder border)
		{
			Border = border;
			Children.Add(Border);
			Border.IsVisible = IsMouseOver;
		}
		protected override void SetIsMouseOver(bool value)
		{
			base.SetIsMouseOver(value);
			if (Border != null)
				Border.IsVisible = value;
		}

		//public static readonly RoutedEvent FlushEvent = EventManager.RegisterRoutedEvent("Flush", RoutingStrategy.Tunnel, typeof(RoutedEventHandler), typeof(PresenterItem));
		//public event RoutedEventHandler Flush
		//{
		//    add { AddHandler(FlushEvent, value); }
		//    remove { RemoveHandler(FlushEvent, value); }
		//}
		//private FrameworkElement _border;
		//public FrameworkElement Border
		//{
		//    get { return _border; }
		//    set
		//    {
		//        _border = value;
		//        //OnPropertyChanged("Border");
		//    }
		//}
		//public double AdornerThickness { get; private set; }
		//public double AdornerMargin { get; private set; }
		//public override void SetLocation()
		//{
		//    if (IsPoint)
		//    {
		//        var rect = Element.GetRectangle();
		//        //Canvas.SetLeft(this, rect.Left - _pointZoom / 2);
		//        //Canvas.SetTop(this, rect.Top - _pointZoom / 2);
		//        //ItemWidth = rect.Width + _pointZoom;
		//        //ItemHeight = rect.Height + _pointZoom;
		//    }
		//    else
		//        base.SetLocation();
		//}
	}
}
