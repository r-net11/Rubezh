using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Common;
using Infrastructure.Client.Converters;
using Infrastructure.Client.Images;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using StrazhAPI.Plans.Elements;
using Microsoft.Win32;

namespace Infrastructure.Designer.ElementProperties.ViewModels
{
	public class ImagePropertiesViewModel : BaseViewModel
	{
		private TileBrush _imageBrush;
		private readonly IElementBackground _element;
		private Guid? _imageSource;
		private ResourceType _imageType;
		private bool _newImage;
		private DrawingGroup _drawing;
		private WMFImage _wmf;
		private string _backgroundSourceName;

		public string BackgroundSourceName

		{
			get { return _backgroundSourceName; }
			set
			{
				if (string.Equals(_backgroundSourceName, value)) return;
				_backgroundSourceName = value;
				OnPropertyChanged(() => BackgroundSourceName);
			}
		}

		public DrawingGroup Drawing
		{
			get { return _drawing; }
			set
			{
				_drawing = value;
				OnPropertyChanged(() => Drawing);
			}
		}

		public WMFImage WMF
		{
			get { return _wmf; }
			set
			{
				_wmf = value;
				OnPropertyChanged(() => WMF);
			}
		}

		public TileBrush ImageBrush
		{
			get { return _imageBrush; }
			set
			{
				_imageBrush = value;
				OnPropertyChanged(() => ImageBrush);
			}
		}

		public ImagePropertiesViewModel()
		{
			SelectPictureCommand = new RelayCommand(OnSelectPicture);
			RemovePictureCommand = new RelayCommand(OnRemovePicture, () => !string.IsNullOrEmpty(BackgroundSourceName));
		}

		public ImagePropertiesViewModel(IElementBackground element) : this()
		{
			_drawing = null;
			_wmf = null;
			_newImage = false;

			if (element != null)
			{
				_element = element;
				BackgroundSourceName = _element.BackgroundSourceName;
				_imageSource = _element.BackgroundImageSource;
				_imageType = _element.ImageType;
			}

			UpdateImage();
		}

		public RelayCommand SelectPictureCommand { get; private set; }
		private void OnSelectPicture()
		{
			_newImage = true;
			var openFileDialog = new OpenFileDialog { Filter = ImageExtensions.GraphicFilter };

			if (!openFileDialog.ShowDialog().Value) return;

			BackgroundSourceName = openFileDialog.FileName;
			if (ImageExtensions.IsSVGGraphics(BackgroundSourceName))
			{
				Drawing = SVGConverters.ReadDrawing(BackgroundSourceName);
				WMF = null;
				ImageBrush = new DrawingBrush(Drawing);
				_imageType = ResourceType.Drawing;
			}
			else if (ImageExtensions.IsWMFGraphics(BackgroundSourceName))
			{
				WMF = WMFConverter.ReadWMF(BackgroundSourceName);
				Drawing = WMF == null ? null : WMF.ToDrawing();
				if (Drawing == null)
				{
					ImageBrush = new VisualBrush(WMF.Canvas);
					_imageType = ResourceType.Visual;
				}
				else
				{
					WMF = null;
					ImageBrush = new DrawingBrush(Drawing);
					_imageType = ResourceType.Drawing;
				}
			}
			else
			{
				Drawing = null;
				WMF = null;
				ImageBrush = new ImageBrush(new BitmapImage(new Uri(BackgroundSourceName)));
				_imageType = ResourceType.Image;
			}
		}

		public RelayCommand RemovePictureCommand { get; private set; }
		private void OnRemovePicture()
		{
			if (_imageSource.HasValue)
				ServiceFactoryBase.ContentService.RemoveContent(_imageSource.Value);

			_imageSource = null;
			BackgroundSourceName = null;
			Drawing = null;
			WMF = null;
			ImageBrush = null;
		}

		public void Save()
		{
			if (_newImage)
			{
				if (_imageSource.HasValue && _imageSource.Value != Guid.Empty)
					ServiceFactoryBase.ContentService.RemoveContent(_imageSource.Value);

				switch (_imageType)
				{
					case ResourceType.Drawing:
						_imageSource = ServiceFactoryBase.ContentService.AddContent(_drawing);
						break;
					case ResourceType.Image:
						_imageSource = ServiceFactoryBase.ContentService.AddContent(BackgroundSourceName);
						break;
					case ResourceType.Visual:
						_imageSource = ServiceFactoryBase.ContentService.AddContent(_wmf.Canvas);
						break;
				}
			}
			_element.BackgroundImageSource = _imageSource;
			_element.BackgroundSourceName = BackgroundSourceName;
			_element.ImageType = _imageType;
		}

		public void UpdateImage()
		{
			try
			{
				ImageBrush = ImageHelper.GetResourceBrush(_imageSource, _imageType);
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове ImagePropertiesViewModel.UpdateImage");
				MessageBoxService.ShowWarning("Возникла ошибка при загрузке изображения");
			}
		}
	}
}
