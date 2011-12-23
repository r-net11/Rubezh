using System.ComponentModel;
namespace FiresecAPI.Models
{
    public enum DeviceCategoryType
    {
        [DescriptionAttribute("Прочие устройства")]
        Other = 0,

        [DescriptionAttribute("Прибор")]
        Device = 1,

        [DescriptionAttribute("Датчик")]
        Sensor = 2,

        [DescriptionAttribute("Исполнительное устройство")]
        Effector = 3,

        [DescriptionAttribute("Сеть передачи данных")]
        Communication = 4,

        [DescriptionAttribute("[Без устройства]")]
        None = 5,

        [DescriptionAttribute("Удаленный сервер")]
        RemoteServer = 6
    }
}