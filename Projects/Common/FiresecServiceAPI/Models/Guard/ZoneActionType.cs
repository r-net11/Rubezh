using System.ComponentModel;
namespace FiresecAPI.Models
{
    public enum ZoneActionType
    {
        [DescriptionAttribute("Поставить на охрану")]
        Set,

        [DescriptionAttribute("Снять с охраны")]
        Unset
    }
}
