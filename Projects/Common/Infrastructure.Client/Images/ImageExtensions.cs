using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Infrastructure.Client.Converters
{
	public static class ImageExtensions
	{
		public const string SVGGraphicExtensions = ".svg;.svgx";
		public const string WMFGraphicExtensions = ".wmf;.emf";
		public const string RasterGraphicExtensions = ".bmp;.png;.jpeg;.jpg";

        //public const string RaterGraphicFilter = "Все файлы изображений|*.bmp; *.png; *.jpeg; *.jpg|BMP Файлы|*.bmp|PNG Файлы|*.png|JPEG Файлы|*.jpeg|JPG Файлы|*.jpg";
	    public static string RaterGraphicFilter = Resources.Language.ImageExtensions.RaterGraphicFilter;
		//public const string VectorGraphicFilter = "Все файлы изображений|*.svg; *.svgx; *.wmf; *.emf|SVG Файлы|*.svg; *.svgx|WMF Файлы|*.wmf|EMF Файлы|*.emf";
	    public static string VectorGraphicFilter = Resources.Language.ImageExtensions.VectorGraphicFilter;
		//public const string GraphicFilter = "Все файлы изображений|*.bmp; *.png; *.jpeg; *.jpg; *.svg; *.svgx; *.wmf; *.emf|BMP Файлы|*.bmp|PNG Файлы|*.png|JPEG Файлы|*.jpeg|JPG Файлы|*.jpg|SVG Файлы|*.svg;*.svgx|WMF Файлы|*.wmf|EMF Файлы|*.emf";
	    public static string GraphicFilter = Resources.Language.ImageExtensions.GraphicFilter;

		public static bool IsGraphics(string fileName)
		{
			return IsRasterGraphics(fileName) || IsVectorGraphics(fileName);
		}
		public static bool IsRasterGraphics(string fileName)
		{
			return CheckFileExtension(fileName, RasterGraphicExtensions);
		}
		public static bool IsVectorGraphics(string fileName)
		{
			return IsSVGGraphics(fileName) || IsWMFGraphics(fileName);
		}
		public static bool IsSVGGraphics(string fileName)
		{
			return CheckFileExtension(fileName, SVGGraphicExtensions);
		}
		public static bool IsWMFGraphics(string fileName)
		{
			return CheckFileExtension(fileName, WMFGraphicExtensions);
		}
		private static bool CheckFileExtension(string fileName, string extensionList)
		{
			return extensionList.Split(';').Contains(Path.GetExtension(fileName).ToLower());
		}
	}
}
