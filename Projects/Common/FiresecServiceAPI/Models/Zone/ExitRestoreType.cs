using System.ComponentModel;

namespace FiresecAPI.Models
{
	public enum ExitRestoreType
	{
		[DescriptionAttribute("Сбрасывать таймер")]
		SetTimer,

		[DescriptionAttribute("Возобновлять таймер")]
		RestoreTimer
	}
}