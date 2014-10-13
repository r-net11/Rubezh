using System.ComponentModel;

namespace FiresecAPI.GK
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