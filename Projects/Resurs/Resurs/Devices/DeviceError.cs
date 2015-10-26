using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Resurs
{
	public enum DeviceError
	{
		[Description("Потеря связи")]
		Communication,
		[Description("Ошибка конфигурации ")]
		Configuration,
		[Description("Часы не исправны")]
		RTC,
		[Description("Ошибка COM-порта")]
		Port
	}
}
