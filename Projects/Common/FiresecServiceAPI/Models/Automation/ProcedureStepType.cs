using System.ComponentModel;
using LocalizationConveters;

namespace StrazhAPI.Automation
{
	public enum ProcedureStepType
	{
		//[Description("Условие")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ProcedureStepType), "If")]
		If,

		//[Description("Выполняется")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ProcedureStepType), "IfYes")]
        IfYes,

		//[Description("Не выполняется")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ProcedureStepType), "IfNo")]
        IfNo,

		//[Description("Цикл по списку")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ProcedureStepType), "Foreach")]
        Foreach,

		//[Description("Цикл со счетчиком (For)")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ProcedureStepType), "For")]
        For,

		//[Description("Цикл с условием (While)")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ProcedureStepType), "While")]
        While,

		//[Description("Выйти из цикла")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ProcedureStepType), "Break")]
        Break,

		//[Description("Продолжить цикл")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ProcedureStepType), "Continue")]
        Continue,

		//[Description("Тело цикла")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ProcedureStepType), "ForeachBody")]
        ForeachBody,

		//[Description("Функция выбора процедуры")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ProcedureStepType), "ProcedureSelection")]
        ProcedureSelection,

		//[Description("Получить значение свойства объекта")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ProcedureStepType), "GetObjectProperty")]
        GetObjectProperty,

		//[Description("Проигрывание звука")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ProcedureStepType), "PlaySound")]
        PlaySound,

		//[Description("Арифметическая операция")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ProcedureStepType), "Arithmetics")]
        Arithmetics,

		//[Description("Найти объекты")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ProcedureStepType), "FindObjects")]
        FindObjects,

		//[Description("Изменение списка")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ProcedureStepType), "ChangeList")]
        ChangeList,

		//[Description("Проверка прав")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ProcedureStepType), "CheckPermission")]
        CheckPermission,

		//[Description("Получить значение журнала")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ProcedureStepType), "GetJournalItem")]
        GetJournalItem,

		//[Description("Получить размер списка")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ProcedureStepType), "GetListCount")]
        GetListCount,

		//[Description("Получить элемент списка")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ProcedureStepType), "GetListItem")]
        GetListItem,

		//[Description("Показать сообщение")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ProcedureStepType), "ShowMessage")]
        ShowMessage,

		//[Description("Добавить запись в журнал")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ProcedureStepType), "AddJournalItem")]
        AddJournalItem,

		//[Description("Выход из процедуры")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ProcedureStepType), "Exit")]
        Exit,

		//[Description("Задание значения переменной")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ProcedureStepType), "SetValue")]
        SetValue,

		//[Description("Запуск программы")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ProcedureStepType), "RunProgram")]
        RunProgram,

		//[Description("Инкремент значения переменной")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ProcedureStepType), "IncrementValue")]
        IncrementValue,

		//[Description("Отправить сообщение по электронной почте")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ProcedureStepType), "SendEmail")]
        SendEmail,

		//[Description("Пауза")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ProcedureStepType), "Pause")]
        Pause,

		//[Description("Случайное значение")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ProcedureStepType), "Random")]
        Random,

		//[Description("Управление устройством СКД")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ProcedureStepType), "ControlSKDDevice")]
        ControlSKDDevice,

		//[Description("Управление зоной СКД")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ProcedureStepType), "ControlSKDZone")]
        ControlSKDZone,

		//[Description("Управление точкой доступа")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ProcedureStepType), "ControlDoor")]
        ControlDoor,

		//[Description("Чтение свойства визуального элемента")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ProcedureStepType), "ControlVisualGet")]
        ControlVisualGet,

		//[Description("Установка свойства визуального элемента")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ProcedureStepType), "ControlVisualSet")]
        ControlVisualSet,

		//[Description("Чтение свойства элементами плана")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ProcedureStepType), "ControlPlanGet")]
        ControlPlanGet,

		//[Description("Установка свойства элементами плана")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ProcedureStepType), "ControlPlanSet")]
        ControlPlanSet,

		//[Description("Показать диалоговую форму")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ProcedureStepType), "ShowDialog")]
        ShowDialog,

		//[Description("Показать свойство объекта")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ProcedureStepType), "ShowProperty")]
        ShowProperty,

		//[Description("Генерировать идентификатор")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ProcedureStepType), "GenerateGuid")]
        GenerateGuid,

		//[Description("Назначить идентификатор события")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ProcedureStepType), "SetJournalItemGuid")]
        SetJournalItemGuid,

		//[Description("")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ProcedureStepType), "DoAction")]
        DoAction,

		//[Description("Экспорт организации")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ProcedureStepType), "ExportOrganisation")]
        ExportOrganisation,

		//[Description("Экспорт списка организаций")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ProcedureStepType), "ExportOrganisationList")]
        ExportOrganisationList,

		//[Description("Экспорт конфигурации")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ProcedureStepType), "ExportConfiguration")]
        ExportConfiguration,

		//[Description("Экспорт журнала")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ProcedureStepType), "ExportJournal")]
        ExportJournal,

		//[Description("Импорт организации")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ProcedureStepType), "ImportOrganisation")]
        ImportOrganisation,

		//[Description("Импорт списка организаций")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ProcedureStepType), "ImportOrganisationList")]
        ImportOrganisationList,

		//[Description("Управление PTZ камерой")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ProcedureStepType), "Ptz")]
        Ptz,

		//[Description("Начать запись")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ProcedureStepType), "StartRecord")]
        StartRecord,

		//[Description("Остановить запись")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ProcedureStepType), "StopRecord")]
        StopRecord,

		//[Description("Вызвать тревогу в RVI Оператор")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ProcedureStepType), "RviAlarm")]
        RviAlarm,

		//[Description("Экспорт отчета")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ProcedureStepType), "ExportReport")]
        ExportReport,

		//[Description("Получить текущее время")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ProcedureStepType), "GetDateTimeNow")]
        GetDateTimeNow,

		//[Description("Получение свойства устройства СКД")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ProcedureStepType), "GetSkdDeviceProperty")]
        GetSkdDeviceProperty,

		//[Description("Получение свойства точки доступа")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ProcedureStepType), "GetDoorProperty")]
        GetDoorProperty,

		//[Description("Получение свойства зоны СКД")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.ProcedureStepType), "GetSkdZoneProperty")]
        GetSkdZoneProperty,

		[Description("Вызов сценария FireSec")]
		ExecuteFiresecScript,

		[Description("Отправить команду")]
		SendOPCScript
	}
}