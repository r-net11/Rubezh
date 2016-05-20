using Infrastructure.Plans.Designer;
using RubezhAPI.Plans.Elements;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Linq;
using System.Windows.Controls;
using Controls.Converters;

namespace Infrastructure.Plans.Painters
{
	public class TextBoxPainter : RectanglePainter
	{
		GeometryDrawing _textDrawing;
		RectangleGeometry _clipGeometry;
		ScaleTransform _scaleTransform;

		public TextBoxPainter(CommonDesignerCanvas designerCanvas, ElementBase element)
			: base(designerCanvas, element)
		{
			_textDrawing = new GeometryDrawing(null, null, null);
		}

		protected override void InnerDraw(DrawingContext drawingContext)
		{
			base.InnerDraw(drawingContext);
			if (_scaleTransform != null)
			{
				drawingContext.PushTransform(_scaleTransform);
			}
			if (_clipGeometry != null)
			{
				drawingContext.PushClip(_clipGeometry);
			}
			drawingContext.DrawDrawing(_textDrawing);
			if (_clipGeometry != null)
			{
				drawingContext.Pop();
			}
			if (_scaleTransform != null)
			{
				drawingContext.Pop();
			}
		}

		public override void Transform()
		{
			base.Transform();
			var elementText = (IElementTextBlock)Element;
			var height = Rect.Height > Element.BorderThickness ? Rect.Height - Element.BorderThickness : 0;
			var width = Rect.Width > Element.BorderThickness ? Rect.Width - Element.BorderThickness : 0;
			Rect bound = new Rect(Rect.Left + Element.BorderThickness / 2, Rect.Top + Element.BorderThickness / 2, width, height);
			var typeface = new Typeface(new FontFamily(elementText.FontFamilyName), elementText.FontItalic ? FontStyles.Italic : FontStyles.Normal, elementText.FontBold ? FontWeights.Bold : FontWeights.Normal, new FontStretch());
			var formattedText = new FormattedText(elementText.Text, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, typeface, elementText.FontSize, PainterCache.BlackBrush);
			formattedText.TextAlignment = (TextAlignment)elementText.TextAlignment;
			Point point = bound.TopLeft;
			if (!elementText.WordWrap || _scaleTransform != null)
			{
				switch (formattedText.TextAlignment)
				{
					case TextAlignment.Right:
						point = bound.TopRight;
						break;
					case TextAlignment.Center:
						point = new Point(bound.Left + bound.Width / 2, bound.Top);
						break;
				}
			}
			if (_clipGeometry != null)
			{
				_clipGeometry.Rect = bound;
				if (elementText.WordWrap)
				{
					formattedText.MaxTextWidth = bound.Width;
					formattedText.MaxTextHeight = bound.Height;
				}
				var valign = (VerticalAlignment)elementText.VerticalAlignment;
				switch (valign)
				{
					case VerticalAlignment.Center:
						point.Y = bound.Top + (bound.Height - formattedText.Height) / 2;
						break;
					case VerticalAlignment.Bottom:
						point.Y = bound.Bottom - formattedText.Height;
						break;
				}
			}
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
			_textDrawing.Brush = PainterCache.GetBrush(elementText.ForegroundColor);
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

			var item = DesignerCanvas.CommonDesignerItems.FirstOrDefault(p => p.Element.UID == Element.UID);

			if (item != null && item.WPFControl != null && item.WPFControl is TextBox)
			{
				var colorConverter = new ColorToSystemColorConverter();
				var wpfControl = item.WPFControl as TextBox;

				wpfControl.Background = new SolidColorBrush((Color)colorConverter.Convert(
					elementText.BackgroundColor, elementText.BackgroundColor.GetType(), null, null));
				wpfControl.BorderBrush = new SolidColorBrush((Color)colorConverter.Convert(
					elementText.BorderColor, elementText.BorderColor.GetType(), null, null));
				wpfControl.BorderThickness = new Thickness(elementText.BorderThickness);
				wpfControl.Foreground = new SolidColorBrush((Color)colorConverter.Convert(
					elementText.ForegroundColor, elementText.ForegroundColor.GetType(), null, null));
				wpfControl.FontSize = elementText.FontSize;
				wpfControl.FontFamily = new FontFamily(elementText.FontFamilyName);
				wpfControl.TextAlignment = (TextAlignment)elementText.TextAlignment;
				wpfControl.TextWrapping = elementText.WordWrap ? TextWrapping.Wrap : TextWrapping.NoWrap;
				wpfControl.VerticalAlignment = (VerticalAlignment)elementText.VerticalAlignment;
				//wpfControl.FontStretch = FontStretches.Condensed;
				//TODO: elementText.Stretch
			}

			base.Invalidate();
		}
	}
}