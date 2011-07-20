using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Infrastructure.Common;
using Microsoft.Win32;

namespace LibraryModule.ViewModels
{
    public class FrameViewModel : BaseViewModel
    {
        const string emptyFrame = "<?xml version=\"1.0\" encoding=\"utf-16\"?>\n<Canvas Width=\"500\" Height=\"500\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">\n</Canvas>";
        const string errorFrame = "<?xml version=\"1.0\" encoding=\"utf-16\"?>\n<Canvas Width=\"500\" Height=\"500\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">\n<Border BorderBrush=\"Red\" BorderThickness=\"5\" Padding=\"20\">\n<TextBlock Text=\"Error Xaml Code\" FontSize=\"60\" />\n</Border>\n</Canvas>";
        const int defaultDuration = 300;
        const int defaultLayer = 0;

        public FrameViewModel(StateViewModel parent, DeviceLibrary.Models.Frame frame)
        {
            Parent = parent;
            Id = frame.Id;
            Image = frame.Image;
            Duration = frame.Duration;
            Layer = frame.Layer;

            Initialize();
        }

        public FrameViewModel(StateViewModel parent)
        {
            Parent = parent;
            Id = parent.Frames.Count;
            Image = emptyFrame;
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

        public StateViewModel Parent { get; private set; }

        int _id;
        public int Id
        {
            get
            {
                return _id;
            }

            set
            {
                _id = value;
                OnPropertyChanged("Id");
            }
        }

        int _duration;
        public int Duration
        {
            get
            {
                return _duration;
            }

            set
            {
                _duration = value;
                OnPropertyChanged("Duration");
            }
        }

        ObservableCollection<Canvas> _picture;
        public ObservableCollection<Canvas> Picture
        {
            get
            {
                return _picture;
            }

            set
            {
                _picture = value;
                OnPropertyChanged("Picture");
            }
        }

        int _layer;
        public int Layer
        {
            get
            {
                return _layer;
            }

            set
            {
                _layer = value;
                OnPropertyChanged("Layer");
            }
        }

        string _image;
        public string Image
        {
            get
            {
                return _image;
            }

            set
            {
                _image = value;
                Picture = new ObservableCollection<Canvas>();
                var canvas = new Canvas();
                try
                {
                    Helper.Draw(Picture, ref canvas, value, Layer);
                }
                catch
                {
                    Helper.Draw(Picture, ref canvas, errorFrame, Layer);
                }
                OnPropertyChanged("Image");
            }
        }

        public RelayCommand AddFrameCommand { get; private set; }
        void OnAddFrame()
        {
            var newFrame = new FrameViewModel(Parent);
            newFrame.Parent = Parent;
            Parent.Frames.Add(newFrame);
        }

        public RelayCommand ImportSvgCommand { get; private set; }
        void OnImportSvg()
        {
            var openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Text Files (.svg)|*.svg";

            if (openFileDialog1.ShowDialog() == true)
            {
                Image = SvgConverter.Svg2Xaml(openFileDialog1.FileName, PathHelper.TransormFileName);
            }
        }

        public RelayCommand RemoveFrameCommand { get; private set; }
        void OnRemoveFrame()
        {
            if (Parent.Frames.Count == 1)
            {
                MessageBox.Show("Невозможно удалить единственный кадр", "Ошибка");
                return;
            }

            var result = MessageBox.Show("Удалить выбранный кадр?",
                "Окно подтверждения", MessageBoxButton.OKCancel,
                MessageBoxImage.Question);
            if (result == MessageBoxResult.OK)
            {
                Parent.Frames.Remove(this);
            }
        }
    }
}
