using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecClient;
using Infrastructure;
using AlarmModule.Events;
using Infrastructure.Events;
using FiresecAPI.Models;

namespace AlarmModule
{
    public class AlarmWatcher
    {
        public AlarmWatcher()
        {
            FiresecEventSubscriber.DeviceStateChangedEvent += new Action<string>(OnDeviceStateChangedEvent);
            DeviceState.AlarmAdded += new Action<AlarmType, string>(DeviceState_AlarmAdded);
            DeviceState.AlarmRemoved += new Action<AlarmType, string>(DeviceState_AlarmRemoved);
            Update();
        }

        void Update()
        {
            foreach (var deviceState in FiresecManager.DeviceStates.DeviceStates)
            {
                if (deviceState.States == null)
                    continue;

                var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.Id == deviceState.Id);
                if (device.Driver.Category == DeviceCategoryType.Sensor)
                {
                    //bool isTest = deviceState.InnerStates.Any(x => ((x.IsActive) && (x.CanResetOnPanel) && (x.State.StateType == StateType.Info)));

                    deviceState.IsFire = deviceState.States.Any(x => ((x.IsActive) && (x.DriverState.State.StateType == StateType.Fire)));
                    deviceState.IsAttention = deviceState.States.Any(x => ((x.IsActive) && (x.DriverState.State.StateType == StateType.Attention)));
                    deviceState.IsInfo = deviceState.States.Any(x => ((x.IsActive) && (x.DriverState.State.StateType == StateType.Info) && (x.DriverState.Name == "Тест")));
                    deviceState.IsOff = deviceState.States.Any(x => ((x.IsActive) && (x.DriverState.State.StateType == StateType.Off)));
                }

                deviceState.IsFailure = deviceState.States.Any(x => ((x.IsActive) && (x.DriverState.IsManualReset) && (x.DriverState.State.StateType == StateType.Failure)));
                deviceState.IsService = deviceState.States.Any(x => ((x.IsActive) && (x.DriverState.IsManualReset) && (x.DriverState.State.StateType == StateType.Service) && (x.DriverState.IsAutomatic) == false));
                deviceState.IsAutomaticOff = deviceState.States.Any(x => ((x.IsActive) && (x.DriverState.IsManualReset) && (x.DriverState.IsAutomatic)));
            }
        }

        void DeviceState_AlarmAdded(AlarmType alarmType, string id)
        {
            var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.Id == id);
            var deviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.Id == id);
            Alarm alarm = new Alarm();
            alarm.AlarmType = alarmType;
            alarm.DeviceId = id;
            alarm.Name = AlarmToString(alarmType) + ". Устройство " + device.Driver.Name + " - " + device.PresentationAddress;
            alarm.Time = DateTime.Now.ToString();
            ServiceFactory.Events.GetEvent<AlarmAddedEvent>().Publish(alarm);

            if (alarmType == AlarmType.Fire)
            {
                ServiceFactory.Events.GetEvent<ShowDeviceOnPlanEvent>().Publish(id);
            }
        }

        void DeviceState_AlarmRemoved(AlarmType alarmType, string id)
        {
            Alarm alarm = new Alarm();
            alarm.AlarmType = alarmType;
            alarm.DeviceId = id;
            ServiceFactory.Events.GetEvent<ResetAlarmEvent>().Publish(alarm);
        }

        void OnDeviceStateChangedEvent(string obj)
        {
            Update();
        }

        public string AlarmToString(AlarmType alarmType)
        {
            switch (alarmType)
            {
                case AlarmType.Attention:
                    return "Внимание";

                case AlarmType.Auto:
                    return "Автоматика отключена";

                case AlarmType.Failure:
                    return "Неисправность";

                case AlarmType.Fire:
                    return "Пожар";

                case AlarmType.Info:
                    return "Информация";

                case AlarmType.Off:
                    return "Отключенное оборудование";

                case AlarmType.Service:
                    return "Требуется обслуживание";

                default:
                    return "";
            }
        }
    }
}
