using StrazhAPI.GK;

namespace Controls
{
	public static class StateHelper
	{
		public static string ToIconSource(this XStateClass stateClass, bool isAm = false)
		{
			if (stateClass == XStateClass.Norm)
				return null;

			if (isAm && stateClass == XStateClass.Fire1)
				return "/Controls;component/StateClassIcons/State1" + ".png";
			if (isAm && stateClass == XStateClass.Fire2)
				return "/Controls;component/StateClassIcons/State2" + ".png";

			return "/Controls;component/StateClassIcons/" + stateClass + ".png";
		}
	}
}