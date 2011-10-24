using System.Windows.Media;
using System.Windows.Shapes;
using FiresecAPI.Models;
using Infrastructure.Common;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Controls;

namespace PlansModule.ViewModels
{
    public class RectanglePropertiesViewModel : SaveCancelDialogContent
    {
        ElementRectangle _elementRectangle;

        public RectanglePropertiesViewModel(ElementRectangle elementRectangle)
        {
            SelectPictureCommand = new RelayCommand(OnSelectPicture);
            RemovePictureCommand = new RelayCommand(OnRemovePicture);

            Title = "Свойства фигуры: Прямоугольник";
            _elementRectangle = elementRectangle;
            CopyProperties();
        }

        void CopyProperties()
        {
            BackgroundColor = _elementRectangle.BackgroundColor;
            BorderColor = _elementRectangle.BorderColor;
            StrokeThickness = _elementRectangle.BorderThickness;
            BackgroundPixels = _elementRectangle.BackgroundPixels;
            UpdateImage();
        }

        Color _backgroundColor;
        public Color BackgroundColor
        {
            get { return _backgroundColor; }
            set
            {
                _backgroundColor = value;
                OnPropertyChanged("BackgroundColor");
            }
        }

        Color _borderColor;
        public Color BorderColor
        {
            get { return _borderColor; }
            set
            {
                _borderColor = value;
                OnPropertyChanged("BorderColor");
            }
        }

        double _strokeThickness;
        public double StrokeThickness
        {
            get { return _strokeThickness; }
            set
            {
                _strokeThickness = value;
                OnPropertyChanged("StrokeThickness");
            }
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

        void UpdateImage()
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

        protected override void Save(ref bool cancel)
        {
            _elementRectangle.BackgroundColor = BackgroundColor;
            _elementRectangle.BorderColor = BorderColor;
            _elementRectangle.BorderThickness = StrokeThickness;
            _elementRectangle.BackgroundPixels = BackgroundPixels;
        }
    }
}