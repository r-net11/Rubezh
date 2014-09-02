using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace FiresecAPI.SKD
{
	public enum SKDControllerDirectionType
	{
		[Description("Однопроходный")]
		Unidirect,

		[Description("Двухпроходный")]
		Bidirect
	}
}