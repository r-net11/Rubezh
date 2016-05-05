using System.ComponentModel;
using LocalizationConveters;

namespace FiresecAPI.Models
{
	public enum PermissionType
	{
        //[DescriptionAttribute("Все")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "All")]
		All,

        //[DescriptionAttribute("Администратор")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Adm_All")]
		Adm_All,

        //[DescriptionAttribute("Просмотр конфигурации")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Adm_ViewConfig")]
		Adm_ViewConfig,

        //[DescriptionAttribute("Применение конфигурации")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Adm_SetNewConfig")]
		Adm_SetNewConfig,

        //[DescriptionAttribute("Запись конфигурации в устройства")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Adm_WriteDeviceConfig")]
		Adm_WriteDeviceConfig,

        //[DescriptionAttribute("Изменение ПО в устройствах")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Adm_ChangeDevicesSoft")]
		Adm_ChangeDevicesSoft,

        //[DescriptionAttribute("Управление правами пользователей")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Adm_Security")]
		Adm_Security,

        //[DescriptionAttribute("ОЗ")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Oper_All")]
		Oper_All,

        //[DescriptionAttribute("Вход")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Oper_Login")]
		Oper_Login,

        //[DescriptionAttribute("Выход")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Oper_Logout")]
		Oper_Logout,

        //[DescriptionAttribute("Выход без пароля")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Oper_LogoutWithoutPassword")]
		Oper_LogoutWithoutPassword,

        //[DescriptionAttribute("Не требуется подтверждение тревог")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Oper_NoAlarmConfirm")]
		Oper_NoAlarmConfirm,

        //[DescriptionAttribute("Управление устройствами, зонами")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Oper_ControlDevices")]
		Oper_ControlDevices,

        //[DescriptionAttribute("Управление интерфейсом")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Oper_ChangeView")]
		Oper_ChangeView,

        //[DescriptionAttribute("Разрешить не подтверждать команды паролем")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Oper_MayNotConfirmCommands")]
		Oper_MayNotConfirmCommands,

        //[DescriptionAttribute("СКД")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Oper_SKD")]
		Oper_SKD,

        //[DescriptionAttribute("Управление графиками СКД")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Oper_GKSchedules")]
		Oper_GKSchedules, //TODO: Rename

        //[DescriptionAttribute("Просмотр журнала")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Oper_Journal_View")]
		Oper_Journal_View,

        //[DescriptionAttribute("Просмотр архива")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Oper_Archive_View")]
		Oper_Archive_View,

        //[DescriptionAttribute("Настройка отображения архива")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Oper_Archive_Settings")]
		Oper_Archive_Settings,

        //[DescriptionAttribute("Список точек доступа")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Oper_Reports_Doors")]
		Oper_Reports_Doors,

        //[DescriptionAttribute("Отчет по событиям системы контроля доступа")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Oper_Reports_Events")]
		Oper_Reports_Events,

        //[DescriptionAttribute("Маршрут сотрудника/посетителя")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Oper_Reports_EmployeeRoot")]
		Oper_Reports_EmployeeRoot,

        //[DescriptionAttribute("Сведения о пропусках")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Oper_Reports_Cards")]
		Oper_Reports_Cards,

        //[DescriptionAttribute("Доступ в зоны сотрудников/посетителей")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Oper_Reports_Employees_Access")]
		Oper_Reports_Employees_Access,

        //[DescriptionAttribute("Права доступа сотрудников/посетителей")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Oper_Reports_Employees_Rights")]
		Oper_Reports_Employees_Rights,

        //[DescriptionAttribute("Список подразделений организации")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Oper_Reports_Departments")]
		Oper_Reports_Departments,

        //[DescriptionAttribute("Список должностей организации")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Oper_Reports_Positions")]
		Oper_Reports_Positions,

        //[DescriptionAttribute("Местонахождение сотрудников/посетителей")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Oper_Reports_EmployeeZone")]
		Oper_Reports_EmployeeZone,

        //[DescriptionAttribute("Справка о сотруднике/посетителе")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Oper_Reports_Employee")]
		Oper_Reports_Employee,

        //[DescriptionAttribute("Дисциплинарный отчет")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Oper_Reports_Discipline")]
		Oper_Reports_Discipline,

        //[DescriptionAttribute("Отчет по графикам работ")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Oper_Reports_Schedules")]
		Oper_Reports_Schedules,

        //[DescriptionAttribute("Отчет по оправдательным документам")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Oper_Reports_Documents")]
		Oper_Reports_Documents,

        //[DescriptionAttribute("Справка по отработанному времени")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Oper_Reports_WorkTime")]
		Oper_Reports_WorkTime,

        //[DescriptionAttribute("Табель учета рабочего времени (форма Т-13)")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Oper_Reports_T13")]
		Oper_Reports_T13,

        //[DescriptionAttribute("Просмотр устройств")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Oper_Strazh_Devices_View")]
		Oper_Strazh_Devices_View,

        //[DescriptionAttribute("Управление замками")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Oper_Strazh_Devices_Control")]
		Oper_Strazh_Devices_Control,

        //[DescriptionAttribute("Просмотр зон")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Oper_Strazh_Zones_View")]
		Oper_Strazh_Zones_View,

        //[DescriptionAttribute("Управление замками")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Oper_Strazh_Zones_Control")]
		Oper_Strazh_Zones_Control,

        //[DescriptionAttribute("Просмотр точек доступа")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Oper_Strazh_Doors_View")]
		Oper_Strazh_Doors_View,

        //[DescriptionAttribute("Управление замками")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Oper_Strazh_Doors_Control")]
		Oper_Strazh_Doors_Control,

        //[DescriptionAttribute("Просмотр сотрудников")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Oper_SKD_Employees_View")]
		Oper_SKD_Employees_View,

        //[DescriptionAttribute("Создание, редактирование, архивирование, восстановление сотрудника")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Oper_SKD_Employees_Edit")]
		Oper_SKD_Employees_Edit,

        //[DescriptionAttribute("Просмотр посетителей")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Oper_SKD_Guests_View")]
		Oper_SKD_Guests_View,

        //[DescriptionAttribute("Создание, редактирование, архивирование, восстановление посетителя")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Oper_SKD_Guests_Edit")]
		Oper_SKD_Guests_Edit,

        //[DescriptionAttribute("Просмотр подразделений")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Oper_SKD_Departments_View")]
		Oper_SKD_Departments_View,

        //[DescriptionAttribute("Создание, редактирование, архивирование, восстановление подразделения")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Oper_SKD_Departments_Etit")]
		Oper_SKD_Departments_Etit,

        //[DescriptionAttribute("Добавление сотрудника в подразделение, удаление сотрудника из подразделения")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Oper_SKD_Departments_Employees")]
		Oper_SKD_Departments_Employees,

        //[DescriptionAttribute("Просмотра должностей")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Oper_SKD_Positions_View")]
		Oper_SKD_Positions_View,

        //[DescriptionAttribute("Создание, редактирование, архивирование, восстановление должности")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Oper_SKD_Positions_Etit")]
		Oper_SKD_Positions_Etit,

        //[DescriptionAttribute("Просмотр списка дополнительных колонок")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Oper_SKD_AdditionalColumns_View")]
		Oper_SKD_AdditionalColumns_View,

        //[DescriptionAttribute("Создание, редактирование, архивирование, восстановление дополнительных колонок")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Oper_SKD_AdditionalColumns_Etit")]
		Oper_SKD_AdditionalColumns_Etit,

        //[DescriptionAttribute("Просмотр пропусков")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Oper_SKD_Cards_View")]
		Oper_SKD_Cards_View,

        //[DescriptionAttribute("Архивирование, восстановление пропусков")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Oper_SKD_Cards_Etit")]
		Oper_SKD_Cards_Etit,

        //[DescriptionAttribute("Сброс флага запрета повторного прохода")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Oper_SKD_Cards_ResetRepeatEnter")]
		Oper_SKD_Cards_ResetRepeatEnter,

        //[DescriptionAttribute("Просмотр шаблонов доступа")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Oper_SKD_AccessTemplates_View")]
		Oper_SKD_AccessTemplates_View,

        //[DescriptionAttribute("Создание, редактирование, архивирование, восстановление шаблонов доступа")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Oper_SKD_AccessTemplates_Etit")]
		Oper_SKD_AccessTemplates_Etit,

        //[DescriptionAttribute("Просмотр шаблонов пропусков")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Oper_SKD_PassCards_View")]
		Oper_SKD_PassCards_View,

        //[DescriptionAttribute("Создание, редактирование, архивирование, восстановление шаблонов пропусков")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Oper_SKD_PassCards_Etit")]
		Oper_SKD_PassCards_Etit,

        //[DescriptionAttribute("Просмотр организаций")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Oper_SKD_Organisations_View")]
		Oper_SKD_Organisations_View,

        //[DescriptionAttribute("Создание, редактирование, архивирование, восстановление организации")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Oper_SKD_Organisations_Edit")]
		Oper_SKD_Organisations_Edit,

        //[DescriptionAttribute("Привязка пользователей к организации, открепление пользователей от организации")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Oper_SKD_Organisations_Users")]
		Oper_SKD_Organisations_Users,

        //[DescriptionAttribute("Привязка точек доступа к организации, открепление точек доступа от организации")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Oper_SKD_Organisations_Doors")]
		Oper_SKD_Organisations_Doors,

        //[DescriptionAttribute("Просмотр дневных графиков")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Oper_SKD_TimeTrack_DaySchedules_View")]
		Oper_SKD_TimeTrack_DaySchedules_View,

        //[DescriptionAttribute("Создание, редактирование, архивирование, восстановление дневных графиков")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Oper_SKD_TimeTrack_DaySchedules_Edit")]
		Oper_SKD_TimeTrack_DaySchedules_Edit,

        //[DescriptionAttribute("Просмотр графиков")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Oper_SKD_TimeTrack_ScheduleSchemes_View")]
		Oper_SKD_TimeTrack_ScheduleSchemes_View,

        //[DescriptionAttribute("Создание, редактирование, архивирование, восстановление графиков")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Oper_SKD_TimeTrack_ScheduleSchemes_Edit")]
		Oper_SKD_TimeTrack_ScheduleSchemes_Edit,

        //[DescriptionAttribute("Просмотр сокращённых дней")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Oper_SKD_TimeTrack_Holidays_View")]
		Oper_SKD_TimeTrack_Holidays_View,

        //[DescriptionAttribute("Добавление, редактирование, архивирование, восстановление сокращённых дней")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Oper_SKD_TimeTrack_Holidays_Edit")]
		Oper_SKD_TimeTrack_Holidays_Edit,

        //[DescriptionAttribute("Просмотр графиков работ")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Oper_SKD_TimeTrack_Schedules_View")]
		Oper_SKD_TimeTrack_Schedules_View,

        //[DescriptionAttribute("Создание, редактирование, архивирование, восстановление графиков работ")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Oper_SKD_TimeTrack_Schedules_Edit")]
		Oper_SKD_TimeTrack_Schedules_Edit,

        //[DescriptionAttribute("Просмотр журнала учета рабочего времени")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Oper_SKD_TimeTrack_Report_View")]
		Oper_SKD_TimeTrack_Report_View,

        //[DescriptionAttribute("Ручное редактирование графика проходов")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Oper_SKD_TimeTrack_Parts_Edit")]
		Oper_SKD_TimeTrack_Parts_Edit,

        //[DescriptionAttribute("Создание, редактирование, удаление, оправдательных документов")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Oper_SKD_TimeTrack_Documents_Edit")]
		Oper_SKD_TimeTrack_Documents_Edit,

		//[DescriptionAttribute("Ведение справочника видов оправдательных документов")]
        [LocalizedDescription(typeof(Resources.Language.Security.PermissionType), "Oper_SKD_TimeTrack_DocumentTypes_Edit")]
		Oper_SKD_TimeTrack_DocumentTypes_Edit,
	}
}