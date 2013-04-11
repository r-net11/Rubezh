using System;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Common;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans.Elements;
using Microsoft.Win32;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;

namespace PlansModule.ViewModels
{
	public class ImagePropertiesViewModel : BaseViewModel
	{
		private const string VectorGraphicExtensions = ".svg;.svgx";
		private IElementBackground _element;
		private Guid? _imageSource;
		private string _sourceName;
		private bool _isVectorImage;
		private bool _newImage;
		private DrawingGroup _drawing;
		private WpfDrawingSettings _settings;
		public Image Image { get; private set; }

		public ImagePropertiesViewModel(IElementBackground element)
		{
			_drawing = null;
			_newImage = false;
			_element = element;
			_sourceName = _element.BackgroundSourceName;
			_imageSource = _element.BackgroundImageSource;
			_isVectorImage = _element.IsVectorImage;
			_settings = new WpfDrawingSettings()
			{
				IncludeRuntime = false,
				TextAsGeometry = true,
				OptimizePath = true,
			};
			SelectPictureCommand = new RelayCommand(OnSelectPicture);
			RemovePictureCommand = new RelayCommand(OnRemovePicture);
			UpdateImage();
		}

		public bool HasImage
		{
			get { return Image != null && Image.Source != null; }
		}

		public RelayCommand SelectPictureCommand { get; private set; }
		void OnSelectPicture()
		{
			var openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Все файлы изображений|*.bmp; *.png; *.jpeg; *.jpg; *.svg|BMP Файлы|*.bmp|PNG Файлы|*.png|JPEG Файлы|*.jpeg|JPG Файлы|*.jpg|SVG Файлы|*.svg";
			if (openFileDialog.ShowDialog().Value)
			{
				// TODO: ограничить размер файла
				_newImage = true;
				_sourceName = openFileDialog.FileName;
				_imageSource = null;
				_isVectorImage = VectorGraphicExtensions.Contains(Path.GetExtension(_sourceName));
				if (_isVectorImage)
				{
					using (FileSvgReader reader = new FileSvgReader(_settings))
						_drawing = reader.Read(_sourceName);
					_drawing.Freeze();
				}
				UpdateImage();
			}
		}

		public RelayCommand RemovePictureCommand { get; private set; }
		void OnRemovePicture()
		{
			if (_imageSource.HasValue)
				ServiceFactory.ContentService.RemoveContent(_imageSource.Value);
			_imageSource = null;
			_sourceName = null;
			_isVectorImage = false;
			_newImage = false;
			_drawing = null;
			UpdateImage();
		}

		public void Save()
		{
			if (_newImage)
				using (new WaitWrapper())
				{
					if (_isVectorImage)
						_imageSource = ServiceFactory.ContentService.AddContent(_drawing);
					else
						_imageSource = ServiceFactory.ContentService.AddContent(_sourceName);
				}
			_element.BackgroundImageSource = _imageSource;
			_element.BackgroundSourceName = _sourceName;
			_element.IsVectorImage = _isVectorImage;
		}

		private void UpdateImage()
		{
			try
			{
				ImageSource imageSource = null;
				if (_newImage && !string.IsNullOrEmpty(_sourceName))
				{
					if (_isVectorImage)
						imageSource = new DrawingImage(_drawing);
					else
						imageSource = new BitmapImage(new Uri(_sourceName));
				}
				else if (_imageSource.HasValue)
				{
					if (_isVectorImage)
						imageSource = new DrawingImage(ServiceFactory.ContentService.GetDrawing(_imageSource.Value));
					else
						imageSource = ServiceFactory.ContentService.GetBitmapContent(_imageSource.Value);
				}

				Image = new Image()
				{
					Source = imageSource,
					Stretch = Stretch.Uniform
				};
				OnPropertyChanged(() => Image);
				OnPropertyChanged(() => HasImage);
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове ImagePropertiesViewModel.UpdateImage");
				MessageBoxService.ShowWarning("Возникла ошибка при загрузке изображения");
			}
		}
	}
}
