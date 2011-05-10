using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Firesec;
using DeviceLibrary;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.IO;
using System.Xml;
using Frame = DeviceLibrary.Models.Frame;
using System.Windows.Markup;
using System.ComponentModel;
using System.Diagnostics;

namespace DeviceControls
{
    public partial class DeviceControl : UserControl, INotifyPropertyChanged
    {
        public DeviceControl()
        {
            InitializeComponent();
            this.DataContext = this;
            StateCanvases = new ObservableCollection<Canvas>();
            StatesViewModels = new List<StateViewModel>();
        }

        string _driverId;
        public string DriverId
        {
            get { return _driverId; }
            set
            {
                _driverId = value;
                CurrentDevice = LibraryManager.Devices.FirstOrDefault(x => x.Id == value);
            }
        }

        string _stateId;
        public string StateId
        {
            get { return _stateId; }
            set
            {
                _stateId = value;
                
                Update();
            }
        }

        List<string> _additionalStatesIds;
        public List<string> AdditionalStatesIds
        {
            get { return _additionalStatesIds; }
            set
            {
                _additionalStatesIds = value;

                Update();
            }
        }

        private List<StateViewModel> StatesViewModels { get; set; }

        private ObservableCollection<StateViewModel> additionalStatesViewModels;
        ObservableCollection<StateViewModel> AdditionalStatesViewModels
        {
            get { return additionalStatesViewModels; }
            set
            {
                additionalStatesViewModels = value;
            }
        }

        public bool IsAdditional;
        public static Device CurrentDevice;

        void Update()
        {
            if (StatesViewModels != null)
                StatesViewModels.ForEach(x => x.Dispose());
            StateCanvases = new ObservableCollection<Canvas>();

            Device device = LibraryManager.Devices.FirstOrDefault(x => x.Id == DriverId);
            State state = device.States.FirstOrDefault(x => (x.Id == StateId) && (x.IsAdditional == IsAdditional));
            
            StatesViewModels.Add(new StateViewModel(state, StateCanvases));
            if (IsAdditional)
                return;
            if (AdditionalStatesIds != null)
                foreach (string additionalState in AdditionalStatesIds)
                {
                    State a_state = CurrentDevice.States.FirstOrDefault(x => (x.Id == additionalState) && (x.IsAdditional));
                    StatesViewModels.Add(new StateViewModel(a_state, StateCanvases));
                }
        }

        ObservableCollection<Canvas> _stateCanvases;
        public ObservableCollection<Canvas> StateCanvases
        {
            get { return _stateCanvases; }
            set
            {
                _stateCanvases = value;
                OnPropertyChanged("StateCanvases");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _itemsControl.LayoutTransform = new ScaleTransform((double)ActualWidth / 500, (double)ActualHeight / 500);
        }
    }
}
