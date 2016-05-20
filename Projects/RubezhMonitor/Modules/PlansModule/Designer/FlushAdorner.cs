using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;
using Infrastructure.Plans.Presenter;
using PlansModule.ViewModels;

namespace PlansModule.Designer
{
	public class FlushAdorner : Adorner
	{
		private const int Length = 2;
		private VisualCollection _visuals;
		private DispatcherTimer _timer;
		private ContentPresenter _contentPresenter;
		private FlushViewModel _flushControl;
		protected Decorator Canvas { get; private set; }

		protected override int VisualChildrenCount
		{
			get { return _visuals.Count; }
		}
		protected override Size ArrangeOverride(Size arrangeBounds)
		{
			_contentPresenter.Arrange(new Rect(arrangeBounds));
			return arrangeBounds;
		}
		protected override Visual GetVisualChild(int index)
		{
			return _visuals[index];
		}

		public FlushAdorner(Decorator canvas)
			: base(canvas)
		{
			Canvas = canvas;
			_flushControl = new FlushViewModel();
			_contentPresenter = new ContentPresenter();
			_contentPresenter.Content = _flushControl;
			_visuals = new VisualCollection(this);
			_visuals.Add(_contentPresenter);
			_timer = new DispatcherTimer();
			_timer.Interval = TimeSpan.FromSeconds(Length);
			_timer.Tick += (s, e) => Hide();
		}

		public void Show(PresenterItem presenterItem)
		{
			if (_timer.IsEnabled || Parent != null)
				Hide();
			_flushControl.SetPresenterItem(presenterItem);
			_flushControl.Show();
			AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(Canvas);
			if (adornerLayer != null)
				adornerLayer.Add(this);
			_timer.Start();
		}
		public void Hide()
		{
			_timer.Stop();
			_flushControl.Hide();
			AdornerLayer adornerLayer = Parent as AdornerLayer;
			if (adornerLayer != null)
				adornerLayer.Remove(this);
		}

		public void UpdateDeviceZoom(double zoom, double pointZoom)
		{
			_flushControl.UpdateDeviceZoom(zoom, pointZoom);
		}
	}
}
