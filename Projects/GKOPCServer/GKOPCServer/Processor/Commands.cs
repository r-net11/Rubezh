using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace GKOPCServer
{
	public enum Commands
	{
		[Description("Перевести в автоматический режим")]
		SetAutomaticMode = 0,
		[Description("Перевести в ручной режим")]
		SetManualMode = 1,
		[Description("Перевести в отключенный режим")]
		SetDisabledMode = 2,
		[Description("Включить")]
		TurnOn = 3,
		[Description("Выключить")]
		TurnOff = 4,
		[Description("Включить немедленно")]
		TurnOnNow = 5,
		[Description("Выключить немедленно")]
		TurnOffNow = 6,
		[Description("Остановить")]
		Stop = 7,
		[Description("Сбросить")]
		Reset = 8
	}
}