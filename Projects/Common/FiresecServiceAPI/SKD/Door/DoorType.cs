using System.ComponentModel;
using Localization;

namespace FiresecAPI.SKD
{
	public enum DoorType
	{
		//[Description("Односторонняя")]
        [LocalizedDescription(typeof(Resources.Language.SKD.Door.DoorType),"OneWay")]
		OneWay,

        //[Description("Двухсторонняя")]
        [LocalizedDescription(typeof(Resources.Language.SKD.Door.DoorType), "TwoWay")]
		TwoWay
	}
}