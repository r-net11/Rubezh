using System;
using System.Windows.Media;
using FiresecAPI.Models.Layouts;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows.ViewModels;

namespace FireMonitor.Layout.ViewModels
{
	public class ImagePartViewModel : BaseViewModel
	{
		public ImagePartViewModel(LayoutPartImageProperties properties)
		{
			if (properties != null)
			{
				Stretch = properties.Stretch;
				if (properties.SourceUID != Guid.Empty)
				{
					if (properties.IsVectorImage)
						ImageSource = new DrawingImage(ServiceFactoryBase.ContentService.GetDrawing(properties.SourceUID));
					else
						ImageSource = ServiceFactoryBase.ContentService.GetBitmapContent(properties.SourceUID);
				}
			}
		}

		public Stretch Stretch { get; private set; }
		public ImageSource ImageSource { get; private set; }
	}
}