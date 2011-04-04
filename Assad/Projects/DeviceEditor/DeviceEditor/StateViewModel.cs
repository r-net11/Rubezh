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
            Parent = DeviceViewModel.Current;
            AddStateCommand = new RelayCommand(OnAddStateCommand);
            RemoveStateCommand = new RelayCommand(OnRemoveStateCommand);
        }
        public DeviceViewModel Parent { get; private set; }

        public RelayCommand AddStateCommand { get; private set; }
        void OnAddStateCommand(object obj)
        {
            StateViewModel newState = new StateViewModel();
            newState.Parent = this.Parent;
            newState.Id = "Новое состояние";
            CadrViewModel newCadr = new CadrViewModel();
            newCadr.Duration = 300;
            newCadr.Image = "<svg width=\"500\" height=\"500\" xmlns:xlink=\"http://www.w3.org/1999/xlink\" xmlns=\"http://www.w3.org/2000/svg\"> <g><title>Layer 1</title></g></svg>";
            newState.CadrViewModels = new ObservableCollection<CadrViewModel>();
            newState.CadrViewModels.Add(newCadr);
            this.Parent.StateViewModels.Insert(this.Parent.StateViewModels.IndexOf(this) + 1, newState);
        }

        public RelayCommand RemoveStateCommand { get; private set; }
        void OnRemoveStateCommand(object obj)
        {
            this.Parent.StateViewModels.Remove(this);
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
