using System.ComponentModel;

namespace FiresecAPI.GK
{
	/// <summary>
	/// Режим после удержания
	/// </summary>
	public enum DelayRegime
	{
		[DescriptionAttribute("Выключено")]
		Off = 0,

		[DescriptionAttribute("Включено")]
		On = 1
	}
}