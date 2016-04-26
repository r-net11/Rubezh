using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Common;
using Infrastructure.Client.Converters;
using Infrastructure.Client.Images;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans.Elements;
using Microsoft.Win32;

namespace Infrastructure.Designer.ElementProperties.ViewModels
{
	public class ImagePropertiesViewModel : BaseViewModel
	{
		private IElementBackground _element;
		private Guid? _imageSource;
		private string _sourceName;
		private ResourceType _imageType;
		private bool _newImage;
		private DrawingGroup _drawing;
		private WMFImage _wmf;
		public TileBrush ImageBrush { get; private set; }

		public ImagePropertiesViewModel(IElementBackground element)
		{
			SelectPictureCommand = new RelayCommand(OnSelectPicture);
			RemovePictureCommand = new RelayCommand(OnRemovePicture, CanRemovePicture);

			_drawing = null;
			_wmf = null;
			_newImage = false;

			if (element != null)
			{
				_element = element;
				_sourceName = _element.BackgroundSourceName;
				_imageSource = _element.BackgroundImageSource;
				_imageType = _element.ImageType;
			}
			UpdateImage();
		}

		public RelayCommand SelectPictureCommand { get; private set; }
		private void OnSelectPicture()
		{
			var openFileDialog = new OpenFileDialog {Filter = ImageExtensions.GraphicFilter};

			if (!openFileDialog.ShowDialog().Value) return;

			_newImage = true;
			_sourceName = openFileDialog.FileName;
			if (ImageExtensions.IsSVGGraphics(_sourceName))
			{
				_drawing = SVGConverters.ReadDrawing(_sourceName);
				_wmf = null;
				ImageBrush = new DrawingBrush(_drawing);
				_imageType = ResourceType.Drawing;
			}
			else if (ImageExtensions.IsWMFGraphics(_sourceName))
			{
				_wmf = WMFConverter.ReadWMF(_sourceName);
				_drawing = _wmf == null ? null : _wmf.ToDrawing();
				if (_drawing == null)
				{
					ImageBrush = new VisualBrush(_wmf.Canvas);
					_imageType = ResourceType.Visual;
				}
				else
				{
					_wmf = null;
					ImageBrush = new DrawingBrush(_drawing);
					_imageType = ResourceType.Drawing;
				}
			}
			else
			{
				_drawing = null;
				_wmf = null;
				ImageBrush = new ImageBrush(new BitmapImage(new Uri(_sourceName)));
				_imageType = ResourceType.Image;
			}
			OnPropertyChanged(() => ImageBrush);
		}

		public RelayCommand RemovePictureCommand { get; private set; }
		private void OnRemovePicture()
		{
			if (_imageSource.HasValue)
				ServiceFactoryBase.ContentService.RemoveContent(_imageSource.Value);
			_imageSource = null;
			_sourceName = null;
			_newImage = false;
			_drawing = null;
			_wmf = null;
			UpdateImage();
		}
		private bool CanRemovePicture()
		{
			return ImageBrush != null;
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
						_imageSource = ServiceFactoryBase.ContentService.AddContent(_sourceName);
						break;
					case ResourceType.Visual:
						_imageSource = ServiceFactoryBase.ContentService.AddContent(_wmf.Canvas);
						break;
				}
			}
			_element.BackgroundImageSource = _imageSource;
			_element.BackgroundSourceName = _sourceName;
			_element.ImageType = _imageType;
		}

		private void UpdateImage()
		{
			try
			{
				ImageBrush = ImageHelper.GetResourceBrush(_imageSource, _imageType);
				OnPropertyChanged(() => ImageBrush);
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове ImagePropertiesViewModel.UpdateImage");
				MessageBoxService.ShowWarning("Возникла ошибка при загрузке изображения");
			}
		}
	}
}
