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
		TwoWay
	}
}