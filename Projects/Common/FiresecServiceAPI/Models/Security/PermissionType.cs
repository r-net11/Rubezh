using System.ComponentModel;

namespace FiresecAPI.Models
{
    public enum PermissionType
    {
        [DescriptionAttribute("АДМ: Изменение конфигурации в БД")]
        Adm_ChangeConfigDb = 6,

        [DescriptionAttribute("АДМ: Изменение конфигурации в устройствах")]
        Adm_ChangeConfigDevices = 7,

        [DescriptionAttribute("АДМ: Изменение ПО в устройствах")]
        Adm_ChangeDevicesSoft = 8,

        [DescriptionAttribute("АДМ: Очистка журнала")]
        Adm_ClearJournal = 4,

        [DescriptionAttribute("АДМ: Просмотр конфигурации")]
        Adm_ViewConfig = 5,

        [DescriptionAttribute("АДМ: Управление правами пользователей")]
        Adm_Security = 9,

        [DescriptionAttribute("ОЗ: Включение автоматики")]
        Oper_AutoOn = 20,

        [DescriptionAttribute("ОЗ: Вход")]
        Oper_Login = 1,

        [DescriptionAttribute("ОЗ: Выход")]
        Oper_Logout = 2,

        [DescriptionAttribute("ОЗ: Выход без пароля")]
        Oper_LogoutWithoutPassword = 13,

        [DescriptionAttribute("ОЗ: Добавление в список обхода")]
        Oper_AddToIgnoreList = 16,

        [DescriptionAttribute("ОЗ: Дополнительные режимы")]
        Oper_AdditionalMode = 11,

        [DescriptionAttribute("ОЗ: Изменение размеров и положения окон")]
        Oper_ChangeLayout = 18,

        [DescriptionAttribute("ОЗ: Не требуется подтверждение тревог")]
        Oper_NoAlarmConfirm = 17,

        [DescriptionAttribute("ОЗ: Обработка тревог")]
        Oper_AlarmEdit = 3,

        [DescriptionAttribute("ОЗ: Постановка, снятие зон с охраны")]
        Oper_SecurityZone = 19,

        [DescriptionAttribute("ОЗ: Расширенное редактирование списка обхода")]
        Oper_IgnoreListEditing = 14,

        [DescriptionAttribute("ОЗ: Удаление из списка обхода")]
        Oper_RemoveFromIgnoreList = 15,

        [DescriptionAttribute("ОЗ: Управление показом планов")]
        Oper_ShowPlans = 12
    }
}
