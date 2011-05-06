using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml;
using Common;
using DeviceLibrary;

namespace DeviceEditor
{
    public class FrameViewModel : BaseViewModel
    {
        public static FrameViewModel Current;
        private ObservableCollection<Canvas> dynamicPicture;
        public int id;
        private string image;
        private int layer;
        private ObservableCollection<Canvas> picture;

        public FrameViewModel()
        {
            AddFrameCommand = new RelayCommand(OnAddFrameCommand);
            RemoveFrameCommand = new RelayCommand(OnRemoveFrameCommand);
            Parent = StateViewModel.Current;
            Layer = 0;
            Current = this;
        }

        public StateViewModel Parent { get; private set; }

        public RelayCommand AddFrameCommand { get; private set; }

        public RelayCommand RemoveFrameCommand { get; private set; }

        public int Id
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged("Id");
            }
        }

        public int duration { get; set; }

        public int Duration
        {
            get { return duration; }
            set
            {
                duration = value;
                OnPropertyChanged("Duration");
            }
        }

        public ObservableCollection<Canvas> Picture
        {
            get { return picture; }
            set
            {
                picture = value;
                OnPropertyChanged("Picture");
            }
        }

        public ObservableCollection<Canvas> DynamicPicture
        {
            get { return dynamicPicture; }
            set
            {
                dynamicPicture = value;
                OnPropertyChanged("DynamicPicture");
            }
        }

        public int Layer
        {
            get { return layer; }
            set
            {
                layer = value;
                OnPropertyChanged("Layer");
            }
        }

        public string Image
        {
            get { return image; }
            set
            {
                image = value;
                string frameImage;
                StringReader stringReader;
                XmlReader xmlReader;

                try
                {
                    frameImage = image;
                    stringReader = new StringReader(frameImage);
                    xmlReader = XmlReader.Create(stringReader);
                    Picture = new ObservableCollection<Canvas>();
                    var FramePicture = (Canvas) XamlReader.Load(xmlReader);
                    Panel.SetZIndex(FramePicture, Layer);
                    Picture.Add(FramePicture);
                }
                catch (Exception)
                {
                    string text =
                        "<svg width=\"500\" height=\"500\" xmlns:xlink=\"http://www.w3.org/1999/xlink\" xmlns=\"http://www.w3.org/2000/svg\"> <g><title>Layer 1</title> <text xml:space=\"preserve\" text-anchor=\"middle\" font-family=\"serif\" font-size=\"94\" stroke-width=\"0\" stroke=\"#000000\" fill=\"#000000\" id=\"svg_1\" y=\"0\" x=\"0\">" +
                        "ERROR SVG" + "</text> </g></svg>";
                    frameImage = text;
                    stringReader = new StringReader(frameImage);
                    xmlReader = XmlReader.Create(stringReader);
                    Picture = new ObservableCollection<Canvas>();
                    var FramePicture = (Canvas) XamlReader.Load(xmlReader);
                    Panel.SetZIndex(FramePicture, Layer);
                    Picture.Add(FramePicture);
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
            newFrame.Image =
                "<svg width=\"500\" height=\"500\" xmlns:xlink=\"http://www.w3.org/1999/xlink\" xmlns=\"http://www.w3.org/2000/svg\">\n<g>\n<title>Layer</title>\n</g>\n</svg>";
            Parent.FrameViewModels.Add(newFrame);
            ViewModel.Current.SelectedStateViewModel = Parent;
        }

        private void OnRemoveFrameCommand(object obj)
        {
            Parent.FrameViewModels.Remove(this);
        }
    }
}