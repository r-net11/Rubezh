using System.ComponentModel;

namespace FiresecAPI.Models
{
	public enum PermissionType
	{
		[DescriptionAttribute("АДМ: Просмотр конфигурации")]
		Adm_ViewConfig = 1,

		[DescriptionAttribute("АДМ: Применение конфигурации")]
		Adm_SetNewConfig = 2,

		[DescriptionAttribute("АДМ: Запись конфигурации в приборы")]
		Adm_WriteDeviceConfig = 3,

		[DescriptionAttribute("АДМ: Изменение ПО в приборах")]
		Adm_ChangeDevicesSoft = 4,

		[DescriptionAttribute("АДМ: Управление правами пользователей")]
		Adm_Security = 5,

		[DescriptionAttribute("АДМ: СКД")]
		Adm_SKUD = 6,

		[DescriptionAttribute("ОЗ: Вход")]
		Oper_Login = 101,

		[DescriptionAttribute("ОЗ: Выход")]
		Oper_Logout = 102,

		[DescriptionAttribute("ОЗ: Выход без пароля")]
		Oper_LogoutWithoutPassword = 103,

		[DescriptionAttribute("ОЗ: Не требуется подтверждение тревог")]
		Oper_NoAlarmConfirm = 104,

		[DescriptionAttribute("ОЗ: Отключение в приборах Рубеж")]
		Oper_AddToIgnoreList = 105,

		[DescriptionAttribute("ОЗ: Снятие отключения в приборах Рубеж")]
		Oper_RemoveFromIgnoreList = 106,

		//[DescriptionAttribute("ОЗ: Постановка, снятие зон с охраны")]
		//Oper_SecurityZone = 107,

		[DescriptionAttribute("ОЗ: Управление устройствами, зонами и направлениями")]
		Oper_ControlDevices = 108,

		[DescriptionAttribute("ОЗ: Управление интерфейсом")]
		Oper_ChangeView = 109,

		[DescriptionAttribute("ОЗ: Разрешить не подтверждать команды паролем")]
		Oper_MayNotConfirmCommands = 110,

		[DescriptionAttribute("ОЗ: Сотрудники картотеки СКД")]
		Oper_SKD_Employees = 111,

		[DescriptionAttribute("ОЗ: Посетители картотеки СКД")]
		Oper_SKD_Guests = 112,

		[DescriptionAttribute("ОЗ: Справочники картотеки СКД")]
		Oper_SKD_HR = 113,

		[DescriptionAttribute("ОЗ: Организации картотеки СКД")]
		Oper_SKD_Organisations = 114,
	}
}