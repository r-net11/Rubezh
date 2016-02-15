using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace GKWebService.Models.GK
{
	public enum DeviceControlRegime
	{
		[Description("автоматика")]
		Automatic,

		[Description("ручное")]
		Manual,

		[Description("отключение")]
		Ignore
	}
}