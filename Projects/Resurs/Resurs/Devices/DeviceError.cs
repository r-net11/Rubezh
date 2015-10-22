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
		CommunicationError,
		[Description("Ошибка конфигурации ")]
		ConfigurationError,
		[Description("Часы не исправны")]
		RTCError
	}
}
