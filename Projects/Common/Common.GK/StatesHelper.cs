using XFiresecAPI;

namespace Common.GK
{
	public static class StatesHelper
	{
		public static int XStateTypeToPriority(XStateType stateType)
		{
			switch(stateType)
			{
				case XStateType.Norm:
					return 7;

				case XStateType.Attention:
					return 1;

				case XStateType.Fire1:
					return 0;

				case XStateType.Fire2:
					return 0;

				case XStateType.Test:
					return 6;

				case XStateType.Failure:
					return 2;

				case XStateType.Ignore:
					return 4;

				case XStateType.On:
					return 7;

				case XStateType.Off:
					return 7;

				case XStateType.TurningOn:
					return 7;

				case XStateType.TurningOff:
					return 7;

				case XStateType.TurnOn:
					return 7;

				case XStateType.CancelDelay:
					return 7;

				case XStateType.TurnOff:
					return 7;

				case XStateType.Stop:
					return 7;

				case XStateType.ForbidStart:
					return 7;

				case XStateType.TurnOnNow:
					return 7;

				case XStateType.TurnOffNow:
					return 7;
			}
			return 7;
		}
	}
}