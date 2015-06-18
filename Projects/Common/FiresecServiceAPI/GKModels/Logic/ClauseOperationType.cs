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

		[DescriptionAttribute("в любой ТД из")]
		AnyDoor,

		[DescriptionAttribute("во всех ТД из")]
		AllDoors,
	}
}