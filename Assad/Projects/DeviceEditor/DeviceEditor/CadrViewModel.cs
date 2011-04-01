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

namespace DeviceEditor
{
    public class CadrViewModel : BaseViewModel
    {
        public CadrViewModel()
        {

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
        
        string image;
        public string Image
        {
            get { return image; }
            set
            {
                image = value;
                try
                {
                    string cadrImage = Svg2Xaml.XSLT_Transform(image, RubezhDevices.RubezhDevice.svg2xaml_xsl);
                    StringReader stringReader = new StringReader(cadrImage);
                    XmlReader xmlReader = XmlReader.Create(stringReader);
                    StateViewModel.Current.ReaderLoadButton = (Canvas)XamlReader.Load(xmlReader);
                    OnPropertyChanged("Image");
                }
                catch
                { }

            }
        }
    }
}
