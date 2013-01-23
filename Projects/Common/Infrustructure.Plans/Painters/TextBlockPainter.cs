﻿using System.Globalization;
using System.Windows;
using System.Windows.Media;
using Infrustructure.Plans.Elements;

namespace Infrustructure.Plans.Painters
{
	public class TextBlockPainter : RectanglePainter
	{
		private GeometryDrawing _textDrawing;
		private RectangleGeometry _clipGeometry;
		private ScaleTransform _scaleTransform;

		public TextBlockPainter(ElementBase element)
			: base(element)
		{
		}

		//private const int Margin = 3;
		protected override void InnerDraw(DrawingContext drawingContext)
		{
			base.InnerDraw(drawingContext);
			if (_scaleTransform != null)
				drawingContext.PushTransform(_scaleTransform);
			if (_clipGeometry != null)
				drawingContext.PushClip(_clipGeometry);
			drawingContext.DrawDrawing(_textDrawing);
			if (_clipGeometry != null)
				drawingContext.Pop();
			if (_scaleTransform != null)
				drawingContext.Pop();
		}
		public override void Transform()
		{
			base.Transform();
			IElementTextBlock elementText = (IElementTextBlock)Element;
			Rect bound = new Rect(Rect.Left + Element.BorderThickness / 2, Rect.Top + Element.BorderThickness / 2, Rect.Width - Element.BorderThickness, Rect.Height - Element.BorderThickness);
			var typeface = new Typeface(new FontFamily(elementText.FontFamilyName), elementText.FontItalic ? FontStyles.Italic : FontStyles.Normal, elementText.FontBold ? FontWeights.Bold : FontWeights.Normal, new FontStretch());
			var formattedText = new FormattedText(elementText.Text, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, typeface, elementText.FontSize, PainterCache.BlackBrush);
			formattedText.TextAlignment = (TextAlignment)elementText.TextAlignment;
			Point point = bound.TopLeft;
			switch (formattedText.TextAlignment)
			{
				case TextAlignment.Right:
					point = bound.TopRight;
					break;
				case TextAlignment.Center:
					point = new Point(bound.Left + bound.Width / 2, bound.Top);
					break;
			}
			if (_clipGeometry != null)
				_clipGeometry.Rect = new Rect(Rect.Left + Element.BorderThickness / 2, Rect.Top + Element.BorderThickness / 2, Rect.Width - Element.BorderThickness, Rect.Height - Element.BorderThickness);
			if (_scaleTransform != null)
			{
				_scaleTransform.CenterX = point.X;
				_scaleTransform.CenterY = point.Y;
				_scaleTransform.ScaleX = bound.Width / formattedText.Width;
				_scaleTransform.ScaleY = bound.Height / formattedText.Height;
			}
			_textDrawing.Geometry = formattedText.BuildGeometry(point);
		}
		public override void Invalidate()
		{
			IElementTextBlock elementText = (IElementTextBlock)Element;
			_textDrawing = new GeometryDrawing(PainterCache.GetBrush(elementText.ForegroundColor), null, null);
			if (elementText.Stretch)
			{
				_clipGeometry = null;
				_scaleTransform = new ScaleTransform();
			}
			else
			{
				_scaleTransform = null;
				_clipGeometry = new RectangleGeometry();
			}
			base.Invalidate();
		}
	}
}
