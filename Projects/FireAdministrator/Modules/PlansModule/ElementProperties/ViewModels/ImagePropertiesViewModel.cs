using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using System.Windows.Media.Imaging;
using System.IO;
using Microsoft.Win32;
using System.Windows.Controls;
using System.Windows.Media;

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

        public RelayCommand RemovePictureCommand { get; private set; }
        void OnRemovePicture()
        {
            BackgroundPixels = null;
            UpdateImage();
        }
    }
}
