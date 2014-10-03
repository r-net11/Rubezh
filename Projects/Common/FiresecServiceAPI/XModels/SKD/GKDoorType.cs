using System.ComponentModel;

namespace FiresecAPI.GK
{
	public enum GKDoorType
	{
		[Description("Однопроходная")]
		OneWay,

		[Description("Двухпроходная")]
		TwoWay
	}
}