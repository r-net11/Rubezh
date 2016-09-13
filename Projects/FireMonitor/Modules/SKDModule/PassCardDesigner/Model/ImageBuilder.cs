using Common;
using Infrastructure.Common.Windows;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Windows.Media.Color;
using Pen = System.Drawing.Pen;

namespace SKDModule.PassCardDesigner.Model
{
	public sealed class ImageBuilder
	{
		private Bitmap _image;
		private Color _color;
		private Color _borderColor;
		private int _pixelWidth;
		private int _pixelHeight;
		private int _borderWidth;
		private readonly string _imageSource;

		public ImageBuilder(string fileName)
		{
			_imageSource = fileName;
		}

		public ImageBuilder()
		{
		}

		public ImageBuilder Color(Color color)
		{
			_color = color;
			return this;
		}

		public ImageBuilder BorderColor(Color color)
		{
			_borderColor = color;
			return this;
		}

		public ImageBuilder Width(int pixelWidth)
		{
			_pixelWidth = pixelWidth;
			return this;
		}

		public ImageBuilder Height(int pixelHeight)
		{
			_pixelHeight = pixelHeight;
			return this;
		}

		public ImageBuilder BorderWidth(int width)
		{
			_borderWidth = width;
			return this;
		}

		public Bitmap Build()
		{
			return GenerateImage();
		}

		private Bitmap GenerateImage()
		{
			_image = string.IsNullOrEmpty(_imageSource)
				? new Bitmap(_pixelWidth, _pixelHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb)
				: LoadImageFromFile();

			if (_image == null)
				return GenerateDefaultImage();

			_image = ImageBuilderHelper.DrawRectangleWithBorderBasedOn(_image, _pixelWidth, _pixelHeight, _color, _borderColor, _borderWidth);

			return _image;
		}

		private Bitmap GenerateDefaultImage()
		{
			_image = ImageBuilderHelper.DrawRectangleWithBorderBasedOn(new Bitmap(_pixelWidth, _pixelHeight), _pixelWidth, _pixelHeight, _color,
				_borderColor, _borderWidth);
			return _image;
		}

		private Bitmap LoadImageFromFile()
		{
			if (!File.Exists(_imageSource))
				MessageBoxService.ShowError("Файл не найден.");

			try
			{
				return new Bitmap(Image.FromFile(_imageSource), _pixelWidth, _pixelHeight);
			}
			catch (Exception e)
			{
				MessageBoxService.ShowError("Произошла ошибка во время загрузки изображения.");
				Logger.Error(e);
			}

			return null;
		}
	}

	public static class ImageBuilderHelper
	{
		public static BitmapSource CreateBitmapSource(Color color)
		{
			const int width = 64;
			const int height = 64;
			const int stride = width/8;

			var pixels = new byte[height * stride];
			var colors = new List<Color> { color };
			var pallete = new BitmapPalette(colors);

			return BitmapSource.Create(width, height, 96, 96, PixelFormats.Indexed1, pallete, pixels, stride);
		}

		public static Bitmap DrawRectangleWithBorderBasedOn(Bitmap image, int imageWidth, int imageHeight, Color backgroundColor, Color borderColor, int borderWidth)
		{
			using (var g = Graphics.FromImage(image))
			{
				g.FillRectangle(new SolidBrush(System.Drawing.Color.FromArgb(backgroundColor.A, backgroundColor.R, backgroundColor.G, backgroundColor.B)), 0, 0, imageWidth, imageHeight);

				if (borderWidth != (default(int)))
				{
					var pen = new Pen(System.Drawing.Color.FromArgb(borderColor.A, borderColor.R, borderColor.G, borderColor.B), borderWidth) { Alignment = PenAlignment.Inset };
					g.DrawRectangle(pen, new Rectangle(0, 0, image.Width, image.Height));
				}
			}

			return image;
		}
	}
}
