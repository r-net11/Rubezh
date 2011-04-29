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
using Frame = DeviceLibrary.Frame;
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
        }

        string _driverId;
        public string DriverId
        {
            get { return _driverId; }
            set
            {
                _driverId = value;
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

        List<string> _additionalStates;
        public List<string> AdditionalStates
        {
            get { return _additionalStates; }
            set
            {
                _additionalStates = value;

                Update();
            }
        }

        void Update()
        {
            StateCanvases = new ObservableCollection<Canvas>();

            Device device = LibraryManager.Devices.FirstOrDefault(x => x.Id == DriverId);
            State state = device.States.FirstOrDefault(x => x.Id == StateId);
            new StateViewModel(state, StateCanvases);

            if (AdditionalStates != null)
                foreach (string additionalState in AdditionalStates)
                {
                    State a_state = device.States.FirstOrDefault(x => x.Id == additionalState);

                    new StateViewModel(a_state, StateCanvases);
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
