using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Common;
using FiresecAPI.Models;
using FiresecClient;

namespace DeviceControls
{
    public partial class DeviceControl : INotifyPropertyChanged
    {
        public DeviceControl()
        {
            InitializeComponent();

            DataContext = this;
            StateCanvases = new ObservableCollection<Canvas>();
            AdditionalStateCodes = new List<string>();
        }

        public string DriverId { get; set; }

        StateType _stateType;
        public StateType StateType
        {
            get { return _stateType; }
            set
            {
                _stateType = value;
                Update();
            }
        }

        List<string> _additionalStateCodes;
        public List<string> AdditionalStateCodes
        {
            get { return _additionalStateCodes; }
            set
            {
                _additionalStateCodes = value;

                if (_additionalStateCodes.IsNotNullOrEmpty())
                    Update();
            }
        }

        ObservableCollection<Canvas> _stateCanvases;
        public ObservableCollection<Canvas> StateCanvases
        {
            get { return _stateCanvases; }
            private set
            {
                _stateCanvases = value;
                OnPropertyChanged("StateCanvases");
            }
        }

        List<StateViewModel> _stateViewModelList;

        void Update()
        {
            if (_stateViewModelList.IsNotNullOrEmpty())
                _stateViewModelList.ForEach(x => x.Dispose());
            _stateViewModelList = new List<StateViewModel>();

            var device = FiresecManager.DeviceLibraryConfiguration.Devices.FirstOrDefault(x => x.Id == DriverId);
            if (device == null)
                return;

            StateCanvases = new ObservableCollection<Canvas>();
            var state = device.States.FirstOrDefault(x => x.Code == null && x.StateType == StateType);
            if (state != null)
                _stateViewModelList.Add(new StateViewModel(state, StateCanvases));

            if (AdditionalStateCodes.IsNotNullOrEmpty())
                foreach (var additionalStateCode in AdditionalStateCodes)
                {
                    var aState = device.States.FirstOrDefault(x => x.Code == additionalStateCode);
                    if (aState != null)
                        _stateViewModelList.Add(new StateViewModel(aState, StateCanvases));
                }
        }

        void UserControlSizeChanged(object sender, SizeChangedEventArgs e)
        {
            _itemsControl.LayoutTransform = new ScaleTransform(ActualWidth / 500, ActualHeight / 500);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}