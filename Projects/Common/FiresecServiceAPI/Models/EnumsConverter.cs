
namespace FiresecAPI.Models
{
    public static class EnumsConverter
    {
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