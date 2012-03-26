using System.ComponentModel;

namespace GroupControllerModule.Models
{
    public enum ClauseJounOperationType
    {
        [DescriptionAttribute("и")]
        And,

        [DescriptionAttribute("или")]
        Or
    }
}