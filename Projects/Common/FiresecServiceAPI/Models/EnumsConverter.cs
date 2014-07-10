
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

		public static FS1SubsystemType StringToFS1SubsystemType(string subsystemId)
		{
			switch (subsystemId)
			{
				case "0":
					return FS1SubsystemType.Other;

				case "1":
					return FS1SubsystemType.Fire;

				case "2":
					return FS1SubsystemType.Guard;

				default:
					return FS1SubsystemType.Fire;
			}
		}
	}
}