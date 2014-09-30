using System.ComponentModel;

namespace FiresecAPI.GK
{
	public enum DelayRegime
	{
		[DescriptionAttribute("Выключено")]
		Off = 0,

		[DescriptionAttribute("Включено")]
		On = 1
	}
}

//