using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using DeviceLibrary;

namespace DeviceControls
{
    public partial class DeviceControl : INotifyPropertyChanged
    {
        public DeviceControl(string driverId)
        {
            InitializeComponent();

            DataContext = this;
            Device = LibraryManager.Devices.FirstOrDefault(x => x.Id == driverId);
            StateCanvases = new ObservableCollection<Canvas>();
            AdditionalStates = new List<string>();
        }

        public DeviceLibrary.Models.Device Device { get; private set; }

        public string StateId { get; set; }

        List<string> _additionalStates;
        public List<string> AdditionalStates
        {
            get
            {
                return _additionalStates;
            }

            set
            {
                _additionalStates = value;
                Update();
            }
        }

        ObservableCollection<Canvas> _stateCanvases;
        public ObservableCollection<Canvas> StateCanvases
        {
            get
            {
                return _stateCanvases;
            }

            set
            {
                _stateCanvases = value;
                OnPropertyChanged("StateCanvases");
            }
        }

        List<StateViewModel> _stateViewModelList;

        void Update()
        {
            if (_stateViewModelList != null)
                _stateViewModelList.ForEach(x => x.Dispose());
            _stateViewModelList = new List<StateViewModel>();

            StateCanvases = new ObservableCollection<Canvas>();
            var state = Device.States.FirstOrDefault(x => (x.Id == StateId) && (!x.IsAdditional));
            if (state != null)
            {
                _stateViewModelList.Add(new StateViewModel(state, StateCanvases));
            }

            if (AdditionalStates != null)
            {
                foreach (var additionalStateId in AdditionalStates)
                {
                    var aState = Device.States.FirstOrDefault(x => (x.Id == additionalStateId) && (x.IsAdditional));
                    if (aState != null)
                    {
                        _stateViewModelList.Add(new StateViewModel(aState, StateCanvases));
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}