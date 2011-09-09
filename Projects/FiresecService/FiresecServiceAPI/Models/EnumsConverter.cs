namespace FiresecAPI.Models
{
    public static class EnumsConverter
    {
        public static string StateTypeToClassName(StateType stateType)
        {
            switch (stateType)
            {
                case StateType.Fire:
                    return "Тревога";

                case StateType.Attention:
                    return "Внимание (предтревожное)";

                case StateType.Failure:
                    return "Неисправность";

                case StateType.Service:
                    return "Требуется обслуживание";

                case StateType.Off:
                    return "Обход устройств";

                case StateType.Unknown:
                    return "Неопределено";

                case StateType.Info:
                    return "Норма(*)";

                case StateType.Norm:
                    return "Норма";

                case StateType.No:
                    return "Нет состояния";

                default:
                    return "";
            }
        }

        public static string StateTypeToLibraryStateName(StateType stateType)
        {
            switch (stateType)
            {
                case StateType.Fire:
                    return "Тревога";

                case StateType.Attention:
                    return "Внимание (предтревожное)";

                case StateType.Failure:
                    return "Неисправность";

                case StateType.Service:
                    return "Требуется обслуживание";

                case StateType.Off:
                    return "Обход устройств";

                case StateType.Unknown:
                    return "Неопределено";

                case StateType.Info:
                    return "Норма(*)";

                case StateType.Norm:
                    return "Норма";

                case StateType.No:
                    return "Базовый рисунок";

                default:
                    return "";
            }
        }

        public static string StateTypeToEventName(StateType stateType)
        {
            switch (stateType)
            {
                case StateType.Fire:
                    return "Тревога";

                case StateType.Attention:
                    return "Внимание";

                case StateType.Failure:
                    return "Неисправность";

                case StateType.Service:
                    return "Требуется обслуживание";

                case StateType.Off:
                    return "Тревоги отключены";

                case StateType.Info:
                    return "Информация";

                case StateType.Norm:
                    return "Прочие";

                default:
                    return "";
            }
        }

        public static string CategoryTypeToCategoryName(DeviceCategoryType categoryType)
        {
            switch (categoryType)
            {
                case DeviceCategoryType.Other:
                    return "Прочие устройства";

                case DeviceCategoryType.Device:
                    return "Прибор";

                case DeviceCategoryType.Sensor:
                    return "Датчик";

                case DeviceCategoryType.Effector:
                    return "Исполнительное устройство";

                case DeviceCategoryType.Communication:
                    return "Сеть передачи данных";

                case DeviceCategoryType.RemoteServer:
                    return "Удаленный сервер";

                case DeviceCategoryType.None:
                    return "[Без устройства]";

                default:
                    return "";
            }
        }

        public static string DeviceTypeToString(DeviceType deviceType)
        {
            switch (deviceType)
            {
                case DeviceType.Fire:
                    return "пожарный";

                case DeviceType.FireSecurity:
                    return "охранно-пожарный";

                case DeviceType.Sequrity:
                    return "охранный";

                case DeviceType.Technoligical:
                    return "";

                default:
                    return "технологический";
            }
        }

        public static string BeeperTypeToBeeperName(BeeperType beeperType)
        {
            switch (beeperType)
            {
                case BeeperType.None:
                    return "<нет>";
                case BeeperType.Alarm:
                    return "Тревога";
                case BeeperType.Attention:
                    return "Внимание";
                default:
                    return "<нет>";
            }
        }

        public static StateType AlarmTypeToStateType(AlarmType alarmType)
        {
            switch (alarmType)
            {
                case AlarmType.Fire:
                    return (StateType) 0;

                case AlarmType.Attention:
                    return (StateType) 1;

                case AlarmType.Info:
                    return (StateType) 6;

                default:
                    return (StateType) 8;
            }
        }

        public static string AlarmToString(AlarmType alarmType)
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

        public static string StateToIcon(StateType stateType)
        {
            switch (stateType)
            {
                case StateType.Fire:
                    return "DS_Critical";

                case StateType.Attention:
                    return "DS_Warning";

                case StateType.Failure:
                    return "DS_Error";

                case StateType.Service:
                    return "DS_ServiceRequired";

                case StateType.Off:
                    return "DS_Mute";

                case StateType.Unknown:
                    return "DS_Unknown";

                case StateType.Info:
                    return "DS_Normal";

                case StateType.Norm:
                    return null;

                default:
                    return null;
            }
        }

        public static string ZoneLogicStateToString(ZoneLogicState zoneLogicState)
        {
            switch (zoneLogicState)
            {
                case ZoneLogicState.Fire:
                    return "Пожар";

                case ZoneLogicState.Attention:
                    return "Внимание";

                case ZoneLogicState.MPTAutomaticOn:
                    return "Включение автоматики МПТ";

                case ZoneLogicState.MPTOn:
                    return "Включение модуля пожаротушения";

                //case ZoneLogicState.zsExitDelay_Unused:
                //    return "Не используется";

                case ZoneLogicState.Alarm:
                    return "Тревога";

                case ZoneLogicState.GuardSet:
                    return "Поставлен на охрану";

                case ZoneLogicState.GuardUnSet:
                    return "Снят с охраны";

                case ZoneLogicState.PCN:
                    return "ПЦН";

                case ZoneLogicState.Lamp:
                    return "Лампа";

                case ZoneLogicState.Failure:
                    return "Неисправность прибора";

                case ZoneLogicState.AM1TOn:
                    return "Сработка АМ1-Т";

                default:
                    return null;
            }
        }

        public static string InstructionTypeToString(InstructionType instructionType)
        {
            switch (instructionType)
            {
                case InstructionType.General:
                    return "Общая инструкция";

                case InstructionType.Details:
                    return "Инструкция для зон и устройств";

                default:
                    return "";
            }
        }

        public static string SubsystemTypeToString(SubsystemType subsystem)
        {
            switch (subsystem)
            {
                case SubsystemType.Fire:
                    return "Пожарная";

                case SubsystemType.Guard:
                    return "Охранная";

                case SubsystemType.Other:
                    return "Прочие";

                default:
                    return "";
            }
        }

        public static SubsystemType StringToSubsystemType(string subsystemId)
        {
            switch (subsystemId)
            {
                case "0":
                    return SubsystemType.Fire;
                case "1":
                    return SubsystemType.Guard;
                default:
                    return SubsystemType.Other;
            }
        }
    }
}