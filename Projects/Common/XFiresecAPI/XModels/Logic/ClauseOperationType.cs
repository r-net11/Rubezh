using System.ComponentModel;

namespace XFiresecAPI
{
    public enum ClauseOperationType
    {
        [DescriptionAttribute("во всех из")]
        All,

        [DescriptionAttribute("в любой из")]
        One
    }
}