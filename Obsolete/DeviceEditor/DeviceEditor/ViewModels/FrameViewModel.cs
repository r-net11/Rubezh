using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Common;

namespace DeviceEditor.ViewModels
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

        public StateViewModel Parent { get; private set; }

        public RelayCommand AddFrameCommand { get; private set; }
        private void OnAddFrame(object obj)
        {
            var newFrame = new FrameViewModel();
            Parent.IsChecked = Parent.IsChecked;
            newFrame.Parent = Parent;
            newFrame.Id = Parent.FrameViewModels.Count;
            newFrame.Duration = 300;
            newFrame.Image = Helper.EmptyFrame;
            Parent.FrameViewModels.Add(newFrame);
            ViewModel.Current.SelectedStateViewModel = Parent;
            ViewModel.Current.Update();
        }

        public RelayCommand RemoveFrameCommand { get; private set; }
        private void OnRemoveFrame(object obj)
        {
            if (Parent.FrameViewModels.Count == 1)
            {
                MessageBox.Show("Невозможно удалить единственный кадр", "Ошибка");
                return;
            }
            Parent.FrameViewModels.Remove(this);
            ViewModel.Current.Update();
        }

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
                ViewModel.Current.Update();
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
                ViewModel.Current.Update();
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
                ViewModel.Current.Update();
            }
        }
    }
}