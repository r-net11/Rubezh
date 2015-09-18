using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ResursAPI
{
	public enum DriverType
	{
		[Description("Система")]
		System,

		[Description("Сеть тип 1")]
		Network1,

		[Description("Счетчик тип 1.1")]
		Network1Device1,

		[Description("Счетчик тип 1.1")]
		Network1Device2,

		[Description("Сеть тип 2")]
		Network2,

		[Description("Счетчик тип 2.3")]
		Network2Device1,

		[Description("Счетчик тип 2.3")]
		Network2Device2,

		[Description("Счетчик тип 2.3")]
		Network2Device3,
	}
}