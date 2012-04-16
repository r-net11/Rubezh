
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
                    return "Норма";

                default:
                    return "Прочие";
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

        public static StateType AlarmTypeToStateType(AlarmType alarmType)
        {
            switch (alarmType)
            {
                case AlarmType.Fire:
                    return (StateType)0;

                case AlarmType.Attention:
                    return (StateType)1;

                case AlarmType.Info:
                    return (StateType)6;

                default:
                    return (StateType)8;
            }
        }

        public static SubsystemType StringToSubsystemType(string subsystemId)
        {
            switch (subsystemId)
            {
                case "0":
                    return SubsystemType.Other;

                case "1":
                    return SubsystemType.Fire;

                case "2":
                    return SubsystemType.Guard;

                default:
                    return SubsystemType.Fire;
            }
        }
    }
}