using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Common;
using System.Windows.Threading;
using RubezhDevices;
using System.Windows.Input;
using System.IO;
using System.Xml;
using System.Windows.Controls;
using System.Windows.Markup;

namespace DeviceEditor
{
    public class DeviceViewModel : BaseViewModel
    {
        public DeviceViewModel()
        {
            Current = this;
            Parent = ViewModel.Current;
            AddDeviceCommand = new RelayCommand(OnAddDeviceCommand);
            RemoveDeviceCommand = new RelayCommand(OnRemoveDeviceCommand);
        }

        public ViewModel Parent { get; private set; }
        public static DeviceViewModel Current { get; private set; }

        public RelayCommand AddDeviceCommand { get; private set; }
        void OnAddDeviceCommand(object obj)
        {
            DeviceViewModel newDevice = new DeviceViewModel();
            newDevice.Parent = this.Parent;
            newDevice.Id = "Новое устройство";
            StateViewModel newState = new StateViewModel();
            newState.Id = "Новое состояние";
            FrameViewModel newFrame = new FrameViewModel();
            newFrame.Duration = 300;
            newFrame.Image = "<svg width=\"500\" height=\"500\" xmlns:xlink=\"http://www.w3.org/1999/xlink\" xmlns=\"http://www.w3.org/2000/svg\"> <g><title>Layer 1</title></g></svg>";
            newDevice.StateViewModels = new ObservableCollection<StateViewModel>();
            newState.FrameViewModels = new ObservableCollection<FrameViewModel>();
            newState.FrameViewModels.Add(newFrame);
            newDevice.StateViewModels.Add(newState);
            this.Parent.DeviceViewModels.Insert(this.Parent.DeviceViewModels.IndexOf(this) + 1, newDevice);
        }

        public RelayCommand RemoveDeviceCommand { get; private set; }
        void OnRemoveDeviceCommand(object obj)
        {
            this.Parent.DeviceViewModels.Remove(this);
        }

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

        StateViewModel selectedStateViewModel;
        public StateViewModel SelectedStateViewModel
        {
            get { return selectedStateViewModel; }
            set
            {
                selectedStateViewModel = value;
                ViewModel.Current.SelectedStateViewModel = selectedStateViewModel;
                OnPropertyChanged("SelectedStateViewModel");
            }
        }
        public ObservableCollection<StateViewModel> stateViewModels;
        public ObservableCollection<StateViewModel> StateViewModels
        {
            get { return stateViewModels; }
            set
            {
                stateViewModels = value;
                OnPropertyChanged("StateViewModels");
            }
        }
    }
}
