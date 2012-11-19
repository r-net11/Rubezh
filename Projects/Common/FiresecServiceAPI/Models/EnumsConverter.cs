
namespace FiresecAPI.Models
{
    public static class EnumsConverter
    {
        public static StateType AlarmTypeToStateType(AlarmType alarmType)
        {
            switch (alarmType)
            {
				case AlarmType.Guard:
                case AlarmType.Fire:
                    return StateType.Fire;

                case AlarmType.Attention:
                    return StateType.Attention;

                case AlarmType.Info:
                    return StateType.Info;

                default:
                    return StateType.No;
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