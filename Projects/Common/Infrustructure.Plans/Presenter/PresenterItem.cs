using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Painters;
using System.Windows;
using System.Windows.Controls;

namespace Infrustructure.Plans.Presenter
{
	public class PresenterItem : CommonDesignerItem
	{
		private double _zoom;
		private double _pointZoom;

		public static readonly RoutedEvent FlushEvent = EventManager.RegisterRoutedEvent("Flush", RoutingStrategy.Tunnel, typeof(RoutedEventHandler), typeof(PresenterItem));
		public event RoutedEventHandler Flush
		{
			add { AddHandler(FlushEvent, value); }
			remove { RemoveHandler(FlushEvent, value); }
		}

		public double AdornerThickness { get; private set; }
		public double AdornerMargin { get; private set; }
		public bool IsPoint { get; set; }

		public PresenterItem(ElementBase element)
			: base(element)
		{
			IsVisibleLayout = true;
			IsSelectableLayout = false;
			IsPoint = false;
		}

		private FrameworkElement _border;
		public FrameworkElement Border
		{
			get { return _border; }
			set
			{
				_border = value;
				OnPropertyChanged("Border");
			}
		}

		public void OverridePainter(IPainter painter)
		{
			Painter = painter;
		}

		protected override void CreateContextMenu()
		{
		}

		public void Navigate()
		{
			RaiseEvent(new RoutedEventArgs(PresenterItem.FlushEvent));
		}

		private void UpdateZoom(double zoom)
		{
			_zoom = zoom;
			AdornerThickness = 3 / zoom;
			AdornerMargin = -AdornerThickness;
			OnPropertyChanged("AdornerThickness");
			OnPropertyChanged("AdornerMargin");
		}
		public void UpdateDeviceZoom(double zoom, double pointZoom)
		{
			UpdateZoom(zoom);
			_pointZoom = pointZoom;
			SetLocation();
		}
		public override void SetLocation()
		{
			if (IsPoint)
			{
				var rect = Element.GetRectangle();
				Canvas.SetLeft(this, rect.Left - _pointZoom / 2);
				Canvas.SetTop(this, rect.Top - _pointZoom / 2);
				ItemWidth = rect.Width + _pointZoom;
				ItemHeight = rect.Height + _pointZoom;
			}
			else
				base.SetLocation();
		}
	}
}
