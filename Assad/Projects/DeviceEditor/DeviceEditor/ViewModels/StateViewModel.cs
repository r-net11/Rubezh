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


        FrameViewModel selectedFrameViewModel;
        public FrameViewModel SelectedFrameViewModel
        {
            get { return selectedFrameViewModel; }
            set
            {
                selectedFrameViewModel = value;
                if (selectedFrameViewModel == null)
                    return;
                string frameImage = Svg2Xaml.XSLT_Transform(selectedFrameViewModel.Image, RubezhDevices.RubezhDevice.svg2xaml_xsl);
                StringReader stringReader = new StringReader(frameImage);
                XmlReader xmlReader = XmlReader.Create(stringReader);
                FrameViewModel.Current.ReaderLoadButton = (Canvas)XamlReader.Load(xmlReader);
                OnPropertyChanged("SelectedFrameViewModel");
            }
        }

        public RelayCommand AddStateCommand { get; private set; }
        void OnAddStateCommand(object obj)
        {
            StateViewModel newState = new StateViewModel();
            newState.Parent = this.Parent;
            newState.Id = "Новое состояние";
            FrameViewModel newFrame = new FrameViewModel();
            newFrame.Duration = 300;
            newFrame.Image = "<svg width=\"500\" height=\"500\" xmlns:xlink=\"http://www.w3.org/1999/xlink\" xmlns=\"http://www.w3.org/2000/svg\"> <g><title>Layer 1</title></g></svg>";
            newState.FrameViewModels = new ObservableCollection<FrameViewModel>();
            newState.FrameViewModels.Add(newFrame);
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

        public ObservableCollection<FrameViewModel> frameViewModels;
        public ObservableCollection<FrameViewModel> FrameViewModels
        {
            get { return frameViewModels; }
            set
            {
                frameViewModels = value;
                OnPropertyChanged("FrameViewModels");
            }
        }

    }
}
