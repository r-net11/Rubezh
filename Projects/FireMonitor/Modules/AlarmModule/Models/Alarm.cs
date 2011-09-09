using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using FiresecClient;

namespace AlarmModule
{
    public class Alarm
    {
        public AlarmType AlarmType { get; set; }
        public StateType StateType { get; set; }

        public Guid DeviceUID { get; set; }
        public string ZoneNo { get; set; }

        public DateTime? Time { get; set; }
        public string StateName { get; set; }

        public bool IsDeleting { get; set; }

        public AlarmEntityType AlarmEntityType
        {
            get
            {
                if (AlarmType == AlarmType.Fire)
                    return AlarmEntityType.Zone;
                return AlarmEntityType.Device;
            }
        }

        public bool IsConfirmed { get; private set; }

        public void Confirm()
        {
            IsConfirmed = true;

            var zone = FiresecManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == ZoneNo);

            var journalRecord = new JournalRecord()
            {
                SystemTime = DateTime.Now,
                DeviceTime = DateTime.Now,
                ZoneName = zone.No,
                Description = "Состояние \"" + StateName + "\" подтверждено оператором",
                StateType = StateType.Info
            };
            FiresecManager.AddJournalRecord(journalRecord);
        }

        public bool CanRemoveFromIgnoreList()
        {
            return StateType == StateType.Off;
        }

        public bool CanReset()
        {
            return StateType != StateType.Off;
        }

        public void RemoveFromIgnoreList()
        {
            var deviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x=>x.UID == DeviceUID);
            if (deviceState.CanDisable())
            {
                if (deviceState.IsDisabled)
                {
                    FiresecManager.RemoveFromIgnoreList(new List<Guid>() { deviceState.Device.UID });
                }
            }
        }

        void OldConfirm()
        {
            string confirmationText = "";
            switch (AlarmEntityType)
            {
                case AlarmEntityType.Zone:
                    var zone = FiresecManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == ZoneNo);
                    confirmationText = "Подтверждение пожара в зоне " + zone.PresentationName;
                    break;

                case AlarmEntityType.Device:
                    var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == DeviceUID);
                    confirmationText = "Подтверждение состояния " + this.StateName +
                        " устройство " + device.Driver.ShortName + " " + device.DottedAddress;

                    break;
            }
            FiresecManager.AddUserMessage(confirmationText);
        }

        public void Reset()
        {
            var resetItems = new List<ResetItem>();
            resetItems.Add(GetResetItem());
            FiresecManager.ResetStates(resetItems);
        }

        public ResetItem GetResetItem()
        {
            switch (AlarmEntityType)
            {
                case AlarmEntityType.Device:
                    return GetDeviceResetItem();

                case AlarmEntityType.Zone:
                    return GetZoneResetItem();
            }

            return null;
        }

        ResetItem GetZoneResetItem()
        {
            foreach(var device in FiresecManager.DeviceConfiguration.Devices)
            {
                if (device.ZoneNo == ZoneNo)
                {
                    var deviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x=>x.UID == device.UID);
                    if (deviceState.States.Any(x => x.DriverState.StateType == StateType.Fire))
                    {
                        DeviceUID = deviceState.UID;
                        return GetDeviceResetItem();
                    }
                }
            }

            return null;
        }

        ResetItem GetDeviceResetItem()
        {
            var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == DeviceUID);
            var parentDevice = device.Parent;
            var deviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.UID == device.UID);
            var parentDeviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.UID == parentDevice.UID);

            var resetItem = new ResetItem();

            if (AlarmType == AlarmType.Fire || AlarmType == AlarmType.Attention || AlarmType == AlarmType.Info)
            {
                resetItem.DeviceUID = parentDeviceState.UID;

                foreach (var state in parentDeviceState.States)
                {
                    if (state.DriverState.StateType == EnumsConverter.AlarmTypeToStateType(AlarmType) &&
                        state.DriverState.IsManualReset)
                    {
                        resetItem.StateNames.Add(state.DriverState.Name);
                    }
                }
            }
            if (AlarmType == AlarmType.Auto)
            {
                resetItem.DeviceUID = device.UID;

                foreach (var state in deviceState.States)
                {
                    if (state.DriverState.IsAutomatic && state.DriverState.IsManualReset)
                    {
                        resetItem.StateNames.Add(state.DriverState.Name);
                    }
                }
            }
            if (AlarmType == AlarmType.Off)
            {
            }

            if (resetItem.StateNames.Count > 0)
                return resetItem;

            return null;
        }
    }
}
