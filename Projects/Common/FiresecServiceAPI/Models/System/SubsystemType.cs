using System.ComponentModel;

namespace FiresecAPI.Models
{
    public enum SubsystemType
    {
        [DescriptionAttribute("Пожарная")]
        Fire,

        [DescriptionAttribute("Охранная")]
        Guard,

        [DescriptionAttribute("Прочие")]
        Other
    };
}
