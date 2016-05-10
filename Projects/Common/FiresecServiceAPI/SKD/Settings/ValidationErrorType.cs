using System;
using System.ComponentModel;
using LocalizationConveters;

namespace StrazhAPI
{
	[Flags]
	public enum ValidationErrorType
	{
		//[Description("Устройство не подключено к зоне")]
        [LocalizedDescription(typeof(Resources.Language.SKD.Settings.ValidationErrorType), "DeviceNotConnected")]
		DeviceNotConnected = 1,

        //[Description("Отсутствует логика срабатывания исполнительного устройства")]
        [LocalizedDescription(typeof(Resources.Language.SKD.Settings.ValidationErrorType), "DeviceHaveNoLogic")]
		DeviceHaveNoLogic = 2,

        //[Description("Количество подключенных к зоне датчиков")]
        [LocalizedDescription(typeof(Resources.Language.SKD.Settings.ValidationErrorType), "ZoneSensorQuantity")]
		ZoneSensorQuantity = 16,

        //[Description("Несвязанные элементы плана")]
        [LocalizedDescription(typeof(Resources.Language.SKD.Settings.ValidationErrorType), "NotBoundedElements")]
		NotBoundedElements = 32,
	}
}