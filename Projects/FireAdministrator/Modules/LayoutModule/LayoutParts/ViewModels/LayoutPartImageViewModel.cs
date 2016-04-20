using Infrastructure.Client.Images;
using Infrastructure.Common.Services.Layout;
using Infrustructure.Plans;
using RubezhAPI.Models.Layouts;
using System.Collections.Generic;
using System.Windows.Media;

namespace LayoutModule.LayoutParts.ViewModels
{
	public class LayoutPartImageViewModel : BaseLayoutPartViewModel
	{
		LayoutPartImageProperties _properties;
		LayoutPartPropertyPageViewModel _imagePage;
		public LayoutPartImageViewModel(LayoutPartImageProperties properties)
		{
			_imageBrush = null;
			_properties = properties ?? new LayoutPartImageProperties();
			_imagePage = new LayoutPartPropertyImagePageViewModel(this);
			UpdateLayoutPart();
		}

		public override ILayoutProperties Properties
		{
			get { return _properties; }
		}
		public override IEnumerable<LayoutPartPropertyPageViewModel> PropertyPages
		{
			get
			{
				yield return _imagePage;
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

		public void UpdateLayoutPart()
		{
			var brush = ImageHelper.GetResourceBrush(_properties.ReferenceUID, _properties.ImageType);
			if (brush != null)
				brush.Stretch = _properties.Stretch.ToWindowsStretch();
			ImageBrush = brush;
		}
	}
}