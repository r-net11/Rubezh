using System.Collections.Generic;
using System.Linq;
using AlarmModule.Events;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;

namespace AlarmModule.ViewModels
{
    public class AlarmViewModel : RegionViewModel
    {
        public AlarmViewModel(Alarm alarm)
        {
            _alarm = alarm;
        }

        public void Initialize()
        {
            ConfirmCommand = new RelayCommand(OnConfirm, CanConfirm);
            ResetCommand = new RelayCommand(OnReset);
            ShowOnPlanCommand = new RelayCommand(OnShowOnPlan);
            ShowDeviceCommand = new RelayCommand(OnShowDevice);
            CloseCommand = new RelayCommand(OnClose);
            LeaveCommand = new RelayCommand(OnLeave);
            ShowInstructionCommand = new RelayCommand(OnShowInstruction);
            ShowVideoCommand = new RelayCommand(OnShowVideo);
        }

        public Alarm _alarm;

        public AlarmType AlarmType
        {
            get { return _alarm.AlarmType; }
        }

        public string Name
        {
            get { return _alarm.Name; }
        }

        public string Time
        {
            get { return _alarm.Time; }
        }

        public string DeviceName
        {
            get
            {
                var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.Id == _alarm.DeviceId);
                return device.Driver.ShortName + " - " + device.PresentationAddress;
            }
        }

        public string Zone
        {
            get
            {
                var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.Id == _alarm.DeviceId);
                var zone = FiresecManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == device.ZoneNo);
                return zone.No + "." + zone.Name;
            }
        }

        bool _isConfirmed;
        public bool IsConfirmed
        {
            get { return _isConfirmed; }
            set
            {
                _isConfirmed = value;
                OnPropertyChanged("IsConfirmed");
            }
        }

        bool CanConfirm(object obj)
        {
            return !IsConfirmed;
        }

        public RelayCommand ConfirmCommand { get; private set; }
        void OnConfirm()
        {
            if (CanConfirm(null))
            {
                IsConfirmed = true;
                FiresecManager.AddUserMessage("Подтверждение " + _alarm.Name);
            }
        }

        public RelayCommand ResetCommand { get; private set; }
        void OnReset()
        {
            //ServiceFactory.Events.GetEvent<ResetAlarmEvent>().Publish(_alarm);
            Reset();
            Close();
        }

        public RelayCommand ShowOnPlanCommand { get; private set; }
        void OnShowOnPlan()
        {
            ServiceFactory.Events.GetEvent<ShowDeviceOnPlanEvent>().Publish(_alarm.DeviceId);
        }

        public RelayCommand ShowDeviceCommand { get; private set; }
        void OnShowDevice()
        {
            ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Publish(_alarm.DeviceId);
        }

        public RelayCommand CloseCommand { get; private set; }
        void OnClose()
        {
            Close();
        }

        public RelayCommand LeaveCommand { get; private set; }
        void OnLeave()
        {
            ServiceFactory.Events.GetEvent<MoveAlarmToEndEvent>().Publish(this);
            Close();
        }

        public RelayCommand ShowInstructionCommand { get; private set; }
        void OnShowInstruction()
        {
            InstructionViewModel instructionViewModel = new InstructionViewModel();
            ServiceFactory.UserDialogs.ShowModalWindow(instructionViewModel);
        }

        public RelayCommand ShowVideoCommand { get; private set; }
        void OnShowVideo()
        {
            var videoViewModel = new VideoViewModel();
            ServiceFactory.UserDialogs.ShowModalWindow(videoViewModel);
        }

        void Reset()
        {
            var resetItems = new List<ResetItem>();
            resetItems.Add(GetResetItem());
            FiresecManager.ResetStates(resetItems);
        }

        public ResetItem GetResetItem()
        {
            var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.Id == _alarm.DeviceId);
            var parentDevice = device.Parent;
            var deviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.Id == device.Id);
            var parentDeviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.Id == parentDevice.Id);

            var resetItem = new ResetItem();

            if (_alarm.AlarmType == AlarmType.Fire ||
                _alarm.AlarmType == AlarmType.Attention ||
                _alarm.AlarmType == AlarmType.Info)
            {
                resetItem.DeviceId = parentDeviceState.Id;

                foreach (var state in parentDeviceState.States)
                {
                    if (state.IsActive &&
                        state.DriverState.StateType == EnumsConverter.AlarmTypeToStateType(_alarm.AlarmType) &&
                        state.DriverState.IsManualReset)
                    {
                        resetItem.StateNames.Add(state.DriverState.Name);
                    }
                }
            }
            if (_alarm.AlarmType == AlarmType.Auto)
            {
                resetItem.DeviceId = device.Id;

                foreach (var state in deviceState.States)
                {
                    if (state.IsActive && state.DriverState.IsAutomatic && state.DriverState.IsManualReset)
                    {
                        resetItem.StateNames.Add(state.DriverState.Name);
                    }
                }
            }
            if (_alarm.AlarmType == AlarmType.Off)
            {
            }

            if (resetItem.StateNames.Count > 0)
                return resetItem;

            return null;
        }

        static void Close()
        {
            ServiceFactory.Layout.ShowAlarm(null);
        }
    }
}