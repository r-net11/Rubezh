﻿using Common;
using Infrastructure.Client.Converters;
using Infrastructure.Client.Images;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans.Elements;
using Microsoft.Win32;
using System;
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
		public TileBrush ImageBrush { get; private set; }

		public ImagePropertiesViewModel(IElementBackground element)
		{
			_drawing = null;
			_wmf = null;
			_newImage = false;
			_element = element;
			_sourceName = _element.BackgroundSourceName;
			_imageSource = _element.BackgroundImageSource;
			_imageType = _element.ImageType;
			SelectPictureCommand = new RelayCommand(OnSelectPicture);
			RemovePictureCommand = new RelayCommand(OnRemovePicture, CanRemovePicture);
			UpdateImage();
		}

		public RelayCommand SelectPictureCommand { get; private set; }
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
		}

		public RelayCommand RemovePictureCommand { get; private set; }
		void OnRemovePicture()
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
		bool CanRemovePicture()
		{
			return ImageBrush != null;
		}

		public void Save()
		{
			if (_newImage)
				using (new WaitWrapper())
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