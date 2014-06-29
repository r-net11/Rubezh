using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace FiresecAPI.Events
{
	public enum GlobalSubsystemType
	{
		[Description("Система")]
		System,

		[Description("ГК")]
		GK,

		[Description("СКД")]
		SKD,

		[Description("Видео")]
		Video
	}
}