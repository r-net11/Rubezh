using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace GKModule
{
	public enum DeviceControlRegime
	{
		[Description("Автоматика")]
		Automatic,

		[Description("Ручное")]
		Manual,

		[Description("Отключение")]
		Ignore
	}
}