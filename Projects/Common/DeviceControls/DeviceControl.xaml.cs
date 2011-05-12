using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using DeviceLibrary.Models;

namespace DeviceControls
{
    public partial class DeviceControl : UserControl, INotifyPropertyChanged
    {
        #region Private Fields
        private List<string> _additionalStatesIds;
        private ObservableCollection<Canvas> _stateCanvases;
        private string _stateId;
        #endregion

        public DeviceControl()
        {
            InitializeComponent();
            DataContext = this;
            StateCanvases = new ObservableCollection<Canvas>();
        }

        public bool IsAdditional;

        public string DriverId { get; set; }

        public string StateId
        {
            get { return _stateId; }
            set
            {
                _stateId = value;
                Update();
            }
        }

        public List<string> AdditionalStatesIds
        {
            get { return _additionalStatesIds; }
            set
            {
                _additionalStatesIds = value;
                Update();
            }
        }

        public ObservableCollection<Canvas> StateCanvases
        {
            get { return _stateCanvases; }
            set
            {
                _stateCanvases = value;
                OnPropertyChanged("StateCanvases");
            }
        }

        private List<StateViewModel> StateViewModelList;

        private void Update()
        {
            if (StateViewModelList != null)
                StateViewModelList.ForEach(x => x.Dispose());
            StateViewModelList = new List<StateViewModel>();

            var device = LibraryManager.Devices.FirstOrDefault(x => x.Id == DriverId);
            StateCanvases = new ObservableCollection<Canvas>();
            var state = device.States.FirstOrDefault(x => (x.Id == StateId) && (x.IsAdditional == IsAdditional));
            StateViewModelList.Add(new StateViewModel(state, StateCanvases));
            if (IsAdditional) return;
            if (AdditionalStatesIds == null) return;
            foreach (var additionalStateId in AdditionalStatesIds)
            {
                var aState = device.States.FirstOrDefault(x => (x.Id == additionalStateId) && (x.IsAdditional));
                StateViewModelList.Add(new StateViewModel(aState, StateCanvases));
            }
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private void UserControlSizeChanged(object sender, SizeChangedEventArgs e)
        {
            _itemsControl.LayoutTransform = new ScaleTransform(ActualWidth/500, ActualHeight/500);
        }
    }
}