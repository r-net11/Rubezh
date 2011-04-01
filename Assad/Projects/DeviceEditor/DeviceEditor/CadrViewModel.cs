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
            SaveCommand = new RelayCommand(OnSaveCommand);
        }

        public RelayCommand SaveCommand { get; private set; }
        void OnSaveCommand(object obj)
        {
            FileStream filexml = new FileStream(ViewModel.deviceLibrary_xml, FileMode.Open, FileAccess.Write, FileShare.Write);
            XmlSerializer serializer = new XmlSerializer(typeof(Cadr));
            serializer.Serialize(filexml, this);
            filexml.Close();
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

        public int Duration { get; set; }
        
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

        //bool isSelected;
        //public bool IsSelected
        //{
        //    get { return isSelected; }
        //    set
        //    {
        //        isSelected = value;
        //        OnPropertyChanged("IsSelected");
        //        //ViewModel.Current.SelectedCadrViewModel = this;
        //    }
        //}
    }
}
