using System.ComponentModel;
namespace FiresecAPI.Models
{
	public enum ZoneLogicOperation
	{
		[DescriptionAttribute("Во всех зонах из")]
		All,

		[DescriptionAttribute("В любой зоне из")]
		Any
	}
}