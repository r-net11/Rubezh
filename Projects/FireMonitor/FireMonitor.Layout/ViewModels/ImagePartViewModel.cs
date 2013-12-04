using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Models.Layouts;
using System.Windows.Media;
using Infrastructure.Common.Services;

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