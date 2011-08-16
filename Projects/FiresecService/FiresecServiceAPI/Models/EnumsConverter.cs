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
    }
}