using System.ComponentModel;

namespace GroupControllerModule.Models
{
    public enum ClauseOperandType
    {
        [DescriptionAttribute("устройство")]
        Device,

        [DescriptionAttribute("зона")]
        Zone
    }
}