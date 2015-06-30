using System.ComponentModel;

namespace FiresecAPI.SKD
{
	public enum DoorType
	{
		[Description("Односторонняя")]
		OneWay,

		[Description("Двухсторонняя")]
		TwoWay,
	}
}