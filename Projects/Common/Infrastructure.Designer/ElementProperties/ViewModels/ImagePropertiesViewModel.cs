using Common;
using Infrastructure.Client.Converters;
using Infrastructure.Client.Images;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans.Elements;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Infrastructure.Designer.ElementProperties.ViewModels
{
	public class ImagePropertiesViewModel : BaseViewModel
	{
		IElementBackground _element;
		Guid? _imageSource;
		Guid? _svgImageSource;
		string _sourceName;
		ResourceType _imageType;
		bool _newImage;
		DrawingGroup _drawing;
		WMFImage _wmf;
		byte[] _svg;
		public TileBrush ImageBrush { get; private set; }
		public event Action<bool> UpdateProperty;

		public ImagePropertiesViewModel(IElementBackground element)
		{
			_drawing = null;
			_wmf = null;
			_newImage = false;
			_element = element;
			_sourceName = _element.BackgroundSourceName;
			_imageSource = _element.BackgroundImageSource;
			_svgImageSource = _element.BackgroundSVGImageSource;
			_imageType = _element.ImageType;
			SelectPictureCommand = new RelayCommand(OnSelectPicture);
			RemovePictureCommand = new RelayCommand(OnRemovePicture, CanRemovePicture);
			UpdateImage();

		}

		public RelayCommand SelectPictureCommand { get; private set; }
		//[DebuggerStepThrough]
		void OnSelectPicture()
		{
			var openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = ImageExtensions.GraphicFilter;
			if (openFileDialog.ShowDialog().Value)
				using (new WaitWrapper())
				{
					_newImage = true;
					_sourceName = openFileDialog.FileName;
					if (ImageExtensions.IsSVGGraphics(_sourceName))
					{
						_drawing = SVGConverters.ReadDrawing(_sourceName);
						_wmf = null;
						ImageBrush = new DrawingBrush(_drawing);
						_imageType = ResourceType.Drawing;
						_svg = File.ReadAllBytes(_sourceName);
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

						if (new FileInfo(_sourceName).Length > 0)
						{
							ImageBrush = new ImageBrush(new BitmapImage(new Uri(_sourceName)));
							_imageType = ResourceType.Image;
						}
						else
						{
							MessageBoxService.Show("Невозможно загрузить пустое изображение");
							return;
						}

					}
					OnPropertyChanged(() => ImageBrush);
					if (UpdateProperty != null)
					UpdateProperty(false);
				}
		}

		bool isWasDelete;
		public RelayCommand RemovePictureCommand { get; private set; }
		void OnRemovePicture()
		{
			_newImage = false;
			_drawing = null;
			_wmf = null;
			_svg = null;
			isWasDelete = true;
			ImageBrush = null;
			OnPropertyChanged(() => ImageBrush);
			if (UpdateProperty != null)
			UpdateProperty(true);
		}
		bool CanRemovePicture()
		{
			return ImageBrush != null;
		}

		bool ValidateImage()
		{
			return new FileInfo(_sourceName).Length >0;
		}

		public void Save()
		{
			if (_newImage)
				using (new WaitWrapper())
				{
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
			if (_svg != null)
			{
				_element.BackgroundSVGImageSource = ServiceFactoryBase.ContentService.AddContent(_svg);
			}
			else
			{
				_element.BackgroundSVGImageSource = null;
			}

			if (isWasDelete)
			{
				if (_imageSource.HasValue)
					ServiceFactoryBase.ContentService.RemoveContent(_imageSource.Value);
				if (_svgImageSource.HasValue)
					ServiceFactoryBase.ContentService.RemoveContent(_svgImageSource.Value);
				if (!_newImage)
				{
					_imageSource = null;
					_sourceName = null;
				}
			}

			_element.BackgroundImageSource = _imageSource;
			_element.BackgroundSourceName = _sourceName;
			_element.ImageType = _imageType;
		}

		void UpdateImage()
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