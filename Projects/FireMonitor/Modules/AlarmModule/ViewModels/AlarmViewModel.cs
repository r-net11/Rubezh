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
            Alarm = alarm;

            ConfirmCommand = new RelayCommand(OnConfirm, CanConfirm);
            ResetCommand = new RelayCommand(OnReset);
            RemoveFromIgnoreListCommand = new RelayCommand(OnRemoveFromIgnoreList);

            ShowOnPlanCommand = new RelayCommand(OnShowOnPlan);
            ShowDeviceCommand = new RelayCommand(OnShowDevice);
            CloseCommand = new RelayCommand(OnClose);
            LeaveCommand = new RelayCommand(OnLeave);
            ShowInstructionCommand = new RelayCommand(OnShowInstruction);
            ShowVideoCommand = new RelayCommand(OnShowVideo);
        }

        public Alarm Alarm { get; set; }

        public AlarmType AlarmType
        {
            get { return Alarm.AlarmType; }
        }

        public string Time
        {
            get { return Alarm.Time.ToString(); }
        }

        public string Description
        {
            get { return Alarm.StateName; }
        }

        public string Source
        {
            get
            {
                switch (Alarm.AlarmEntityType)
                {
                    case AlarmEntityType.Device:
                        var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == Alarm.DeviceUID);
                        return "Устройство " + device.Driver.ShortName + " " + device.DottedAddress;

                    case AlarmEntityType.Zone:
                        var zone = FiresecManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == Alarm.ZoneNo);
                        return "Зона " + zone.PresentationName;
                }
                return "";
            }
        }

        public bool MustConfirm
        {
            get
            {
                return ((Alarm.AlarmType == AlarmType.Fire) &&
                    (FiresecManager.CurrentUser.Permissions.Any(x => x == PermissionType.Oper_NoAlarmConfirm) == false));
            }
        }

        bool CanConfirm()
        {
            return !Alarm.IsConfirmed;
        }

        public RelayCommand ConfirmCommand { get; private set; }
        void OnConfirm()
        {
            Alarm.Confirm();
        }

        public bool CanReset
        {
            get { return Alarm.CanReset(); }
        }

        public RelayCommand ResetCommand { get; private set; }
        void OnReset()
        {
            Alarm.Reset();
            Close();
        }

        public bool CanRemoveFromIgnoreList
        {
            get { return Alarm.CanRemoveFromIgnoreList(); }
        }

        public RelayCommand RemoveFromIgnoreListCommand { get; private set; }
        void OnRemoveFromIgnoreList()
        {
            Alarm.RemoveFromIgnoreList();
            Close();
        }

        public RelayCommand ShowOnPlanCommand { get; private set; }
        void OnShowOnPlan()
        {
            ServiceFactory.Events.GetEvent<ShowDeviceOnPlanEvent>().Publish(Alarm.DeviceUID);
        }

        public RelayCommand ShowDeviceCommand { get; private set; }
        void OnShowDevice()
        {
            ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Publish(Alarm.DeviceUID);
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
            var instructionViewModel = new InstructionViewModel(Alarm.DeviceUID, Alarm.AlarmType);
            ServiceFactory.UserDialogs.ShowModalWindow(instructionViewModel);
        }

        public RelayCommand ShowZoneCommand { get; private set; }
        void OnShowZone()
        {
        }

        public RelayCommand NotifyPhoneCommand { get; private set; }
        void OnNotifyPhone()
        {
        }

        public RelayCommand NotifySmsCommand { get; private set; }
        void OnNotifySms()
        {
        }

        public RelayCommand ShowVideoCommand { get; private set; }
        void OnShowVideo()
        {
            var videoViewModel = new VideoViewModel();
            ServiceFactory.UserDialogs.ShowModalWindow(videoViewModel);
        }

        void Close()
        {
            ServiceFactory.Layout.ShowAlarm(null);
        }
    }
}