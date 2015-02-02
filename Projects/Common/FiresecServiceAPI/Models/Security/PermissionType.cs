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

		[DescriptionAttribute("ОЗ: Управление особо охраняемыми охранными зонами")]
		Oper_ExtraGuardZone,



		[DescriptionAttribute("Просмотр журнала")]
		Oper_Journal_View,

		[DescriptionAttribute("Просмотр архива")]
		Oper_Archive_View,

		[DescriptionAttribute("Настройка отображения архива")]
		Oper_Archive_Settings,

		[DescriptionAttribute("ОЗ: Список точек доступа")]
		Oper_Reports_Doors,

		[DescriptionAttribute("ОЗ: Отчет по событиям системы контроля доступа")]
		Oper_Reports_Events,

		[DescriptionAttribute("ОЗ: Маршрут сотрудника/посетителя")]
		Oper_Reports_EmployeeRoot,

		[DescriptionAttribute("ОЗ: Сведения о пропусках")]
		Oper_Reports_Cards,

		[DescriptionAttribute("ОЗ: Доступ в зоны сотрудников/посетителей")]
		Oper_Reports_Employees_Access,

		[DescriptionAttribute("ОЗ: Права доступа сотрудников/посетителей")]
		Oper_Reports_Employees_Rights,

		[DescriptionAttribute("ОЗ: Список подразделений организации")]
		Oper_Reports_Departments,

		[DescriptionAttribute("ОЗ: Список должностей организации")]
		Oper_Reports_Positions,

		[DescriptionAttribute("ОЗ: Местонахождение сотрудников/посетителей")]
		Oper_Reports_EmployeeZone,

		[DescriptionAttribute("ОЗ: Справка о сотруднике/посетителе")]
		Oper_Reports_Employee,

		[DescriptionAttribute("ОЗ: Дисциплинарный отчет")]
		Oper_Reports_Discipline,

		[DescriptionAttribute("ОЗ: Отчет по графикам работ")]
		Oper_Reports_Schedules,

		[DescriptionAttribute("ОЗ: Отчет по оправдательным документам")]
		Oper_Reports_Documents,

		[DescriptionAttribute("ОЗ: Справка по отработанному времени")]
		Oper_Reports_WorkTime,

		[DescriptionAttribute("ОЗ: Табель учета рабочего времени (форма Т-13)")]
		Oper_Reports_T13,

		[DescriptionAttribute("ОЗ: Просмотр устройств")]
		Oper_Strazh_Devices_View,

		[DescriptionAttribute("ОЗ: Управление замками")]
		Oper_Strazh_Devices_Control,

		[DescriptionAttribute("ОЗ: Просмотр зон")]
		Oper_Strazh_Zones_View,

		[DescriptionAttribute("ОЗ: Управление замками")]
		Oper_Strazh_Zones_Control,

		[DescriptionAttribute("ОЗ: Просмотр точек доступа")]
		Oper_Strazh_Doors_View,

		[DescriptionAttribute("ОЗ: Управление замками")]
		Oper_Strazh_Doors_Control,

		[DescriptionAttribute("ОЗ: Просмотр сотрудников")]
		Oper_SKD_Employees_View,

		[DescriptionAttribute("ОЗ: Создание, редактирование, архивирование, восстановление сотрудника")]
		Oper_SKD_Employees_Edit,

		[DescriptionAttribute("ОЗ: Просмотр пропусков сотрудника")]
		Oper_SKD_Employees_Cards_View,

		[DescriptionAttribute("ОЗ: Добавление, редактирование, деактивация, пропусков сотрудников")]
		Oper_SKD_Employees_Cards_Edit,

		[DescriptionAttribute("ОЗ: Просмотр посетителей")]
		Oper_SKD_Guests_View,

		[DescriptionAttribute("ОЗ: Создание, редактирование, архивирование, восстановление посетителя")]
		Oper_SKD_Guests_Edit,

		[DescriptionAttribute("ОЗ: Просмотр пропусков посетителя")]
		Oper_SKD_Guests_Cards_View,

		[DescriptionAttribute("ОЗ: Добавление, редактирование, деактивация, пропусков посетителя")]
		Oper_SKD_Guests_Cards_Edit,

		[DescriptionAttribute("ОЗ: Просмотр подразделений")]
		Oper_SKD_Departments_View,

		[DescriptionAttribute("ОЗ: Создание, редактирование, архивирование, восстановление подразделения")]
		Oper_SKD_Departments_Etit,

		[DescriptionAttribute("ОЗ: Добавление сотрудника в подразделение, удаление сотрудника из подразделения")]
		Oper_SKD_Departments_Employees,

		[DescriptionAttribute("ОЗ: Редактирование сотрудника")]
		Oper_SKD_Departments_Employees_Edit,

		[DescriptionAttribute("ОЗ: Просмотра должностей")]
		Oper_SKD_Positions_View,

		[DescriptionAttribute("ОЗ: Создание, редактирование, архивирование, восстановление должности")]
		Oper_SKD_Positions_Etit,

		[DescriptionAttribute("ОЗ: Присвоение сотруднику должности, открепление должности от сотрудника")]
		Oper_SKD_Positions_Employees,

		[DescriptionAttribute("ОЗ: Редактирование сотрудника")]
		Oper_SKD_Positions_Employees_Edit,

		[DescriptionAttribute("ОЗ: Просмотр списка дополнительных колонок")]
		Oper_SKD_AdditionalColumns_View,

		[DescriptionAttribute("ОЗ: Создание, редактирование, архивирование, восстановление дополнительных колонок")]
		Oper_SKD_AdditionalColumns_Etit,

		[DescriptionAttribute("ОЗ: Присвоение сотруднику доп. колонок, открепление доп. колонок от сотрудника")]
		Oper_SKD_AdditionalColumns_Employees,

		[DescriptionAttribute("ОЗ: Редактирование сотрудника")]
		Oper_SKD_AdditionalColumns_Employees_Edit,

		[DescriptionAttribute("ОЗ: Просмотр пропусков")]
		Oper_SKD_Cards_View,

		[DescriptionAttribute("ОЗ: Архивирование, восстановление пропусков")]
		Oper_SKD_Cards_Etit,

		[DescriptionAttribute("ОЗ: Просмотр шаблонов доступа")]
		Oper_SKD_AccessTemplates_View,

		[DescriptionAttribute("ОЗ: Создание, редактирование, архивирование, восстановление шаблонов доступа")]
		Oper_SKD_AccessTemplates_Etit,

		[DescriptionAttribute("ОЗ: Просмотр шаблонов пропусков")]
		Oper_SKD_PassCards_View,

		[DescriptionAttribute("ОЗ: Создание, редактирование, архивирование, восстановление шаблонов пропусков")]
		Oper_SKD_PassCards_Etit,

		[DescriptionAttribute("ОЗ: Просмотр организаций")]
		Oper_SKD_Organisations_View,

		[DescriptionAttribute("ОЗ: Создание, редактирование, архивирование, восстановление организации")]
		Oper_SKD_Organisations_Edit,

		[DescriptionAttribute("ОЗ: Привязка пользователей к организации, открепление пользователей от организации")]
		Oper_SKD_Organisations_Users,

		[DescriptionAttribute("ОЗ: Привязка точек доступа к организации, открепление точек доступа от организации")]
		Oper_SKD_Organisations_Doors,

		[DescriptionAttribute("ОЗ: Просмотр дневных графиков")]
		Oper_SKD_TimeTrack_DaySchedules_View,

		[DescriptionAttribute("ОЗ: Создание, редактирование, архивирование, восстановление дневных графиков")]
		Oper_SKD_TimeTrack_DaySchedules_Edit,

		[DescriptionAttribute("ОЗ: Создание, редактирование, удаление интервалов")]
		Oper_SKD_TimeTrack_DaySchedules_IntervalsEdit,

		[DescriptionAttribute("ОЗ: Просмотр графиков")]
		Oper_SKD_TimeTrack_ScheduleSchemes_View,

		[DescriptionAttribute("ОЗ: Создание, редактирование, архивирование, восстановление графиков")]
		Oper_SKD_TimeTrack_ScheduleSchemes_Edit,

		[DescriptionAttribute("ОЗ: Прикрепление, открепление к графику дневных графиков")]
		Oper_SKD_TimeTrack_ScheduleSchemes_DayEdit,

		[DescriptionAttribute("ОЗ: Просмотр праздничных дней")]
		Oper_SKD_TimeTrack_Holidays_View,

		[DescriptionAttribute("ОЗ: Добавление, редактирование, архивирование, восстановление праздничных дней")]
		Oper_SKD_TimeTrack_Holidays_Edit,

		[DescriptionAttribute("ОЗ: Просмотр графиков работ")]
		Oper_SKD_TimeTrack_Schedules_View,

		[DescriptionAttribute("ОЗ: Создание, редактирование, архивирование, восстановление графиков работ")]
		Oper_SKD_TimeTrack_Schedules_Edit,

		[DescriptionAttribute("ОЗ: Прикрепление, редактирование, открепление зон, привязанных к графику работ")]
		Oper_SKD_TimeTrack_Schedules_ScheduleEdit,

		[DescriptionAttribute("ОЗ: Просмотр журнала учета рабочего времени")]
		Oper_SKD_TimeTrack_Report_View,

		[DescriptionAttribute("ОЗ: Создание, редактирование, удаление, оправдательных документов")]
		Oper_SKD_TimeTrack_Documents_Edit,

		[DescriptionAttribute("ОЗ: Добавление, удаление, редактирование типа документов в списке 'Типы документов'")]
		Oper_SKD_TimeTrack_DocumentTypes_Edit,

		[DescriptionAttribute("ОЗ: Настройка планов обмена")]
		Oper_SKD_Integration_Settings,

		[DescriptionAttribute("ОЗ: Экспорт журнала")]
		Oper_SKD_Export_Journal,

		[DescriptionAttribute("ОЗ: Экспорт конфигурации")]
		Oper_SKD_Export_Config,

		[DescriptionAttribute("ОЗ: Экспорт и импорт картотеки")]
		Oper_SKD_ExportImport_HR,

		[DescriptionAttribute("ОЗ: Экспорт справочника организаций")]
		Oper_SKD_Export_Organisations,
	}
}