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

namespace DeviceEditor
{
    public class CadrViewModel : BaseViewModel
    {
        public CadrViewModel()
        {
            AddCadrCommand = new RelayCommand(OnAddCadrCommand);
            RemoveCadrCommand = new RelayCommand(OnRemoveCadrCommand);
            Parent = StateViewModel.Current;
        }

        public StateViewModel Parent{get; private set;}

        public RelayCommand AddCadrCommand { get; private set; }
        void OnAddCadrCommand(object obj)
        {
            CadrViewModel newCadr = new CadrViewModel();
            newCadr.Parent = this.Parent;
            newCadr.Id = this.Parent.CadrViewModels.Count;
            newCadr.Duration = 300;
            newCadr.Image = "<svg width=\"500\" height=\"500\" xmlns:xlink=\"http://www.w3.org/1999/xlink\" xmlns=\"http://www.w3.org/2000/svg\"> <g><title>Layer 1</title></g></svg>";
            this.Parent.CadrViewModels.Add(newCadr);
        }

        public RelayCommand RemoveCadrCommand { get; private set; }
        void OnRemoveCadrCommand(object obj)
        {
            this.Parent.CadrViewModels.Remove(this);
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
                string cadrImage = Svg2Xaml.XSLT_Transform(image, RubezhDevices.RubezhDevice.svg2xaml_xsl);
                StringReader stringReader;
                XmlReader xmlReader;

                try
                {
                    stringReader = new StringReader(cadrImage);
                    xmlReader = XmlReader.Create(stringReader);
                    StateViewModel.Current.ReaderLoadButton = (Canvas)XamlReader.Load(xmlReader);
                    OnPropertyChanged("Image");
                }
                catch(Exception)
                {
                    string text = "<svg width=\"500\" height=\"500\" xmlns:xlink=\"http://www.w3.org/1999/xlink\" xmlns=\"http://www.w3.org/2000/svg\"> <g><title>Layer 1</title> <text xml:space=\"preserve\" text-anchor=\"middle\" font-family=\"serif\" font-size=\"94\" stroke-width=\"0\" stroke=\"#000000\" fill=\"#000000\" id=\"svg_1\" y=\"0\" x=\"0\">"+ "ERROR SVG" +"</text> </g></svg>";
                    cadrImage = Svg2Xaml.XSLT_Transform(text, RubezhDevices.RubezhDevice.svg2xaml_xsl);
                    stringReader = new StringReader(cadrImage);
                    xmlReader = XmlReader.Create(stringReader);
                    StateViewModel.Current.ReaderLoadButton = (Canvas)XamlReader.Load(xmlReader);
                    
                }
            }
        }

    }
}
