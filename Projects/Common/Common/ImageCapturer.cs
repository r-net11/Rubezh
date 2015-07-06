using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Common
{
	public static class ImageCapturer
	{
		public enum Encoder
		{
			Bmp,
			Gif,
			Jpeg,
			Png,
			Tiff,
		}

		public static void Save(FrameworkElement visual, string fileName, Encoder encoder)
		{
			using (var fs = File.Create(fileName))
				Save(visual, fs, encoder);
		}

		public static void Save(FrameworkElement visual, Stream stream, Encoder encoder)
		{
			Save(visual, stream, GetEncoder(encoder));
		}

		public static void Save(FrameworkElement visual, Stream stream, BitmapEncoder encoder)
		{
			RenderTargetBitmap bitmap = new RenderTargetBitmap((int)visual.ActualWidth, (int)visual.ActualHeight, 96, 96, PixelFormats.Pbgra32);
			bitmap.Render(visual);
			BitmapFrame frame = BitmapFrame.Create(bitmap);
			encoder.Frames.Add(frame);
			encoder.Save(stream);
		}

		public static BitmapEncoder GetEncoder(Encoder encoder)
		{
			switch (encoder)
			{
				case Encoder.Bmp:
					return new BmpBitmapEncoder();

				case Encoder.Gif:
					return new GifBitmapEncoder();

				case Encoder.Jpeg:
					return new JpegBitmapEncoder();

				case Encoder.Png:
					return new PngBitmapEncoder();

				case Encoder.Tiff:
					return new TiffBitmapEncoder();

				default:
					throw new ApplicationException();
			}
		}
	}
}