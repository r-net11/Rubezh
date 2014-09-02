using System.ComponentModel;

namespace FiresecAPI.Models
{
	public enum PermissionType
	{
		[DescriptionAttribute("Все")]
		All,

		[DescriptionAttribute("Администратор")]
		Adm_All,

		[DescriptionAttribute("АДМ: Просмотр конфигурации")]
		Adm_ViewConfig,

		[DescriptionAttribute("АДМ: Применение конфигурации")]
		Adm_SetNewConfig,

		[DescriptionAttribute("АДМ: Запись конфигурации в приборы")]
		Adm_WriteDeviceConfig,

		[DescriptionAttribute("АДМ: Изменение ПО в приборах")]
		Adm_ChangeDevicesSoft,

		[DescriptionAttribute("АДМ: Управление правами пользователей")]
		Adm_Security,

		//[DescriptionAttribute("АДМ: СКД")]
		//Adm_SKUD,

		[DescriptionAttribute("ОЗ")]
		Oper_All,

		[DescriptionAttribute("ОЗ: Вход")]
		Oper_Login,

		[DescriptionAttribute("ОЗ: Выход")]
		Oper_Logout,

		[DescriptionAttribute("ОЗ: Выход без пароля")]
		Oper_LogoutWithoutPassword,

		[DescriptionAttribute("ОЗ: Не требуется подтверждение тревог")]
		Oper_NoAlarmConfirm,

		//[DescriptionAttribute("ОЗ: Отключение в приборах Рубеж")]
		//Oper_AddToIgnoreList,

		//[DescriptionAttribute("ОЗ: Снятие отключения в приборах Рубеж")]
		//Oper_RemoveFromIgnoreList,

		[DescriptionAttribute("ОЗ: Постановка, снятие зон с охраны")]
		Oper_SecurityZone,

		[DescriptionAttribute("ОЗ: Управление устройствами, зонами и направлениями")]
		Oper_ControlDevices,

		[DescriptionAttribute("ОЗ: Управление интерфейсом")]
		Oper_ChangeView,

		[DescriptionAttribute("ОЗ: Разрешить не подтверждать команды паролем")]
		Oper_MayNotConfirmCommands,

		[DescriptionAttribute("ОЗ: СКД")]
		Oper_SKD,

		[DescriptionAttribute("ОЗ: Сотрудники картотеки СКД")]
		Oper_SKD_Employees,

		[DescriptionAttribute("ОЗ: Посетители картотеки СКД")]
		Oper_SKD_Guests,

		[DescriptionAttribute("ОЗ: Справочники картотеки СКД")]
		Oper_SKD_HR,

		[DescriptionAttribute("ОЗ: Организации картотеки СКД")]
		Oper_SKD_Organisations,
	}
}