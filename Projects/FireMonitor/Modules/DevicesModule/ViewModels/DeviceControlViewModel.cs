using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using FiresecClient.Models;
using FiresecClient;

namespace DevicesModule.ViewModels
{
    public class DeviceControlViewModel
    {
        Device _device;

        public DeviceControlViewModel(Device device)
        {
            _device = device;

            CloseCommand = new RelayCommand(OnClose);
            StopCommand = new RelayCommand(OnStop);
            OpenCommand = new RelayCommand(OnOpen);
            AutomaticOnCommand = new RelayCommand(OnAutomaticOn);
            AutomaticOffCommand = new RelayCommand(OnAutomaticOff);
            StartCommand = new RelayCommand(OnStart);
            CancelStartCommand = new RelayCommand(OnCancelStart);
            ConfirmCommand = new RelayCommand(OnConfirm);
        }

        public RelayCommand CloseCommand { get; private set; }
        void OnClose()
        {
            FiresecInternalClient.ExecuteCommand(_device.PlaceInTree, "BoltClose");
        }

        public RelayCommand StopCommand { get; private set; }
        void OnStop()
        {
            FiresecInternalClient.ExecuteCommand(_device.PlaceInTree, "BoltStop");
        }

        public RelayCommand OpenCommand { get; private set; }
        void OnOpen()
        {
            FiresecInternalClient.ExecuteCommand(_device.PlaceInTree, "BoltOpen");
        }

        public RelayCommand AutomaticOnCommand { get; private set; }
        void OnAutomaticOn()
        {
            FiresecInternalClient.ExecuteCommand(_device.PlaceInTree, "BoltAutoOn");
        }

        public RelayCommand AutomaticOffCommand { get; private set; }
        void OnAutomaticOff()
        {
            FiresecInternalClient.ExecuteCommand(_device.PlaceInTree, "BoltAutoOff");
        }

        public RelayCommand StartCommand { get; private set; }
        void OnStart()
        {
        }

        public RelayCommand CancelStartCommand { get; private set; }
        void OnCancelStart()
        {
        }

        public RelayCommand ConfirmCommand { get; private set; }
        void OnConfirm()
        {
        }
    }
}
