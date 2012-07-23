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
				case XStateType.Fire2:
					return 0;

				case XStateType.Test:
					return 6;

				case XStateType.Failure:
					return 2;

				case XStateType.Ignore:
					return 4;

				case XStateType.On:
				case XStateType.Off:
				case XStateType.TurningOn:
				case XStateType.TurningOff:
				case XStateType.TurnOn:
				case XStateType.CancelDelay:
				case XStateType.TurnOff:
				case XStateType.Stop:
				case XStateType.ForbidStart:
				case XStateType.TurnOnNow:
				case XStateType.TurnOffNow:
				case XStateType.Reserve1:
				case XStateType.Reserve2:
					return 7;
			}
			return 7;
		}
	}
}