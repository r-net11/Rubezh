using System.ComponentModel;

namespace FiresecAPI.GK
{
	/// <summary>
	/// Тип точки доступа ГК
	/// </summary>
	public enum GKDoorType
	{
		[Description("Однопроходная")]
		OneWay,

		[Description("Двухпроходная")]
		TwoWay,

		[Description("Турникет")]
		Turnstile,

		[Description("Шлагбаум")]
		Barrier,

		[Description("Шлюзовая кабина")]
		AirlockBooth
	}
}