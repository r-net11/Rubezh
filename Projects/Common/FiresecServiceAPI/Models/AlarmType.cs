using System.ComponentModel;
namespace FiresecAPI.Models
{
    public enum AlarmType
    {
        [DescriptionAttribute("Пожар")]
        Fire = 0,

        [DescriptionAttribute("Внимание")]
        Attention = 1,

        [DescriptionAttribute("Неисправность")]
        Failure = 2,

        [DescriptionAttribute("Отключенное оборудование")]
        Off = 3,

        [DescriptionAttribute("Информация")]
        Info = 4,

        [DescriptionAttribute("Требуется обслуживание")]
        Service = 5,

        [DescriptionAttribute("Автоматика отключена")]
        Auto = 6
    }
}