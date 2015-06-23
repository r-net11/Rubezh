using System.ComponentModel;
namespace FiresecAPI.GK
{
	public enum GKDriverType
	{
		[Description("Локальная сеть")]
		System,
		[Description("ГК")]
		GK
	}
}