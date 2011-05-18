using System.Windows.Controls;
using System.Collections.ObjectModel;
using Infrastructure.Common;
using System.Windows;

namespace LibraryModule.ViewModels
{
    public class FrameViewModel : BaseViewModel
    {
        public FrameViewModel()
        {
            AddFrameCommand = new RelayCommand(OnAddFrame);
            RemoveFrameCommand = new RelayCommand(OnRemoveFrame);
            Parent = StateViewModel.Current;
            Layer = 0;
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
            Parent.IsChecked = Parent.IsChecked;
            newFrame.Parent = Parent;
            newFrame.Id = Parent.Frames.Count;
            newFrame.Duration = 300;
            newFrame.Image = Helper.EmptyFrame;
            Parent.Frames.Add(newFrame);
            LibraryViewModel.Current.SelectedState = Parent;
            LibraryViewModel.Current.Update();
        }

        public RelayCommand RemoveFrameCommand { get; private set; }
        private void OnRemoveFrame()
        {
            if (Parent.Frames.Count == 1)
            {
                MessageBox.Show("Невозможно удалить единственный кадр", "Ошибка");
                return;
            }
            Parent.Frames.Remove(this);
            LibraryViewModel.Current.Update();
        }
    }
}
