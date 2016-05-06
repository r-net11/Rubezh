using System.ComponentModel;

namespace StrazhAPI.Models
{
	public enum PermissionType
	{
		[Description("Все")]
		All,

		[Description("Администратор")]
		Adm_All,

		[Description("Просмотр конфигурации")]
		Adm_ViewConfig,

		[Description("Применение конфигурации")]
		Adm_SetNewConfig,

		[Description("Запись конфигурации в устройства")]
		Adm_WriteDeviceConfig,

		[Description("Изменение ПО в устройствах")]
		Adm_ChangeDevicesSoft,

		[Description("Управление правами пользователей")]
		Adm_Security,

		[Description("ОЗ")]
		Oper_All,

		[Description("Вход")]
		Oper_Login,

		[Description("Выход")]
		Oper_Logout,

		[Description("Выход без пароля")]
		Oper_LogoutWithoutPassword,

		[Description("Не требуется подтверждение тревог")]
		Oper_NoAlarmConfirm,

		[Description("Управление устройствами, зонами")]
		Oper_ControlDevices,

		[Description("Управление интерфейсом")]
		Oper_ChangeView,

		[Description("Разрешить не подтверждать команды паролем")]
		Oper_MayNotConfirmCommands,

		[Description("СКД")]
		Oper_SKD,

		[Description("Управление графиками СКД")]
		Oper_GKSchedules, //TODO: Rename

		[Description("Просмотр журнала")]
		Oper_Journal_View,

		[Description("Просмотр архива")]
		Oper_Archive_View,

		[Description("Настройка отображения архива")]
		Oper_Archive_Settings,

		[Description("Список точек доступа")]
		Oper_Reports_Doors,

		[Description("Отчет по событиям системы контроля доступа")]
		Oper_Reports_Events,

		[Description("Маршрут сотрудника/посетителя")]
		Oper_Reports_EmployeeRoot,

		[Description("Сведения о пропусках")]
		Oper_Reports_Cards,

		[Description("Доступ в зоны сотрудников/посетителей")]
		Oper_Reports_Employees_Access,

		[Description("Права доступа сотрудников/посетителей")]
		Oper_Reports_Employees_Rights,

		[Description("Список подразделений организации")]
		Oper_Reports_Departments,

		[Description("Список должностей организации")]
		Oper_Reports_Positions,

		[Description("Местонахождение сотрудников/посетителей")]
		Oper_Reports_EmployeeZone,

		[Description("Справка о сотруднике/посетителе")]
		Oper_Reports_Employee,

		[Description("Дисциплинарный отчет")]
		Oper_Reports_Discipline,

		[Description("Отчет по графикам работ")]
		Oper_Reports_Schedules,

		[Description("Отчет по оправдательным документам")]
		Oper_Reports_Documents,

		[Description("Справка по отработанному времени")]
		Oper_Reports_WorkTime,

		[Description("Табель учета рабочего времени (форма Т-13)")]
		Oper_Reports_T13,

		[Description("Просмотр устройств")]
		Oper_Strazh_Devices_View,

		[Description("Управление замками")]
		Oper_Strazh_Devices_Control,

		[Description("Просмотр зон")]
		Oper_Strazh_Zones_View,

		[Description("Управление замками")]
		Oper_Strazh_Zones_Control,

		[Description("Просмотр точек доступа")]
		Oper_Strazh_Doors_View,

		[Description("Управление замками")]
		Oper_Strazh_Doors_Control,

		[Description("Просмотр сотрудников")]
		Oper_SKD_Employees_View,

		[Description("Создание, редактирование, архивирование, восстановление сотрудника")]
		Oper_SKD_Employees_Edit,

		[Description("Просмотр посетителей")]
		Oper_SKD_Guests_View,

		[Description("Создание, редактирование, архивирование, восстановление посетителя")]
		Oper_SKD_Guests_Edit,

		[Description("Просмотр подразделений")]
		Oper_SKD_Departments_View,

		[Description("Создание, редактирование, архивирование, восстановление подразделения")]
		Oper_SKD_Departments_Etit,

		[Description("Добавление сотрудника в подразделение, удаление сотрудника из подразделения")]
		Oper_SKD_Departments_Employees,

		[Description("Просмотра должностей")]
		Oper_SKD_Positions_View,

		[Description("Создание, редактирование, архивирование, восстановление должности")]
		Oper_SKD_Positions_Etit,

		[Description("Просмотр списка дополнительных колонок")]
		Oper_SKD_AdditionalColumns_View,

		[Description("Создание, редактирование, архивирование, восстановление дополнительных колонок")]
		Oper_SKD_AdditionalColumns_Etit,

		[Description("Просмотр пропусков")]
		Oper_SKD_Cards_View,

		[Description("Архивирование, восстановление пропусков")]
		Oper_SKD_Cards_Etit,

		[Description("Сброс флага запрета повторного прохода")]
		Oper_SKD_Cards_ResetRepeatEnter,

		[Description("Просмотр шаблонов доступа")]
		Oper_SKD_AccessTemplates_View,

		[Description("Создание, редактирование, архивирование, восстановление шаблонов доступа")]
		Oper_SKD_AccessTemplates_Etit,

		[Description("Просмотр шаблонов пропусков")]
		Oper_SKD_PassCards_View,

		[Description("Создание, редактирование, архивирование, восстановление шаблонов пропусков")]
		Oper_SKD_PassCards_Etit,

		[Description("Просмотр организаций")]
		Oper_SKD_Organisations_View,

		[Description("Создание, редактирование, архивирование, восстановление организации")]
		Oper_SKD_Organisations_Edit,

		[Description("Привязка пользователей к организации, открепление пользователей от организации")]
		Oper_SKD_Organisations_Users,

		[Description("Привязка точек доступа к организации, открепление точек доступа от организации")]
		Oper_SKD_Organisations_Doors,

		[Description("Просмотр дневных графиков")]
		Oper_SKD_TimeTrack_DaySchedules_View,

		[Description("Создание, редактирование, архивирование, восстановление дневных графиков")]
		Oper_SKD_TimeTrack_DaySchedules_Edit,

		[Description("Просмотр графиков")]
		Oper_SKD_TimeTrack_ScheduleSchemes_View,

		[Description("Создание, редактирование, архивирование, восстановление графиков")]
		Oper_SKD_TimeTrack_ScheduleSchemes_Edit,

		[Description("Просмотр сокращённых дней")]
		Oper_SKD_TimeTrack_Holidays_View,

		[Description("Добавление, редактирование, архивирование, восстановление сокращённых дней")]
		Oper_SKD_TimeTrack_Holidays_Edit,

		[Description("Просмотр графиков работ")]
		Oper_SKD_TimeTrack_Schedules_View,

		[Description("Создание, редактирование, архивирование, восстановление графиков работ")]
		Oper_SKD_TimeTrack_Schedules_Edit,

		[Description("Просмотр журнала учета рабочего времени")]
		Oper_SKD_TimeTrack_Report_View,

		[Description("Ручное редактирование графика проходов")]
		Oper_SKD_TimeTrack_Parts_Edit,

		[Description("Создание, редактирование, удаление, оправдательных документов")]
		Oper_SKD_TimeTrack_Documents_Edit,

		[Description("Ведение справочника видов оправдательных документов")]
		Oper_SKD_TimeTrack_DocumentTypes_Edit,
	}
}