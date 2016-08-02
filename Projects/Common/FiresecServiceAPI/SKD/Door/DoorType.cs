using System.ComponentModel;

namespace StrazhAPI.SKD
{
	public enum DoorType
	{
		[Description("Односторонняя")]
		OneWay,

		[Description("Двухсторонняя")]
		TwoWay
	}
}