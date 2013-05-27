using System.ComponentModel;

namespace XFiresecAPI
{
    public enum XPumpStationPumpType
    {
        [DescriptionAttribute("Основной")]
        Main,

        [DescriptionAttribute("Резервный")]
        Reserve
    }
}