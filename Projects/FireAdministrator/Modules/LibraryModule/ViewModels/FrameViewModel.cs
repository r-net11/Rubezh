using System.Windows.Controls;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Microsoft.Win32;

namespace LibraryModule.ViewModels
{
    public class FrameViewModel : BaseViewModel
    {
        public static readonly string EmptyFrame = "<?xml version=\"1.0\" encoding=\"utf-16\"?>\n<Canvas Width=\"500\" Height=\"500\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">\n</Canvas>";
        public static readonly string ErrorFrame = "<?xml version=\"1.0\" encoding=\"utf-16\"?>\n<Canvas Width=\"500\" Height=\"500\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">\n<Border BorderBrush=\"Red\" BorderThickness=\"5\" Padding=\"20\">\n<TextBlock Text=\"Error Xaml Code\" FontSize=\"60\" />\n</Border>\n</Canvas>";
        public static readonly int DefaultDuration = 300;
        public static readonly int DefaultLayer = 0;

        public FrameViewModel(LibraryFrame libraryFrame)
        {
            Frame = libraryFrame;
            ImportSvgCommand = new RelayCommand(OnImportSvg);
            ExportSvgCommand = new RelayCommand(OnExportSvg);
        }

        public LibraryFrame Frame { get; private set; }

        public int Layer
        {
            get { return Frame.Layer; }
            set
            {
                if (value != Frame.Layer)
                {
                    Frame.Layer = value;
                    ServiceFactory.SaveService.LibraryChanged = true;
                }
            }
        }

        public int Duration
        {
            get { return Frame.Duration; }
            set
            {
                if (value != Frame.Duration)
                {
                    Frame.Duration = value;
                    ServiceFactory.SaveService.LibraryChanged = true;
                }
            }
        }

        public Canvas XamlOfImage
        {
            get
            {
                try { return ImageConverters.Xml2Canvas(Frame.Image, Frame.Layer); }
                catch { return ImageConverters.Xml2Canvas(ErrorFrame, Frame.Layer); }
            }
        }

        public static LibraryFrame GetDefaultFrameWith(int id)
        {
            return new LibraryFrame()
            {
                Duration = DefaultDuration,
                Id = id,
                Layer = DefaultLayer,
                Image = EmptyFrame
            };
        }

        public RelayCommand ImportSvgCommand { get; private set; }
        void OnImportSvg()
        {
            var openFileDialog = new OpenFileDialog()
            {
                Filter = "Svg Files (.svg)|*.svg"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                Frame.Image = ImageConverters.Svg2Xaml(openFileDialog.FileName);
                OnPropertyChanged("XamlOfImage");

                ServiceFactory.SaveService.LibraryChanged = true;
            }
        }

        public RelayCommand ExportSvgCommand { get; private set; }
        void OnExportSvg()
        {
            var saveFileDialog = new SaveFileDialog()
            {
                Filter = "Svg Files (.svg)|*.svg",
                RestoreDirectory = true,
                DefaultExt = "Svg Files (.svg)|*.svg",
                ValidateNames = true
            };

            if (saveFileDialog.ShowDialog() == true)
                ImageConverters.Xaml2Svg(Frame.Image, saveFileDialog.FileName);
        }
    }
}