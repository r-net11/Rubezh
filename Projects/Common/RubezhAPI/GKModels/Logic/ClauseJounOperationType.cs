using System.ComponentModel;

namespace RubezhAPI.GK
{
	/// <summary>
	/// Тип объединения условий
	/// </summary>
	public enum ClauseJounOperationType
	{
		[DescriptionAttribute("и")]
		And,

		[DescriptionAttribute("или")]
		Or
	}
}