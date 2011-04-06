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

namespace DeviceEditor
{
    public class FrameViewModel : BaseViewModel
    {
        public FrameViewModel()
        {
            AddFrameCommand = new RelayCommand(OnAddFrameCommand);
            RemoveFrameCommand = new RelayCommand(OnRemoveFrameCommand);
            Parent = StateViewModel.Current;
            Current = this;
        }

        public static FrameViewModel Current;
        public StateViewModel Parent{get; private set;}

        public RelayCommand AddFrameCommand { get; private set; }
        void OnAddFrameCommand(object obj)
        {
            FrameViewModel newFrame = new FrameViewModel();
            newFrame.Parent = this.Parent;
            newFrame.Id = this.Parent.FrameViewModels.Count;
            newFrame.Duration = 300;
            newFrame.Image = "<svg width=\"500\" height=\"500\" xmlns:xlink=\"http://www.w3.org/1999/xlink\" xmlns=\"http://www.w3.org/2000/svg\"> <g><title>Layer 1</title></g></svg>";
            this.Parent.FrameViewModels.Add(newFrame);
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
        Canvas readerLoadButton;
        public Canvas ReaderLoadButton
        {
            get { return readerLoadButton; }
            set
            {
                readerLoadButton = value;
                OnPropertyChanged("ReaderLoadButton");
            }
        }
        string image;
        public string Image
        {
            get { return image; }
            set
            {
                image = value;
                string frameImage = Svg2Xaml.XSLT_Transform(image, RubezhDevices.RubezhDevice.svg2xaml_xsl);
                StringReader stringReader;
                XmlReader xmlReader;

                try
                {
                    stringReader = new StringReader(frameImage);
                    xmlReader = XmlReader.Create(stringReader);
                    ReaderLoadButton = (Canvas)XamlReader.Load(xmlReader);
                    OnPropertyChanged("Image");
                }
                catch(Exception)
                {
                    string text = "<svg width=\"500\" height=\"500\" xmlns:xlink=\"http://www.w3.org/1999/xlink\" xmlns=\"http://www.w3.org/2000/svg\"> <g><title>Layer 1</title> <text xml:space=\"preserve\" text-anchor=\"middle\" font-family=\"serif\" font-size=\"94\" stroke-width=\"0\" stroke=\"#000000\" fill=\"#000000\" id=\"svg_1\" y=\"0\" x=\"0\">"+ "ERROR SVG" +"</text> </g></svg>";
                    frameImage = Svg2Xaml.XSLT_Transform(text, RubezhDevices.RubezhDevice.svg2xaml_xsl);
                    stringReader = new StringReader(frameImage);
                    xmlReader = XmlReader.Create(stringReader);
                    ReaderLoadButton = (Canvas)XamlReader.Load(xmlReader);
                    
                }
            }
        }
    }
}
