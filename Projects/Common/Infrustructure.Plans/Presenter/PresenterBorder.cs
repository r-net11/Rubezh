using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrustructure.Plans.Designer;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;

namespace Infrustructure.Plans.Presenter
{
	public class PresenterBorder : DrawingVisual
	{
		protected static Brush BorderBrush { get; private set; }
		protected double Thickness { get { return 3 / PresenterItem.DesignerCanvas.Zoom; } }
		protected double ResizeMargin { get { return 3 / PresenterItem.DesignerCanvas.Zoom; } }
		private bool _isVisualValid;
		private bool _isVisible;
		protected PresenterItem PresenterItem { get; private set; }
		public bool IsVisible
		{
			get { return _isVisible; }
			set
			{
				_isVisible = value;
				if (IsVisible && !_isVisualValid && !PresenterItem.ContentBounds.IsEmpty)
					Redraw();
				Opacity = IsVisible ? 1 : 0;
			}
		}

		static PresenterBorder()
		{
			BorderBrush = new SolidColorBrush(Colors.Orange);
			BorderBrush.Freeze();
		}
		public PresenterBorder(PresenterItem presenterItem)
		{
			PresenterItem = presenterItem;
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
		protected void Render(DrawingContext drawingContext)
		{
			drawingContext.DrawRectangle(null, new Pen(BorderBrush, Thickness), GetBounds());
		}

		protected Rect GetBounds()
		{
			var rect = PresenterItem.ContentBounds;
			return new Rect(rect.Left - ResizeMargin, rect.Top - ResizeMargin, rect.Width + 2 * ResizeMargin, rect.Height + 2 * ResizeMargin);
		}
	}
}
