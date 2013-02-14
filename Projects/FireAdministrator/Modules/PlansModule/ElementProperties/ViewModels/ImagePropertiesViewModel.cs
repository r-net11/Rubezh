using System;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Common;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Microsoft.Win32;
using Infrastructure;
using Infrustructure.Plans.Elements;

namespace PlansModule.ViewModels
{
	public class ImagePropertiesViewModel : BaseViewModel
	{
		private IElementBackground _element;
		private Guid? _imageSource;
		private string _sourceName;
		private bool _newImage;
		public Image Image { get; private set; }

		public ImagePropertiesViewModel(IElementBackground element)
		{
			_newImage = false;
			_element = element;
			_sourceName = _element.BackgroundSourceName;
			_imageSource = _element.BackgroundImageSource;
			SelectPictureCommand = new RelayCommand(OnSelectPicture);
			RemovePictureCommand = new RelayCommand(OnRemovePicture);
			UpdateImage();
		}

		public RelayCommand SelectPictureCommand { get; private set; }
		void OnSelectPicture()
		{
			var openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Все файлы изображений|*.bmp; *.png; *.jpeg; *.jpg|BMP Файлы|*.bmp|PNG Файлы|*.png|JPEG Файлы|*.jpeg|JPG Файлы|*.jpg";
			if (openFileDialog.ShowDialog().Value)
			{
				// TODO: ограничить размер файла
				_newImage = true;
				_sourceName = openFileDialog.FileName;
				_imageSource = null;
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
			_newImage = false;
			UpdateImage();
		}

		public void Save()
		{
			if (_newImage)
				_imageSource = ServiceFactory.ContentService.AddContent(_sourceName);
			_element.BackgroundImageSource = _imageSource;
			_element.BackgroundSourceName = _sourceName;
		}
		private void UpdateImage()
		{
			try
			{
				BitmapImage bitmapImage = null;
				if (_newImage && !string.IsNullOrEmpty(_sourceName))
					bitmapImage = new BitmapImage(new Uri(_sourceName));
				else if (_imageSource.HasValue)
					bitmapImage = ServiceFactory.ContentService.GetBitmapContent(_imageSource.Value);

				Image = new Image()
				{
					Source = bitmapImage,
					Stretch = Stretch.Uniform
				};
				OnPropertyChanged("Image");
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове ImagePropertiesViewModel.UpdateImage");
				MessageBoxService.ShowWarning("Возникла ошибка при загрузке изображения");
			}
		}
	}
}
