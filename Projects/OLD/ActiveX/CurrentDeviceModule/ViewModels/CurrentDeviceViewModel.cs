using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows;
using CurrentDeviceModule.Views;
using DeviceControls;
using FiresecClient;
using System.Windows;
using DevicesModule.ViewModels;
using DevicesModule.Views;
using FiresecAPI.Models;
using System.Windows.Resources;

namespace CurrentDeviceModule.ViewModels
{
    public class CurrentDeviceViewModel : BaseViewModel
    {
        public CurrentDeviceViewModel()
        {
            FiresecEventSubscriber.DeviceStateChangedEvent += new Action<Guid>(OnDeviceStateChanged);
            CurrentDeviceControl = new DeviceControl();
            CurrentDevice = new Device();
            IsCurrentDeviceSelected = false;
            ShowPropertiesCommand = new RelayCommand(OnShowProperties);
        }

        public void Inicialize(Guid deviceId)
        {
            DeviceId = deviceId;
            CurrentDevice = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == DeviceId);
            CurrentDeviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.UID == DeviceId);
            CurrentDeviceControl.DriverId = DriverId;
            Update();
            IsCurrentDeviceSelected = true;
        }

        DeviceControl _currentDeviceControl;
        public DeviceControl CurrentDeviceControl
        {
            get { return _currentDeviceControl; }
            set
            {
                _currentDeviceControl = value;
            }
        }

        public Device CurrentDevice { get; private set; }
        public DeviceState CurrentDeviceState { get; private set; }
        public Guid DeviceId { get; set; }

        public StateType StateType
        {
            get { return CurrentDeviceState.StateType; }
        }

        public Guid DriverId
        {
            get { return CurrentDevice.Driver.UID; }
        }

        string _toolTip;
        public string ToolTip
        {
            get { return _toolTip; }
            set
            {
                _toolTip = value;
                OnPropertyChanged("ToolTip");
            }
        }

        bool _isCurrentDeviceSelected;
        public bool IsCurrentDeviceSelected
        {
            get { return _isCurrentDeviceSelected; }
            set { _isCurrentDeviceSelected = value; }
        }

        public void SelectDevice()
        {
            DeviceTreeViewModel newDeviceTreeViewModel = new DeviceTreeViewModel();
            newDeviceTreeViewModel.Initialize();
            DeviceTreeView newDeviceTreeView = new DeviceTreeView();
            newDeviceTreeView.DataContext = newDeviceTreeViewModel;
            newDeviceTreeViewModel.Closing += newDeviceTreeView.Close;

            newDeviceTreeView.ShowDialog();

            if (newDeviceTreeViewModel.DeviceId != Guid.Empty)
            {
                DeviceId = newDeviceTreeViewModel.DeviceId;
                Inicialize(DeviceId);
            }
        }

        public void UpdateToolTip()
        {
            string str = "";
            str = CurrentDevice.PresentationAddress + " - " + CurrentDevice.Driver.ShortName + "\n";

            if (CurrentDeviceState.ParentStringStates != null)
                foreach (var parentState in CurrentDeviceState.ParentStringStates)
                {
                    str += parentState + "\n";
                }

            if (CurrentDeviceState.Parameters != null)
                foreach (var parameter in CurrentDeviceState.Parameters)
                {
                    if (parameter.Visible)
                    {
                        if (string.IsNullOrEmpty(parameter.Value))
                            continue;
                        if (parameter.Value == "<NULL>")
                            continue;
                        str += parameter.Caption + " - " + parameter.Value + "\n";
                    }
                }
            ToolTip = str;
        }

        void Update()
        {
            UpdateToolTip();
            CurrentDeviceControl.StateType = StateType;
        }

        void OnDeviceStateChanged(Guid id)
        {
            if (DeviceId == id)
            {
                Update();
            }
        }

        public RelayCommand ShowPropertiesCommand { get; private set; }
        public void OnShowProperties()
        {            
            var deviceDetailsView = new CurrentDeviceDetailsView();
            var deviceDetailsViewModel = new CurrentDeviceDetailsViewModel(DeviceId);
            deviceDetailsView.DataContext = deviceDetailsViewModel;
            deviceDetailsView.ShowDialog();
        }
    }
}
