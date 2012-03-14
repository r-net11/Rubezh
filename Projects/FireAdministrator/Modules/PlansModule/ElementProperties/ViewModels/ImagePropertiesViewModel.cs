using System;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Controls.MessageBox;
using Infrastructure.Common;
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

        public RelayCommand SelectPictureCommand { get; set; }
        void OnSelectPicture()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Все файлы изображений|*.bmp; *.png; *.jpeg; *.jpg|BMP Файлы|*.bmp|PNG Файлы|*.png|JPEG Файлы|*.jpeg|JPG Файлы|*.jpg";
            if (openFileDialog.ShowDialog().Value)
            {
                Uri uri = new Uri(openFileDialog.FileName);
                BackgroundPixels = File.ReadAllBytes(openFileDialog.FileName);
                UpdateImage();
            }
        }

        public void UpdateImage()
        {
            try
            {
                BitmapImage bitmapImage = null;
                if (BackgroundPixels != null)
                    using (var imageStream = new MemoryStream(BackgroundPixels))
                    {
                        bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.StreamSource = imageStream;
                        bitmapImage.EndInit();
                    }
                Image = new Image()
                {
                    Source = bitmapImage,
                    Stretch = Stretch.Uniform
                };
                OnPropertyChanged("Image");
            }
            catch (Exception e)
            {
                MessageBoxService.ShowWarning("Возникла ошибка при загрузке изоюражения");
            }
        }

        public RelayCommand RemovePictureCommand { get; private set; }
        void OnRemovePicture()
        {
            BackgroundPixels = null;
            UpdateImage();
        }
    }
}
