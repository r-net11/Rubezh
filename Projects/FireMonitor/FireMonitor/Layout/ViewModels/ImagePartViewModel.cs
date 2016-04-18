using Infrastructure.Client.Images;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans;
using RubezhAPI.Models.Layouts;
using System.Windows.Media;

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
					brush.Stretch = properties.Stretch.ToWindowsStretch();
				ImageBrush = brush;
			}
		}

		TileBrush _imageBrush;
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