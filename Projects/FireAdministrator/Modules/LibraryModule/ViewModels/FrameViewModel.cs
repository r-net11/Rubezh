using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml;
using Infrastructure.Common;
using Microsoft.Win32;

namespace LibraryModule.ViewModels
{
    public class FrameViewModel : BaseViewModel
    {
        static readonly string emptyFrame = "<?xml version=\"1.0\" encoding=\"utf-16\"?>\n<Canvas Width=\"500\" Height=\"500\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">\n</Canvas>";
        static readonly string errorFrame = "<?xml version=\"1.0\" encoding=\"utf-16\"?>\n<Canvas Width=\"500\" Height=\"500\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">\n<Border BorderBrush=\"Red\" BorderThickness=\"5\" Padding=\"20\">\n<TextBlock Text=\"Error Xaml Code\" FontSize=\"60\" />\n</Border>\n</Canvas>";
        static readonly int defaultDuration = 300;
        static readonly int defaultLayer = 0;

        public FrameViewModel(StateViewModel parent, DeviceLibrary.Models.Frame frame)
        {
            ParentState = parent;
            Id = frame.Id;
            XmlOfImage = frame.Image;
            Duration = frame.Duration;
            Layer = frame.Layer;

            Initialize();
        }

        public FrameViewModel(StateViewModel parent)
        {
            ParentState = parent;
            Id = parent.Frames.Count;
            XmlOfImage = emptyFrame;
            Duration = defaultDuration;
            Layer = defaultLayer;

            Initialize();
        }

        void Initialize()
        {
            AddFrameCommand = new RelayCommand(OnAddFrame);
            RemoveFrameCommand = new RelayCommand(OnRemoveFrame);
            ImportSvgCommand = new RelayCommand(OnImportSvg);
        }

        public StateViewModel ParentState { get; private set; }

        int _id;
        public int Id
        {
            get { return _id; }

            set
            {
                _id = value;
                OnPropertyChanged("Id");
            }
        }

        int _duration;
        public int Duration
        {
            get { return _duration; }

            set
            {
                _duration = value;
                OnPropertyChanged("Duration");
            }
        }

        int _layer;
        public int Layer
        {
            get { return _layer; }

            set
            {
                _layer = value;
                OnPropertyChanged("Layer");
            }
        }

        Canvas _xamlOfImage;
        public Canvas XamlOfImage
        {
            get { return _xamlOfImage; }

            set
            {
                _xamlOfImage = value;
                OnPropertyChanged("XamlOfImage");
            }
        }

        string _xmlOfImage;
        public string XmlOfImage
        {
            get { return _xmlOfImage; }

            set
            {
                _xmlOfImage = value;
                try
                {
                    XamlOfImage = GetCanvasFromXml(value, Layer);
                }
                catch
                {
                    XamlOfImage = GetCanvasFromXml(errorFrame, Layer);
                }

                OnPropertyChanged("XmlOfImage");
            }
        }

        static Canvas GetCanvasFromXml(string image, int layer)
        {
            if (string.IsNullOrWhiteSpace(image)) return new Canvas();

            using (var stringReader = new StringReader(image))
            using (var xmlReader = XmlReader.Create(stringReader))
            {
                Canvas canvas = (Canvas) XamlReader.Load(xmlReader);
                Panel.SetZIndex(canvas, layer);
                return canvas;
            }
        }

        public RelayCommand AddFrameCommand { get; private set; }
        void OnAddFrame()
        {
            ParentState.Frames.Add(new FrameViewModel(ParentState));
        }

        public RelayCommand ImportSvgCommand { get; private set; }
        void OnImportSvg()
        {
            var openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Text Files (.svg)|*.svg";

            if (openFileDialog1.ShowDialog() == true)
            {
                XmlOfImage = ImageConverters.Svg2Xaml(openFileDialog1.FileName, PathHelper.TransormFileName);
            }
        }

        public RelayCommand RemoveFrameCommand { get; private set; }
        void OnRemoveFrame()
        {
            if (ParentState.Frames.Count == 1)
            {
                MessageBox.Show("Невозможно удалить единственный кадр", "Ошибка");
            }
            else
            {
                var result = MessageBox.Show("Удалить выбранный кадр?",
                                            "Окно подтверждения",
                                            MessageBoxButton.OKCancel,
                                            MessageBoxImage.Question);
                if (result == MessageBoxResult.OK)
                {
                    ParentState.Frames.Remove(this);
                }
            }
        }
    }
}
