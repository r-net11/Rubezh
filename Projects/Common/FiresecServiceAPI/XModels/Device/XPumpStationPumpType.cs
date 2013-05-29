using System.ComponentModel;

namespace XFiresecAPI
{
    public enum XPumpStationPumpType
    {
        [DescriptionAttribute("Основной")]
        Main = 0,

        [DescriptionAttribute("Резервный")]
        Reserve = 1
    }
}