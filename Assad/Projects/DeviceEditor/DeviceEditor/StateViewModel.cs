using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Common;
using System.Windows.Controls;
using RubezhDevices;
using System.IO;
using System.Xml;
using System.Windows.Markup;
using System.Windows.Input;

namespace DeviceEditor
{
    public class StateViewModel : BaseViewModel
    {

        public StateViewModel()
        {
            Current = this;
        }


        public static StateViewModel Current { get; private set; }
        public string id;
        public string Id
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged("Id");
            }
        }

        public ObservableCollection<CadrViewModel> cadrViewModels;
        public ObservableCollection<CadrViewModel> CadrViewModels
        {
            get { return cadrViewModels; }
            set
            {
                cadrViewModels = value;
                OnPropertyChanged("CadrViewModels");
            }
        }

        CadrViewModel selectedCadrViewModel;
        public CadrViewModel SelectedCadrViewModel
        {
            get { return selectedCadrViewModel; }
            set
            {
                selectedCadrViewModel = value;
                if (selectedCadrViewModel == null)
                    return;
                string cadrImage = Svg2Xaml.XSLT_Transform(selectedCadrViewModel.Image, RubezhDevices.RubezhDevice.svg2xaml_xsl);
                StringReader stringReader = new StringReader(cadrImage);
                XmlReader xmlReader = XmlReader.Create(stringReader);
                ReaderLoadButton = (Canvas)XamlReader.Load(xmlReader);
                ViewModel.Current.SelectedCadrViewModel = selectedCadrViewModel;
                OnPropertyChanged("SelectedCadrViewModel");
            }
        }

        Canvas readerLoadButton;
        public Canvas ReaderLoadButton
        {
            get { return readerLoadButton; }
            set
            {
                readerLoadButton = value;
                ViewModel.Current.ReaderLoadButton = readerLoadButton;
                OnPropertyChanged("ReaderLoadButton");
            }
        }

    }
}
