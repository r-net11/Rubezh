using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Infrustructure.Plans.Presenter;
using PlansModule.ViewModels;
using System.Windows.Shapes;

namespace PlansModule.Designer
{
	public class FlushAdorner : Adorner
	{
		private const int Length = 2;
		private VisualCollection _visuals;
		private DispatcherTimer _timer;
		private ContentPresenter _contentPresenter;
		private FlushViewModel _flushControl;
		protected Canvas Canvas { get; private set; }

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

		public FlushAdorner(Canvas canvas)
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
			AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(Canvas);
			if (adornerLayer != null)
				adornerLayer.Add(this);
			_timer.Start();
		}
		public void Hide()
		{
			_timer.Stop();
			AdornerLayer adornerLayer = Parent as AdornerLayer;
			if (adornerLayer != null)
				adornerLayer.Remove(this);
		}

		public void UpdateZoom()
		{
		}
	}
}
