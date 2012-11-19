using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Painters;
using System.Windows;

namespace Infrustructure.Plans.Presenter
{
	public class PresenterItem : CommonDesignerItem
	{
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

		public void UpdateZoom(double zoom)
		{
			AdornerThickness = 3 / zoom;
			AdornerMargin = -AdornerThickness;
			OnPropertyChanged("AdornerThickness");
			OnPropertyChanged("AdornerMargin");
		}
	}
}
