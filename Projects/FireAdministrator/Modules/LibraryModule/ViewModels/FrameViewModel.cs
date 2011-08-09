using System.Windows.Controls;
using Infrastructure.Common;
using Microsoft.Win32;

namespace LibraryModule.ViewModels
{
    public class FrameViewModel : BaseViewModel
    {
        public static readonly string emptyFrame = "<?xml version=\"1.0\" encoding=\"utf-16\"?>\n<Canvas Width=\"500\" Height=\"500\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">\n</Canvas>";
        public static readonly string errorFrame = "<?xml version=\"1.0\" encoding=\"utf-16\"?>\n<Canvas Width=\"500\" Height=\"500\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">\n<Border BorderBrush=\"Red\" BorderThickness=\"5\" Padding=\"20\">\n<TextBlock Text=\"Error Xaml Code\" FontSize=\"60\" />\n</Border>\n</Canvas>";
        public static readonly int defaultDuration = 300;
        public static readonly int defaultLayer = 0;

        public FrameViewModel(DeviceLibrary.Models.Frame frame)
        {
            Frame = new DeviceLibrary.Models.Frame();
            Frame.Duration = frame.Duration;
            Frame.Id = frame.Id;
            Frame.Image = frame.Image;
            Frame.Layer = frame.Layer;

            ImportSvgCommand = new RelayCommand(OnImportSvg);
        }

        public DeviceLibrary.Models.Frame Frame { get; private set; }

        public Canvas XamlOfImage
        {
            get
            {
                try
                {
                    return ImageConverters.Xml2Canvas(Frame.Image, Frame.Layer);
                }
                catch
                {
                    return ImageConverters.Xml2Canvas(errorFrame, Frame.Layer);
                }
            }
        }

        public static DeviceLibrary.Models.Frame GetDefaultFrameWith(int id)
        {
            var frame = new DeviceLibrary.Models.Frame();
            frame.Duration = defaultDuration;
            frame.Id = id;
            frame.Layer = defaultLayer;
            frame.Image = emptyFrame;

            return frame;
        }

        public RelayCommand ImportSvgCommand { get; private set; }
        void OnImportSvg()
        {
            var openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Text Files (.svg)|*.svg";

            if (openFileDialog1.ShowDialog() == true)
            {
                Frame.Image = ImageConverters.Svg2Xaml(openFileDialog1.FileName, PathHelper.TransormFileName);
                OnPropertyChanged("XamlOfImage");
            }
        }
    }
}