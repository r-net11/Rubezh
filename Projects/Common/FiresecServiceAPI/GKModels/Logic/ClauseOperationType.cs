using System.ComponentModel;

namespace FiresecAPI.GK
{
	/// <summary>
	/// Тип операции
	/// </summary>
	public enum ClauseOperationType
	{
		[DescriptionAttribute("в любом устройстве из")]
		AnyDevice,

		[DescriptionAttribute("во всех устройствах из")]
		AllDevices,

		[DescriptionAttribute("в любом МПТ из")]
		AnyMPT,

		[DescriptionAttribute("во всех МПТ из")]
		AllMPTs,

		[DescriptionAttribute("в любой задержке из")]
		AnyDelay,

		[DescriptionAttribute("во всех задержках из")]
		AllDelays,

		[DescriptionAttribute("в любой ТД из")]
		AnyDoor,

		[DescriptionAttribute("во всех ТД из")]
		AllDoors,
	}
}