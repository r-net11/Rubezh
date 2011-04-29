using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Common;
using System.Windows.Controls;
using System.Xml;
using Resources;
using System.IO;
using System.Windows.Markup;
using System.Xml.Serialization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Collections.ObjectModel;

namespace DevicesControl
{
    public class FrameViewModel : BaseViewModel
    {
        public FrameViewModel()
        {
            Layer = 0;
            Current = this;
        }
        public static FrameViewModel Current;
        
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
                    frameImage = Functions.Svg2Xaml(image, Resources.References.svg2xaml_xsl);
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
                    frameImage = Functions.Svg2Xaml(text, Resources.References.svg2xaml_xsl);
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
