using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;

namespace Infrastructure.Client.Images
{
	public static class ImageHelper
	{
		public static TileBrush GetResourceBrush(Guid uid, ResourceType type, bool showError = true)
		{
			TileBrush brush = null;
			if (uid != Guid.Empty)
				try
				{
					switch (type)
					{
						case ResourceType.Image:
							brush = new ImageBrush(ServiceFactoryBase.ContentService.GetBitmapContent(uid));
							break;
						case ResourceType.Visual:
							brush = new VisualBrush(ServiceFactoryBase.ContentService.GetVisual(uid));
							break;
						case ResourceType.Drawing:
							brush = new DrawingBrush(ServiceFactoryBase.ContentService.GetDrawing(uid));
							break;
					}
				}
				catch (Exception e)
				{
					Logger.Error(e, "Исключение при вызове ImageHelper.GetResourceImage({0},{1})", uid, type);
					if (showError)
						MessageBoxService.ShowWarningExtended("Возникла ошибка при загрузке изображения");
				}
			return brush;
		}
	}
}
