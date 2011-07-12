using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecClient;
using Firesec;
using Infrastructure;
using AlarmModule.Events;
using Infrastructure.Events;
using FiresecClient.Models;

namespace AlarmModule
{
    public class AlarmWatcher
    {
        public AlarmWatcher()
        {
            CurrentStates.NewJournalEvent += new Action<Firesec.ReadEvents.journalType>(CurrentStates_NewJournalEvent);
            FiresecManager.States.DeviceStateChanged += new Action<string>(CurrentStates_DeviceStateChanged);
            DeviceState.AlarmAdded += new Action<AlarmType, string>(DeviceState_AlarmAdded);
            DeviceState.AlarmRemoved += new Action<AlarmType, string>(DeviceState_AlarmRemoved);
            Update();
        }

        void Update()
        {
            foreach (var deviceState in FiresecManager.States.DeviceStates)
            {
                var device = FiresecManager.Configuration.Devices.FirstOrDefault(x => x.Id == deviceState.Id);
                if (device.Driver.Category == DeviceCategory.Sensor)
                {
                    //bool isTest = deviceState.InnerStates.Any(x => ((x.IsActive) && (x.CanResetOnPanel) && (x.State.StateType == StateType.Info)));

                    deviceState.IsFire = deviceState.InnerStates.Any(x => ((x.IsActive) && (x.State.StateType == StateType.Fire)));
                    deviceState.IsAttention = deviceState.InnerStates.Any(x => ((x.IsActive) && (x.State.StateType == StateType.Attention)));
                    deviceState.IsInfo = deviceState.InnerStates.Any(x => ((x.IsActive) && (x.State.StateType == StateType.Info) && (x.Name == "Тест")));
                    deviceState.IsOff = deviceState.InnerStates.Any(x => ((x.IsActive) && (x.State.StateType == StateType.Off)));
                }

                deviceState.IsFailure = deviceState.InnerStates.Any(x => ((x.IsActive) && (x.IsManualReset) && (x.State.StateType == StateType.Failure)));
                deviceState.IsService = deviceState.InnerStates.Any(x => ((x.IsActive) && (x.IsManualReset) && (x.State.StateType == StateType.Service) && (x.IsAutomatic) == false));
                deviceState.IsAutomaticOff = deviceState.InnerStates.Any(x => ((x.IsActive) && (x.IsManualReset) && (x.IsAutomatic)));
            }
        }

        void DeviceState_AlarmAdded(AlarmType alarmType, string id)
        {
            var deviceState = FiresecManager.States.DeviceStates.FirstOrDefault(x => x.Id == id);
            var device = FiresecManager.Configuration.Devices.FirstOrDefault(x => x.Id == id);
            Alarm alarm = new Alarm();
            alarm.AlarmType = alarmType;
            alarm.DeviceId = id;
            alarm.Name = AlarmToString(alarmType) + ". Устройство " + device.Driver.Name + " - " + device.Address;
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

        void CurrentStates_DeviceStateChanged(string obj)
        {
            Update();
        }

        void CurrentStates_NewJournalEvent(Firesec.ReadEvents.journalType journalItem)
        {
            //Alarm.CreateFromJournalEvent(journalItem);
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
