using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using System.Windows.Documents;
using System.IO;

namespace Infrastructure.Client.Images
{
	public static class ImageHelper
	{
		public static TileBrush GetResourceBrush(Guid? uid, ResourceType type, bool showError = true)
		{
			TileBrush brush = null;
			if (uid.HasValue && uid != Guid.Empty)
				try
				{
					switch (type)
					{
						case ResourceType.Image:
							brush = new ImageBrush(ServiceFactoryBase.ContentService.GetBitmapContent(uid.Value));
							break;
						case ResourceType.Visual:
							var visual = ServiceFactoryBase.ContentService.GetVisual(uid.Value);
							UpdateReferences(visual);
							brush = new VisualBrush(visual);
							break;
						case ResourceType.Drawing:
							brush = new DrawingBrush(ServiceFactoryBase.ContentService.GetDrawing(uid.Value));
							break;
					}
				}
				catch (Exception e)
				{
					Logger.Error(e, "Исключение при вызове ImageHelper.GetResourceImage({0},{1})", uid, type);
					if (showError)
                        MessageBoxService.ShowWarningExtended(Resources.Language.Images.ImageHelper.GetResourceBrush_Exception);
				}
			return brush;
		}

		public static Guid SaveImage(WMFImage wmf)
		{
			if (wmf == null || wmf.Canvas == null)
				return Guid.Empty;
			foreach (var glyph in wmf.Canvas.FindVisualChildren<Glyphs>())
			{
				var glyphGuid = new Guid(Path.GetFileNameWithoutExtension(glyph.FontUri.ToString()));
				var data = wmf.Resources[glyphGuid];
				var resourceGuid = ServiceFactoryBase.ContentService.AddContent(data);
				glyph.FontUri = new Uri(resourceGuid.ToString(), UriKind.Relative);
			}
			return ServiceFactoryBase.ContentService.AddContent(wmf.Canvas);
		}
		private static void UpdateReferences(Visual visual)
		{
			foreach (var glyph in visual.FindVisualChildren<Glyphs>())
				if (!glyph.FontUri.IsAbsoluteUri)
				{
					var glyphGuid = glyph.FontUri.ToString();
					glyph.FontUri = new Uri(ServiceFactoryBase.ContentService.GetContentFileName(glyphGuid), UriKind.Absolute);
				}
		}
	}
}
