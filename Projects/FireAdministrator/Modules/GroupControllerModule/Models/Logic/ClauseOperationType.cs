using System.ComponentModel;

namespace GroupControllerModule.Models
{
    public enum ClauseOperationType
    {
        [DescriptionAttribute("во всех из")]
        All,

        [DescriptionAttribute("в любой из")]
        One
    }
}