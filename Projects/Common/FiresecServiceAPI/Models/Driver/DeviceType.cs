using System.ComponentModel;
namespace FiresecAPI.Models
{
    public enum DeviceType
    {
        [DescriptionAttribute("пожарный")]
        Fire,

        [DescriptionAttribute("охранный")]
        Sequrity,

        [DescriptionAttribute("технологический")]
        Technoligical,

        [DescriptionAttribute("охранно-пожарный")]
        FireSecurity
    }
}