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

namespace PlansModule.ViewModels
{
	public class ImagePropertiesViewModel : BaseViewModel
	{
		public ImagePropertiesViewModel()
		{
			SelectPictureCommand = new RelayCommand(OnSelectPicture);
			RemovePictureCommand = new RelayCommand(OnRemovePicture);
		}

		public byte[] BackgroundPixels { get; set; }

		public Image Image { get; private set; }

		public string ImageSource { get; private set; }

		public RelayCommand SelectPictureCommand { get; set; }
		void OnSelectPicture()
		{
			var openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Все файлы изображений|*.bmp; *.png; *.jpeg; *.jpg|BMP Файлы|*.bmp|PNG Файлы|*.png|JPEG Файлы|*.jpeg|JPG Файлы|*.jpg";
			if (openFileDialog.ShowDialog().Value)
			{
				var folderName = AppDataFolderHelper.GetFolder("Administrator/Configuration/Unzip/Images");
				if (!Directory.Exists(folderName))
					Directory.CreateDirectory(folderName);
				var fileInfo = new FileInfo(openFileDialog.FileName);
				var newFileName = Guid.NewGuid() + "." + fileInfo.Extension;
				var newFilePath = Path.Combine(folderName, newFileName);
				File.Copy(openFileDialog.FileName, newFilePath);
				ImageSource = newFileName;

				BitmapImage bitmap = new BitmapImage();
				bitmap.BeginInit();
				bitmap.StreamSource = new FileStream(newFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
				bitmap.EndInit();
				Image = new Image();
				Image.Source = bitmap;
				OnPropertyChanged("Image");

				//BackgroundPixels = File.ReadAllBytes(openFileDialog.FileName);
				//UpdateImage();
			}
		}

		public void UpdateImage()
		{
			//try
			//{
			//    BitmapImage bitmapImage = null;
			//    if (BackgroundPixels != null)
			//        using (var imageStream = new MemoryStream(BackgroundPixels))
			//        {
			//            bitmapImage = new BitmapImage();
			//            bitmapImage.BeginInit();
			//            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
			//            bitmapImage.StreamSource = imageStream;
			//            bitmapImage.EndInit();
			//        }
			//    Image = new Image()
			//    {
			//        Source = bitmapImage,
			//        Stretch = Stretch.Uniform
			//    };
			//    OnPropertyChanged("Image");
			//}
			//catch (Exception e)
			//{
			//    Logger.Error(e, "Исключение при вызове ImagePropertiesViewModel.UpdateImage");
			//    MessageBoxService.ShowWarning("Возникла ошибка при загрузке изображения");
			//}
		}

		public RelayCommand RemovePictureCommand { get; private set; }
		void OnRemovePicture()
		{
			//BackgroundPixels = null;
			//UpdateImage();
		}
	}
}
