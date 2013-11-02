using System.ComponentModel;

namespace XFiresecAPI
{
	public enum DelayRegime
	{
		[DescriptionAttribute("Выключено")]
		Off = 0,

		[DescriptionAttribute("Включено")]
		On = 1
	}
}