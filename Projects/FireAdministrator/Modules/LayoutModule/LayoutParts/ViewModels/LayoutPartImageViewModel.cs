using System;
using System.Collections.Generic;
using System.Windows.Media;
using Common;
using StrazhAPI.Models.Layouts;
using Infrastructure.Common.Services;
using Infrastructure.Common.Services.Layout;
using Infrastructure.Common.Windows;
using Infrastructure.Client.Images;

namespace LayoutModule.LayoutParts.ViewModels
{
	public class LayoutPartImageViewModel : BaseLayoutPartViewModel
	{
		private LayoutPartImageProperties _properties;
		private LayoutPartPropertyPageViewModel _imagePage;
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

		public void UpdateLayoutPart()
		{
			var brush = ImageHelper.GetResourceBrush(_properties.ReferenceUID, _properties.ImageType);
			if (brush != null)
				brush.Stretch = _properties.Stretch;
			ImageBrush = brush;
		}
	}
}