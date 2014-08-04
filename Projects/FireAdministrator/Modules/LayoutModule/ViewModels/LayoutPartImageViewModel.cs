using System;
using System.Collections.Generic;
using System.Windows.Media;
using Common;
using FiresecAPI.Models.Layouts;
using Infrastructure.Common.Services;
using Infrastructure.Common.Services.Layout;
using Infrastructure.Common.Windows;

namespace LayoutModule.ViewModels
{
	public class LayoutPartImageViewModel : BaseLayoutPartViewModel
	{
		private LayoutPartImageProperties _properties;
		private LayoutPartPropertyPageViewModel _imagePage;
		public LayoutPartImageViewModel(LayoutPartImageProperties properties)
		{
			_imageSource = null;
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

		private Stretch _stretch;
		public Stretch Stretch
		{
			get { return _stretch; }
			set
			{
				_stretch = value;
				OnPropertyChanged(() => Stretch);
			}
		}
		private ImageSource _imageSource;
		public ImageSource ImageSource
		{
			get { return _imageSource; }
			set
			{
				_imageSource = value;
				OnPropertyChanged(() => ImageSource);
			}
		}

		public void UpdateLayoutPart()
		{
			Stretch = _properties.Stretch;
			ImageSource = GetImage(_properties.SourceUID, _properties.IsVectorImage);
		}
		private ImageSource GetImage(Guid uid, bool isVector)
		{
			ImageSource imageSource = null;
			if (uid != Guid.Empty)
				try
				{
					if (isVector)
						imageSource = new DrawingImage(ServiceFactoryBase.ContentService.GetDrawing(uid));
					else
						imageSource = ServiceFactoryBase.ContentService.GetBitmapContent(uid);
				}
				catch (Exception e)
				{
					Logger.Error(e, "Исключение при вызове LayoutPartPropertyImagePageViewModel.UpdateImage");
					MessageBoxService.ShowWarning("Возникла ошибка при загрузке изображения");
				}
			return imageSource;
		}
	}
}