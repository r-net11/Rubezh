using System.ComponentModel;
namespace FiresecAPI.Models
{
	public enum ZoneLogicMROMessageType
	{
		[DescriptionAttribute("Добавить")]
		Add,

		[DescriptionAttribute("Заменить")]
		Remove
	}
}