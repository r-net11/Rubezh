using System;
using System.ComponentModel;

namespace RubezhAPI
{
	[Flags]
	public enum ValidationErrorType
	{
		[Description("Устройство не подключено к зоне")]
		DeviceNotConnected = 0,

		[Description("Отсутствует логика срабатывания исполнительного устройства")]
		DeviceHaveNoLogic = 1,

		[Description("Количество подключенных к зоне датчиков")]
		ZoneSensorQuantity = 2,

		[Description("Несвязанные элементы плана")]
		NotBoundedElements = 3,

		[Description("Датчик не подключен к ТД")]
		SensorNotConnected = 4,

		[Description("Невозможно управлять устройством СУ, содержащим собственную логику")]
		DeviceHaveSelfLogik = 5,
	}
}