using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml;
using Common;
using DeviceLibrary;

namespace DeviceEditor.ViewModels
{
    public class FrameViewModel : BaseViewModel
    {
        private int _id;
        private string _image;
        private int _layer;
        private ObservableCollection<Canvas> _picture;
        private int _duration;
        public FrameViewModel()
        {
            AddFrameCommand = new RelayCommand(OnAddFrameCommand);
            RemoveFrameCommand = new RelayCommand(OnRemoveFrameCommand);
            Parent = StateViewModel.Current;
            Layer = 0;
        }
        public StateViewModel Parent { get; private set; }
        public RelayCommand AddFrameCommand { get; private set; }
        public RelayCommand RemoveFrameCommand { get; private set; }
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
                StringReader stringReader;
                XmlReader xmlReader;
                string frameImage;
                try
                {
                    frameImage = _image;
                    stringReader = new StringReader(frameImage);
                    xmlReader = XmlReader.Create(stringReader);
                    Picture = new ObservableCollection<Canvas>();
                    var framePicture = (Canvas) XamlReader.Load(xmlReader);
                    Panel.SetZIndex(framePicture, Layer);
                    Picture.Add(framePicture);
                }
                catch
                {
                    frameImage = LibraryManager.ErrorFrame;
                    stringReader = new StringReader(frameImage);
                    xmlReader = XmlReader.Create(stringReader);
                    Picture = new ObservableCollection<Canvas>();
                    var framePicture = (Canvas)XamlReader.Load(xmlReader);
                    Panel.SetZIndex(framePicture, Layer);
                    Picture.Add(framePicture);
                }

                OnPropertyChanged("Image");
            }
        }
        private void OnAddFrameCommand(object obj)
        {
            var newFrame = new FrameViewModel();
            Parent.IsChecked = Parent.IsChecked;
            newFrame.Parent = Parent;
            newFrame.Id = Parent.FrameViewModels.Count;
            newFrame.Duration = 500;
            newFrame.Image = LibraryManager.EmptyFrame;
            Parent.FrameViewModels.Add(newFrame);
            ViewModel.Current.SelectedStateViewModel = Parent;
        }
        private void OnRemoveFrameCommand(object obj)
        {
            Parent.FrameViewModels.Remove(this);
        }
    }
}