using System.ComponentModel;

namespace FiresecAPI.GK
{
	/// <summary>
	/// Тип подсистемы ГК
	/// </summary>
	public enum GKSubsystemType
	{
		[DescriptionAttribute("Система")]
		System = 0,

		[DescriptionAttribute("Прибор")]
		GK = 1,
	}
}