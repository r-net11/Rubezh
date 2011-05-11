using System.Collections.ObjectModel;
using System.Windows.Controls;
using Common;

namespace DeviceEditor.ViewModels
{
    public class FrameViewModel : BaseViewModel
    {
        #region Private Fields
        private int _id;
        private string _image;
        private int _layer;
        private ObservableCollection<Canvas> _picture;
        private int _duration;
        #endregion

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
            newFrame.Duration = 500;
            newFrame.Image = Helper.EmptyFrame;
            Parent.FrameViewModels.Add(newFrame);
            ViewModel.Current.SelectedStateViewModel = Parent;
        }
        public RelayCommand RemoveFrameCommand { get; private set; }
        private void OnRemoveFrame(object obj)
        {
            Parent.FrameViewModels.Remove(this);
        }
        /// <summary>
        /// Идентификатор кадра.
        /// </summary>
        public int Id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged("Id");
            }
        }
        /// <summary>
        /// Длительность кадра.
        /// </summary>
        public int Duration
        {
            get { return _duration; }
            set
            {
                _duration = value;
                OnPropertyChanged("Duration");
            }
        }
        /// <summary>
        /// Рисунок кадра.
        /// </summary>
        public ObservableCollection<Canvas> Picture
        {
            get { return _picture; }
            set
            {
                _picture = value;
                OnPropertyChanged("Picture");
            }
        }
        /// <summary>
        /// Слой кадра.
        /// </summary>
        public int Layer
        {
            get { return _layer; }
            set
            {
                _layer = value;
                OnPropertyChanged("Layer");
            }
        }

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
                    DeviceLibrary.Helper.Draw(Picture, ref canvas, value, Layer);
                }
                catch
                {
                    DeviceLibrary.Helper.Draw(Picture, ref canvas, Helper.ErrorFrame, Layer);
                }
                OnPropertyChanged("Image");
            }
        }
    }
}