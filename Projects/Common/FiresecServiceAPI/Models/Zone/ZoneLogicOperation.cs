using System.ComponentModel;
namespace FiresecAPI.Models
{
	public enum ZoneLogicOperation
	{
		[DescriptionAttribute("Во всех зонах из")]
		All,

		[DescriptionAttribute("В любых зонах из")]
		Any
	}
}