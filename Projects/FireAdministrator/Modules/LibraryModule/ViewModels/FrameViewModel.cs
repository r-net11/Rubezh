using System.Windows.Controls;
using System.Collections.ObjectModel;
using Infrastructure.Common;
using System.Windows;
using Microsoft.Win32;

namespace LibraryModule.ViewModels
{
    public class FrameViewModel : BaseViewModel
    {
        public FrameViewModel()
        {
            Parent = StateViewModel.Current;
            Layer = 0;

            AddFrameCommand = new RelayCommand(OnAddFrame);
            RemoveFrameCommand = new RelayCommand(OnRemoveFrame);
            ImportSvgCommand = new RelayCommand(OnImportSvg);
        }

        public FrameViewModel(string image, int duration, int layer)
        {
            Parent = StateViewModel.Current;
            Duration = duration;
            Layer = layer;
            Image = image;

            AddFrameCommand = new RelayCommand(OnAddFrame);
            RemoveFrameCommand = new RelayCommand(OnRemoveFrame);
            ImportSvgCommand = new RelayCommand(OnImportSvg);
        }

        public void Initialize(DeviceLibrary.Models.Frame frame)
        {
            Id = frame.Id;
            Image = frame.Image;
            Duration = frame.Duration;
            Layer = frame.Layer;
        }

        public StateViewModel Parent { get; private set; }

        private int _id;
        public int Id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged("Id");
            }
        }

        private int _duration;
        public int Duration
        {
            get { return _duration; }
            set
            {
                _duration = value;
                OnPropertyChanged("Duration");
                LibraryViewModel.Current.Update();
            }
        }

        private ObservableCollection<Canvas> _picture;
        public ObservableCollection<Canvas> Picture
        {
            get { return _picture; }
            set
            {
                _picture = value;
                OnPropertyChanged("Picture");
            }
        }

        private int _layer;
        public int Layer
        {
            get { return _layer; }
            set
            {
                _layer = value;
                OnPropertyChanged("Layer");
                LibraryViewModel.Current.Update();
            }
        }

        private string _image;
        public string Image
        {
            get { return _image; }
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
                    Helper.Draw(Picture, ref canvas, Helper.ErrorFrame, Layer);
                }
                OnPropertyChanged("Image");
                LibraryViewModel.Current.Update();
            }
        }

        public RelayCommand AddFrameCommand { get; private set; }
        private void OnAddFrame()
        {
            var newFrame = new FrameViewModel();
            newFrame.Parent = Parent;
            newFrame.Id = Parent.Frames.Count;
            newFrame.Duration = 300;
            newFrame.Image = Helper.EmptyFrame;
            Parent.Frames.Add(newFrame);
            LibraryViewModel.Current.Update();
        }

        public RelayCommand ImportSvgCommand { get; private set; }
        private void OnImportSvg()
        {
            var openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Text Files (.svg)|*.svg|All Files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.Multiselect = false;
            var userClickedOk = openFileDialog1.ShowDialog();

            if (userClickedOk != true) return;
            var fileStream = openFileDialog1.OpenFile();
            using (var reader = new System.IO.StreamReader(fileStream))
            {
                Image = SvgConverter.Svg2Xaml(reader.ReadToEnd(),Helper.SFileNameXsl);
            }
            fileStream.Close();
        }

        public RelayCommand RemoveFrameCommand { get; private set; }
        private void OnRemoveFrame()
        {
            if (Parent.Frames.Count == 1)
            {
                MessageBox.Show("Невозможно удалить единственный кадр", "Ошибка");
                return;
            }

            var result = MessageBox.Show("Удалить выбранный кадр?",
                "Окно подтверждения", MessageBoxButton.OKCancel,
                MessageBoxImage.Question);
            if (result == MessageBoxResult.Cancel) return;

            Parent.Frames.Remove(this);
            LibraryViewModel.Current.Update();
        }
    }
}
