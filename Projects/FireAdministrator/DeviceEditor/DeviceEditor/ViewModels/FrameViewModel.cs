using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Common;
using System.Windows.Controls;
using System.Xml;
using RubezhDevices;
using System.IO;
using System.Windows.Markup;
using System.Xml.Serialization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Collections.ObjectModel;

namespace DeviceEditor
{
    public class FrameViewModel : BaseViewModel
    {
        public FrameViewModel()
        {
            AddFrameCommand = new RelayCommand(OnAddFrameCommand);
            RemoveFrameCommand = new RelayCommand(OnRemoveFrameCommand);
            Parent = StateViewModel.Current;
            Layer = 0;
            Current = this;
        }
        public static FrameViewModel Current;
        public StateViewModel Parent{get; private set;}

        public RelayCommand AddFrameCommand { get; private set; }
        void OnAddFrameCommand(object obj)
        {
            FrameViewModel newFrame = new FrameViewModel();
            this.Parent.IsChecked = this.Parent.IsChecked;
            newFrame.Parent = this.Parent;           
            newFrame.Id = this.Parent.FrameViewModels.Count;
            newFrame.Duration = 500;
            newFrame.Image = "<svg width=\"500\" height=\"500\" xmlns:xlink=\"http://www.w3.org/1999/xlink\" xmlns=\"http://www.w3.org/2000/svg\">\n<g>\n<title>Layer</title>\n</g>\n</svg>";
            this.Parent.FrameViewModels.Add(newFrame);
            ViewModel.Current.SelectedStateViewModel = this.Parent;
        }

        public RelayCommand RemoveFrameCommand { get; private set; }
        void OnRemoveFrameCommand(object obj)
        {
            this.Parent.FrameViewModels.Remove(this);
        }
        public int id;
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

        ObservableCollection<Canvas> picture;
        public ObservableCollection<Canvas> Picture
        {
            get { return picture; }
            set
            {
                picture = value;
                OnPropertyChanged("Picture");
            }
        }

        ObservableCollection<Canvas> dynamicPicture;
        public ObservableCollection<Canvas> DynamicPicture
        {
            get { return dynamicPicture; }
            set
            {
                dynamicPicture = value;
                OnPropertyChanged("DynamicPicture");
            }
        }

        int layer;
        public int Layer
        {
            get { return layer; }
            set
            {
                layer = value;
                OnPropertyChanged("Layer");
            }
        }

        string image;
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
                    frameImage = Svg2Xaml.XSLT_Transform(image, RubezhDevices.RubezhDevice.svg2xaml_xsl);
                    stringReader = new StringReader(frameImage);
                    xmlReader = XmlReader.Create(stringReader);
                    Picture = new ObservableCollection<Canvas>();
                    Canvas FramePicture = (Canvas)XamlReader.Load(xmlReader);
                    Canvas.SetZIndex(FramePicture, this.Layer);
                    Picture.Add(FramePicture);
                }
                catch(Exception)
                {
                    string text = "<svg width=\"500\" height=\"500\" xmlns:xlink=\"http://www.w3.org/1999/xlink\" xmlns=\"http://www.w3.org/2000/svg\"> <g><title>Layer 1</title> <text xml:space=\"preserve\" text-anchor=\"middle\" font-family=\"serif\" font-size=\"94\" stroke-width=\"0\" stroke=\"#000000\" fill=\"#000000\" id=\"svg_1\" y=\"0\" x=\"0\">"+ "ERROR SVG" +"</text> </g></svg>";
                    frameImage = Svg2Xaml.XSLT_Transform(text, RubezhDevices.RubezhDevice.svg2xaml_xsl);
                    stringReader = new StringReader(frameImage);
                    xmlReader = XmlReader.Create(stringReader);
                    Picture = new ObservableCollection<Canvas>();
                    Canvas FramePicture = (Canvas)XamlReader.Load(xmlReader);
                    Canvas.SetZIndex(FramePicture, this.Layer);
                    Picture.Add(FramePicture);
                }
                OnPropertyChanged("Image");
            }
        }
    }
}
