using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace FiresecAPI
{
	public enum CardType
	{
		[Description("Постоянный")]
		Constant,

		[Description("Временный")]
		Temporary,

		[Description("Разовый")]
		OneTime,

		[Description("Заблокирован")]
		Blocked
	}
}