using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssadDevices.Devices
{
    public class AssadMonitor : AssadBase
    {
        public override void SetInnerDevice(Assad.MHconfigTypeDevice innerDevice)
        {
            base.SetInnerDevice(innerDevice);
        }

        public override Assad.DeviceType GetInnerStates()
        {
            Assad.DeviceType deviceType = new Assad.DeviceType();
            deviceType.deviceId = DeviceId;
            List<Assad.DeviceTypeState> states = new List<Assad.DeviceTypeState>();

            Assad.DeviceTypeState configurationState = new Assad.DeviceTypeState();
            configurationState.state = "Конфигурация";
            configurationState.value = "Норма";
            states.Add(configurationState);

            Assad.DeviceTypeState state1 = new Assad.DeviceTypeState();
            state1.state = "Тревога";
            state1.value = "Нет";
            states.Add(state1);

            Assad.DeviceTypeState state2 = new Assad.DeviceTypeState();
            state2.state = "Внимание (предтревожное)";
            state2.value = "Нет";
            states.Add(state2);

            Assad.DeviceTypeState state3 = new Assad.DeviceTypeState();
            state3.state = "Неисправность";
            state3.value = "Нет";
            states.Add(state3);

            Assad.DeviceTypeState state4 = new Assad.DeviceTypeState();
            state4.state = "Требуется обслуживание";
            state4.value = "Нет";
            states.Add(state4);

            Assad.DeviceTypeState state5 = new Assad.DeviceTypeState();
            state5.state = "Обход устройств";
            state5.value = "Нет";
            states.Add(state5);

            Assad.DeviceTypeState state6 = new Assad.DeviceTypeState();
            state6.state = "Неопределено";
            state6.value = "Нет";
            states.Add(state6);

            Assad.DeviceTypeState state7 = new Assad.DeviceTypeState();
            state7.state = "Норма(*)";
            state7.value = "Нет";
            states.Add(state7);

            Assad.DeviceTypeState state8 = new Assad.DeviceTypeState();
            state8.state = "Норма";
            state8.value = "Нет";
            states.Add(state8);

            deviceType.state = states.ToArray();
            return deviceType;
        }

        public override Assad.DeviceType QueryAbility()
        {
            Assad.DeviceType deviceAbility = new Assad.DeviceType();
            deviceAbility.deviceId = DeviceId;
            List<Assad.DeviceTypeParam> abilityParameters = new List<Assad.DeviceTypeParam>();

            deviceAbility.param = abilityParameters.ToArray();
            return deviceAbility;
        }
    }
}
