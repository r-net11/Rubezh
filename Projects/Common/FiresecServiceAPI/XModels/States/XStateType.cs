using System.ComponentModel;

namespace XFiresecAPI
{
    public enum XStateType
    {
        [DescriptionAttribute("Дежурный")]
        Norm = 0,

        [DescriptionAttribute("Внимание")]
        Attention = 1,

        [DescriptionAttribute("Пожар 1")]
        Fire1 = 2,

        [DescriptionAttribute("Пожар 2")]
        Fire2 = 3,

        [DescriptionAttribute("Тест")]
        Test = 4,

        [DescriptionAttribute("Неисправность")]
        Failure = 5,

        [DescriptionAttribute("Обход")]
        Ignore = 6,

        [DescriptionAttribute("Включено")]
        On = 7,

        [DescriptionAttribute("Выключено")]
        Off = 8,

        [DescriptionAttribute("Включается")]
        TurningOn = 9,

        [DescriptionAttribute("Выключается")]
        TurningOff = 10,

        [DescriptionAttribute("Включить")]
        TurnOn = 11,

        [DescriptionAttribute("Отменить задержку")]
        CancelDelay = 12,

        [DescriptionAttribute("Выключить")]
        TurnOff = 13,

        [DescriptionAttribute("Остановить")]
        Stop = 14,

        [DescriptionAttribute("Запретить пуск")]
        ForbidStart = 15,

        [DescriptionAttribute("Включить немедленно")]
        TurnOnNow = 16,

        [DescriptionAttribute("Выключить немедленно")]
        TurnOffNow = 17,
    }
}