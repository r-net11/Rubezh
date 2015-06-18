using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace FiresecAPI
{
	[Flags]
	public enum ValidationErrorType
	{
		[Description("Устройство не подключено к зоне")]
		DeviceNotConnected = 1,
		[Description("Отсутствует логика срабатывания исполнительного устройства")]
		DeviceHaveNoLogic = 2,
		[Description("Количество подключенных к зоне датчиков")]
		ZoneSensorQuantity = 16,
		[Description("Несвязанные элементы плана")]
		NotBoundedElements = 32,
	}
}
