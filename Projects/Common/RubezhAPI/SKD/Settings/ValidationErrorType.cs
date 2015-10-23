using System;
using System.ComponentModel;

namespace RubezhAPI
{
	[Flags]
	public enum ValidationErrorType
	{
		[Description("Устройство не подключено к зоне")]
		DeviceNotConnected = 1,

		[Description("Отсутствует логика срабатывания исполнительного устройства")]
		DeviceHaveNoLogic = 2,

		[Description("Количество подключенных к зоне датчиков")]
		ZoneSensorQuantity = 3,

		[Description("Несвязанные элементы плана")]
		NotBoundedElements = 3,

		[Description("Датчик не подключен к ТД")]
		SensorNotConnected = 4,

		[Description("Исполнительное устройство содержит собственную логику")]
		DeviceHaveSelfLogik = 5,
	}
}