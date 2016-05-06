using System;
using System.Windows.Media;
using StrazhAPI.Models.Layouts;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Client.Images;

namespace FireMonitor.Layout.ViewModels
{
	public class ImagePartViewModel : BaseViewModel
	{
		public ImagePartViewModel(LayoutPartImageProperties properties)
		{
			if (properties != null)
			{
				var brush = ImageHelper.GetResourceBrush(properties.ReferenceUID, properties.ImageType);
				if (brush != null)
					brush.Stretch = properties.Stretch;
				ImageBrush = brush;
			}
		}

		private TileBrush _imageBrush;
		public TileBrush ImageBrush
		{
			get { return _imageBrush; }
			set
			{
				_imageBrush = value;
				OnPropertyChanged(() => ImageBrush);
			}
		}
	}
}