using System.Windows.Controls;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Microsoft.Win32;
using XFiresecAPI;

namespace GKModule.ViewModels
{
    public class XFrameViewModel : BaseViewModel
    {
        public static readonly string ErrorFrame = "<?xml version=\"1.0\" encoding=\"utf-16\"?>\n<Canvas Width=\"500\" Height=\"500\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">\n<Border BorderBrush=\"Red\" BorderThickness=\"5\" Padding=\"20\">\n<TextBlock Text=\"Error Xaml Code\" FontSize=\"60\" />\n</Border>\n</Canvas>";

        public XFrameViewModel(LibraryXFrame libraryXFrame)
        {
            XFrame = libraryXFrame;
            ImportSvgCommand = new RelayCommand(OnImportSvg);
            ExportSvgCommand = new RelayCommand(OnExportSvg);
        }

        public LibraryXFrame XFrame { get; private set; }

        public int Duration
        {
            get { return XFrame.Duration; }
            set
            {
                if (value != XFrame.Duration)
                {
                    XFrame.Duration = value;
                    ServiceFactory.SaveService.XLibraryChanged = true;
                }
            }
        }

        public Canvas XamlOfImage
        {
            get
            {
                try { return ImageConverters.Xml2Canvas(XFrame.Image); }
                catch { return ImageConverters.Xml2Canvas(ErrorFrame); }
            }
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
                XFrame.Image = ImageConverters.Svg2Xaml(openFileDialog.FileName);
                OnPropertyChanged("XamlOfImage");
                XLibraryViewModel.Current.SelectedXState = XLibraryViewModel.Current.SelectedXState;
                ServiceFactory.SaveService.XLibraryChanged = true;
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
                ImageConverters.Xaml2Svg(XFrame.Image, saveFileDialog.FileName);
        }
    }
}